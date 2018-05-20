using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.Model.Event
{
    public class FileDialogEvent : PubSubEvent<ObservableCollection<ObservableCollection<Image>>>
    {

    }
}
