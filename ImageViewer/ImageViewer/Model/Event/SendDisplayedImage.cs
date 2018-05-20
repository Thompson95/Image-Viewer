using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageViewer.Model.Event
{
    class SendDisplayedImage : PubSubEvent<SendDisplayedImage>
    {
        public Image Image { get; set; }
        public int PresenterID { get; set; }
        public bool IsSynchronized { get; set; }
        public bool DoProcessing { get; set; }
        public bool DoReset { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
    }
}
