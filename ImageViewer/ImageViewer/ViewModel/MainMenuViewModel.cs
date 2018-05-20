using ImageViewer.Methods;
using ImageViewer.Model;
using ImageViewer.Model.Event;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.ViewModel
{
    public class MainMenuViewModel : BaseViewModel
    {
        public RelayCommand DialogCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }

        public MainMenuViewModel()
        {
            DialogCommand = new RelayCommand(DialogExecute, DialogCanExecute);
            ClearCommand = new RelayCommand(ClearExecute, ClearCanExecute);
        }

        private void ClearExecute(object obj)
        {
            _aggregator.GetEvent<ClearEvent>().Publish();
        }
    
        private bool ClearCanExecute(object obj)
        {
            return true;
        }

        private void DialogExecute(object obj)
        {
            Task.Run(() => DialogMethod());
        }

        public void DialogMethod()
        {
            ObservableCollection<Image> list = new ObservableCollection<Image>();
            FileDialogMethod fdm = new FileDialogMethod();
            fdm.ReturnFilesFromDialog(list);
            if(list.Count != 0)
            {
                ObservableCollection<ObservableCollection<Image>> imageList = new ObservableCollection<ObservableCollection<Image>>();
                imageList.Add(list);
                _aggregator.GetEvent<FileDialogEvent>().Publish(imageList);
            }
        }

        private bool DialogCanExecute(object obj)
        {
            return true;
        }
    }
}
