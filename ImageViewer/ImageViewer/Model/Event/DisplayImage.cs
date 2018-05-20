using Prism.Events;
using System.Collections.ObjectModel;

namespace ImageViewer.Model.Event
{
    public class DisplayImage : PubSubEvent<ObservableCollection<Image>>
    {
    }
}
