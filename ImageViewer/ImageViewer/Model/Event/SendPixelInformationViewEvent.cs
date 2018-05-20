using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageViewer.View.ImagesWindow;

namespace ImageViewer.Model.Event
{
    public class SendPixelInformationViewEvent : PubSubEvent<PixelInformationView>
    {
    }
}
