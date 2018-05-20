using ImageViewer.Methods;
using ImageViewer.Model.Event;
using ImageViewer.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageViewer.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public RelayCommand ShowExplorerCommand { get; set; }
        private Visibility fileExplorerVisibility;
        public Visibility FileExplorerVisibility {
            get
            {
                return fileExplorerVisibility;
            }
            set
            {
                fileExplorerVisibility = value;
                NotifyPropertyChanged();
            }
        }
        public MainViewModel()
        {
            ShowExplorerCommand = new RelayCommand(ShowExplorer);
            _aggregator.GetEvent<CollapseEvent>().Subscribe(Collapse);
        }

        private void Collapse()
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                FileExplorerVisibility = Visibility.Collapsed;
            }));
        }

        private void ShowExplorer(object obj)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                FileExplorerVisibility = Visibility.Visible;
            }));
        }
    }
}
