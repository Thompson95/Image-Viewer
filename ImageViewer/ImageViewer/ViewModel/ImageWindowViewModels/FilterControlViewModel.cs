using ImageViewer.Methods;
using ImageViewer.Model;
using ImageViewer.Model.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.ViewModel.ImageWindowViewModels
{
    public class FilterControlViewModel : BaseViewModel
    {
        public RelayCommand RemoveFilterCommand { get; set; }

        private Filter.Filters _filter = Model.Filter.Filters.Contrast;
        private byte _value = 0;
        public int PresenterID { get; set; }
        public Filter.Filters Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("FilterName");
            }
        }
        public byte Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("UIValue");
            }
        }
        public byte UIValue
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                NotifyPropertyChanged();
                FilterEvent fe = new FilterEvent();
                fe.Value = Value;
                fe.Filter = Filter;
                fe.PresenterID = PresenterID;
                fe.FilterControlVM = this;
                _aggregator.GetEvent<SendFilterValueEvent>().Publish(fe);
            }
        }
        public String FilterName
        {
            get
            {
                return _filter.ToString();
            }
        }
        public FilterControlViewModel()
        {
            RemoveFilterCommand = new RelayCommand(RemoveFilterExecute);
        }

        private void RemoveFilterExecute(Object obj)
        {
            _aggregator.GetEvent<RemoveFilterEvent>().Publish(this);
        }
    }
}
