using Prism.Events;
using System.Collections.Generic;

namespace ImageViewer.Model.Event
{
    class RotateImageEvent : PubSubEvent<RotateImageEvent>
    {
        public Image Image { get; set; }
        public int PresenterID { get; set; }
        public List<int> SynchronizedPresenters { get; set; }
        public int Angle { get; set; }
    }
}
