using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.Model.Event
{
    public class SynchronizeImagePositions : PubSubEvent<Dictionary<String, Object>>
    {
    }
}
