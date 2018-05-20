using ImageViewer.Methods;
using ImageViewer.Model.Event;
using ImageViewer.View;
using ImageViewer.View.ImagesWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageViewer.ViewModel.ImageWindowViewModels
{
    public class SaveRegionWindowViewModel : BaseViewModel
    {
        private string _name = String.Empty;
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public SaveRegionWindowViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Save(Object obj)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (Name == String.Empty)
                {
                    MessageBox.Show("The name must not be empty!");
                    return;
                }
                _aggregator.GetEvent<SendRegionNameEvent>().Publish(Name);
            }));
            
        }
        private void Cancel(Object obj)
        {
            SaveRegionWindow.Instance.Close();
        }
    }
}
