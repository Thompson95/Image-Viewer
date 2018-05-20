using ImageViewer.Methods;
using ImageViewer.Model.Event;
using ImageViewer.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.ViewModel
{
    public class FileExplorerViewModel : BaseViewModel
    {
        public RelayCommand CollapseAllCommand { get; set; }
        public RelayCommand CollapseCommand { get; set; }
        public FileExplorerViewModel()
        {
            CollapseCommand = new RelayCommand(CollapseExecute, CollapseCanExecute);
            CollapseAllCommand = new RelayCommand(CollapseAll);
        }
        private void CollapseAll(object obj)
        {
            _aggregator.GetEvent<CollapseAllEvent>().Publish();
        }
        private void CollapseExecute(object obj)
        {
            _aggregator.GetEvent<CollapseEvent>().Publish();
            //Task.Run(() => CollapseMethod());
        }

        private bool CollapseCanExecute(object obj)
        {
            return true;
        }
    }
}
