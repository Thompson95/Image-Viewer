using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageViewer.Model.Event
{
    public class SynchronizeRegions : PubSubEvent<SynchronizeRegions>
    {
        public int PresenterID { get; set; }
        public Thickness Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool DoProcessing { get; set; }
        public double Zoom { get; set; }

        public SynchronizeRegions()
        {
            Zoom = -1;
        }
    }
}
