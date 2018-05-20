using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.Model.Event
{
    public class NextPreviousImageEvent : PubSubEvent<NextPreviousImageEvent>
    {
        public bool ToNext { get; set; }
        public int PresenterID { get; set; }

        public NextPreviousImageEvent()
        {

        }
        public NextPreviousImageEvent(bool toNext, int id)
        {
            ToNext = toNext;
            PresenterID = id;
        }
    }
}
