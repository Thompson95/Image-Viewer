using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ImageViewer.Methods;
using System.Runtime.InteropServices;
using Prism.Events;
using ImageViewer.Model.Event;
using System.Drawing.Imaging;
using System.IO;

namespace ImageViewer.Model
{
    class PanImage : ITool
    {
        public void AffectImage(Dictionary<String, Object> args)
        {
            try
            {
                Image image = (Image)args["DisplayedImage"];
                int mouseX = (int)((int)args["MouseX"]);
                int mouseY = (int)((int)args["MouseY"]);
                int mouseXDelta = (int)((int)args["MouseXDelta"]);
                int mouseYDelta = (int)((int)args["MouseYDelta"]);
                int mouseClickX = (int)((System.Windows.Point)args["MouseClickPosition"]).X;
                int mouseClickY = (int)((System.Windows.Point)args["MouseClickPosition"]).Y;
                int presenterID = (int)args["PresenterID"];
                Thickness imagePosition = (Thickness)args["Position"];
                int offsetX = mouseX - mouseClickX;
                int offsetY = mouseY - mouseClickY;
                imagePosition.Left = imagePosition.Left + offsetX - mouseXDelta;
                imagePosition.Right = -imagePosition.Left;
                imagePosition.Top = imagePosition.Top + offsetY - mouseYDelta;
                imagePosition.Bottom = -imagePosition.Top;
                image.Position = imagePosition;
                IEventAggregator aggregator = GlobalEvent.GetEventAggregator();
                SendDisplayedImage sdi = new SendDisplayedImage();
                sdi.Image = image;
                sdi.PresenterID = presenterID;
                sdi.IsSynchronized = (bool)args["IsSynchronized"];
                sdi.DoProcessing = (bool)args["DoProcessing"];
                sdi.OffsetX = offsetX - mouseXDelta;
                sdi.OffsetY = offsetY - mouseYDelta;
                sdi.DoReset = false;
                aggregator.GetEvent<SendDisplayedImage>().Publish(sdi);

            }
            catch (Exception)
            {
                return;
            }
        }
        public Tools GetToolEnum()
        {
            return Tools.ImagePan;
        }
    }
}
