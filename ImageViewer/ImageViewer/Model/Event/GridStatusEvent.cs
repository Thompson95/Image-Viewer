using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.Model.Event
{
    public class GridStatusEvent : PubSubEvent<GridStatusEvent.GridStatus>
    {
        public enum GridStatus
        {
            OneToOne,
            OneToTwo,
            TwoToTwo,
            ThreeToThree,
        }
    }
}
