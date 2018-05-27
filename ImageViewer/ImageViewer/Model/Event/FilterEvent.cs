using ImageViewer.ViewModel.ImageWindowViewModels;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ImageViewer.Model.Event
{
    public class FilterEvent : PubSubEvent<FilterEvent>
    {
        public int PresenterID { get; set; }
        public Filter.Filters Filter { get; set; }
        public byte Value { get; set; }
        public FilterControlViewModel FilterControlVM { get; set; }
    }
}
