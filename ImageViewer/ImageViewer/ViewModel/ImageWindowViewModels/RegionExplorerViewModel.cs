using ImageViewer.Methods;
using ImageViewer.Model;
using ImageViewer.Model.Event;
using ImageViewer.View;
using ImageViewer.View.ImagesWindow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace ImageViewer.ViewModel.ImageWindowViewModels
{
    class RegionExplorerViewModel : BaseViewModel
    {
        private ObservableCollection<Region> _regionList = new ObservableCollection<Region>();
        public RelayCommand RemoveRegionCommand { get; set; }
        public RelayCommand DoubleClickCommand { get; set; }
        public ObservableCollection<Region> RegionList
        {
            get
            {
                return _regionList;
            }
            set
            {
                _regionList = value;
                NotifyPropertyChanged();
            }
        }

        public RegionExplorerViewModel()
        {
            DoubleClickCommand = new RelayCommand(DoubleClickExecute);
            RemoveRegionCommand = new RelayCommand(RemoveRegion);
            _aggregator.GetEvent<SendRegionEvent>().Subscribe(AddRegion);
            _aggregator.GetEvent<SendImageList>().Subscribe(RemoveRegionBasingOnLoadedLists);
            _aggregator.GetEvent<DisposeEvent>().Subscribe(ClearList);
        }
        private void ClearList()
        {
            for (int i = _regionList.Count - 1; i >= 0; i--)
            {
                _regionList[i] = null;
                _regionList.Remove(_regionList[i]);
            }
            foreach (var file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ImageViewer\Regions"))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception)
                {

                }
            }
        }
        private void RemoveRegionBasingOnLoadedLists(ObservableCollection<ObservableCollection<Image>> imageList)
        {
            var list  = _regionList.Where(x => imageList.Contains(x.ImageList));
            var regionList = new ObservableCollection<Region>();
            foreach (var x in list)
            {
                regionList.Add(x);
            }
            RegionList = regionList;
        }
        private void AddRegion(Region region)
        {
            if (_regionList.Any(x => x.Image.FileName == region.Image.FileName))
            {
                MessageBox.Show("A region with this name already exists. Use a different name or delete the existing region to save it.", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                SaveRegionWindow.Instance.Close();
                return;
            }
            else
            {
                if (region.Save())
                {
                    RegionList.Add(region);
                    NotifyPropertyChanged("RegionList");
                    SaveRegionWindow.Instance.Close();
                }
            }
        }
        private void RemoveRegion(Object obj)
        {
            var region = (Region)obj;
            if (region != null)
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    RegionList.Remove(region);
                }));
            }
        }
        private void DoubleClickExecute(object obj)
        {
            Region region = (Region)obj;
            if (region != null)
            {
                _aggregator.GetEvent<LoadRegionEvent>().Publish(region);
            }

        }
    }
}
