using ImageViewer.Methods;
using ImageViewer.Model;
using ImageViewer.Model.Event;
using ImageViewer.View;
using ImageViewer.View.ImagesWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageViewer.ViewModel.ImageWindowViewModels
{
    public class ImageExplorerViewModel : BaseViewModel
    {
        private const int displayedImages = 6;
        private ObservableCollection<ObservableCollection<Image>> _imageList;
        public RelayCommand DoubleClickCommand { get; set; }
        public RelayCommand RemoveImageCommand { get; set; }
        public RelayCommand DialogCommandList { get; set; }
        public RelayCommand DialogCommandImage { get; set; }
        public ObservableCollection<ObservableCollection<Image>> ImageList
        {
            get
            {
                return _imageList;
            }
            set
            {
                _imageList = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("ImageListCount");
            }
        }

        public int ImageListCount {
            get
            {
                return _imageList.Count > displayedImages ? _imageList.Count : displayedImages;
            }
            set
            {
            }
        }

        public ImageExplorerViewModel()
        {
            DoubleClickCommand = new RelayCommand(DoubleClickExecute, DoubleClickCanExecute);
            RemoveImageCommand = new RelayCommand(RemoveImageExecute, RemoveImageCanExecute);
            DialogCommandList = new RelayCommand(DialogExecuteList);
            DialogCommandImage = new RelayCommand(DialogExecuteImage);
            ImageList = new ObservableCollection<ObservableCollection<Image>>();

            _aggregator.GetEvent<ClearEvent>().Subscribe(Clear);
            _aggregator.GetEvent<SendImage>().Subscribe(item => {
                if (ImageList.Contains(item) == false)
                    ImageList.Add(item);
            });
            _aggregator.GetEvent<SendImageList>().Subscribe( item => 
            {
                ImageList = item;
            });
        }

        private void DialogExecuteList(object obj)
        {
            Task.Run(() => DialogMethodAddListImage());
        }

        private void DialogExecuteImage(object obj)
        {
            Task.Run(() => DialogMethodAddImage());
        }


        public void DialogMethodAddListImage()
        {
            ObservableCollection<Image> list = new ObservableCollection<Image>();
            FileDialogMethod fdm = new FileDialogMethod();
            fdm.ReturnFilesFromDialog(list);
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (list.Count != 0)
            {
                ImageList.Add(list);
                _aggregator.GetEvent<FileDialogEvent>().Publish(ImageList);
                }
            }));
        }

        public void DialogMethodAddImage()
        {
            ObservableCollection<Image> list = new ObservableCollection<Image>();
            FileDialogMethod fdm = new FileDialogMethod();
            fdm.ReturnFilesFromDialog(list);
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (list.Count != 0)
                {
                    foreach(var image in list)
                    {
                        var temp = new ObservableCollection<Image>();
                        temp.Add(image);

                        ImageList.Add(temp);
                    }

                    _aggregator.GetEvent<FileDialogEvent>().Publish(ImageList);

                }
            }));
        }


        private void RemoveImageExecute(object obj)
        {
            var image = (ObservableCollection<Image>)obj;
            if (image != null)
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ImageList.Remove(image);
                    _aggregator.GetEvent<SendImageList>().Publish(ImageList);
                }));
            }
        }

        private bool RemoveImageCanExecute(object obj)
        {
            return true;
        }

        private void DoubleClickExecute(object obj)
        {
            ObservableCollection<Image> image = (ObservableCollection<Image>)obj;
            if (image != null)
            {
                DisplayImageWindow displayImageWindow = DisplayImageWindow.Instance;
                displayImageWindow.Show();
                _aggregator.GetEvent<DisplayImage>().Publish(image);
            }

        }

        private bool DoubleClickCanExecute(object obj)
        {
            return true;
        }

        public void Clear()
        {
            Task.Run(() => ClearAll());
        }

        private void ClearAll()
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                ImageList.Clear();
            }));
        }
    }
}
