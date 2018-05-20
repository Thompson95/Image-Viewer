using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageViewer.Model.Event
{
    public class ZoomEvent : PubSubEvent<ZoomEvent>
    {
        public bool ZoomReset { get; set; }
        public int ViewModelID { get; set; }
        public double ScaleDelta { get; set; }
        public double MouseX { get; set; }
        public double MouseY { get; set; }
        public ZoomEvent()
        {

        }
        public ZoomEvent(bool zoomReset, int viewModelID, double scaleDelta, double x, double y)
        {
            ZoomReset = zoomReset;
            ViewModelID = viewModelID;
            ScaleDelta = scaleDelta;
            MouseX = x;
            MouseY = y;
        }
    }
}
