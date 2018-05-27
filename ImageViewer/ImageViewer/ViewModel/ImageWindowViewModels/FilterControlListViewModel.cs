using ImageViewer.Methods;
using ImageViewer.Model;
using ImageViewer.Model.Event;
using ImageViewer.View;
using ImageViewer.View.ImagesWindow;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageViewer.ViewModel.ImageWindowViewModels
{
    public class FilterControlListViewModel : BaseViewModel, IDisposable
    {
        private static FilterControlListViewModel _instance;
        public static FilterControlListViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FilterControlListViewModel();
                }
                return _instance;
            }
        }
        private ObservableCollection<FilterControlViewModel> _filterList;

        public ObservableCollection<FilterControlViewModel> FilterList
        {
            get
            {
                return _filterList;
            }
            set
            {
                //_filterList.Clear();
                _filterList = value;
                NotifyPropertyChanged();
            }
        }
        public int PresenterID { get; set; }
        public FilterControlListViewModel()
        {
            FilterList = new ObservableCollection<FilterControlViewModel>();
            _aggregator.GetEvent<SendFilterListEvent>().Subscribe(sfl =>
           {
               FilterList = sfl.FilterList;
           });
        }

        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                handle.Dispose();
            }
        }

        ~FilterControlListViewModel()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
