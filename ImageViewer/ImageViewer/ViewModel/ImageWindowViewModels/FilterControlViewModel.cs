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
    public class FilterControlViewModel : BaseViewModel, IDisposable
    {
        private Dictionary<Filter.Filters, byte> filterValues;
        private byte _filterValue = 0;
        public Filter.Filters filter = Filter.Filters.None;
        public String FilterName
        {
            get
            {
                return filter.ToString();
            }
            set
            {
                NotifyPropertyChanged();
            }
        }
        public String FilterValue
        {
            get
            {
                return _filterValue.ToString();
            }
            set
            {
                _filterValue = (byte)double.Parse(value);
                filterValues[filter] = _filterValue;
                NotifyPropertyChanged();
                FilterEvent fe = new FilterEvent();
                fe.PresenterID = PresenterID;
                fe.Filter = filter;
                fe.Value = _filterValue;
                _aggregator.GetEvent<SendFilterValueEvent>().Publish(fe);
            }
        }
        public int PresenterID { get; set; }
        public FilterControlViewModel()
        {
            filterValues = new Dictionary<Filter.Filters, byte>();
            filterValues.Add(Filter.Filters.Brightness, 0);
            filterValues.Add(Filter.Filters.Contrast, 0);
            filterValues.Add(Filter.Filters.Sepia, 0);

            _aggregator.GetEvent<FilterEvent>().Subscribe((fe) =>
            {
                filter = fe.Filter;
                FilterName = filter.ToString();
                Debug.WriteLine(filter.ToString());
                FilterValue = filterValues[filter].ToString();
            PresenterID = fe.PresenterID;
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

        ~FilterControlViewModel()
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
