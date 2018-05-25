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
    public class FilterControlViewModel : BaseViewModel
    {
        private short _filterValue = 0;
        private int _presenterID = 0;
        public Filter.Filters filter = Filter.Filters.None;
        public String FilterValue
        {
            get
            {
                return _filterValue.ToString();
            }
            set
            {
                _filterValue = Int16.Parse(value);
                NotifyPropertyChanged();
                //    FilterEvent fe = new FilterEvent();
                //    fe.PresenterID = PresenterID;
                //    fe.Filter = filter;
                //    fe.Value = _filterValue;
                //    _aggregator.GetEvent<FilterEvent>().Publish(fe);
            }
        }
        public int PresenterID { get; set; }
        public FilterControlViewModel()
        {
            //_aggregator.GetEvent<FilterEvent>().Subscribe((fe) =>
            //{
            //    filter = fe.Filter;
            //    PresenterID = fe.PresenterID;
            //    FilterValue = fe.Value.ToString();
            //});
        }
    }
}
