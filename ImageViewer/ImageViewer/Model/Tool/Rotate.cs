using ImageViewer.Model.Event;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace ImageViewer.Model
{
    public class Rotate : ITool
    {

        public Rotate()
        {

        }

        public BitmapSource SingleBitmapRotation(int angle, BitmapSource bitmap )
        {
            var transformedBitmap = new TransformedBitmap();
            transformedBitmap.BeginInit();
            transformedBitmap.Source = bitmap;
            transformedBitmap.Transform = new RotateTransform(angle);
            transformedBitmap.EndInit();
            return transformedBitmap;
        }

        public void AffectImage(Dictionary<String, Object> args)
        {
            Image image = (Image)args["DisplayedImage"];
            int presenterID = (int)args["PresenterID"];
            int angle = (int)args["Angle"];
            List<int> presenters = (List<int>)args["SynchronizedPresenters"];
            image.Bitmap = SingleBitmapRotation(angle, image.Bitmap);
            image.Rotation = (Rotation)((int)image.Rotation + (int)(angle/90) % 4);
            RotateImageEvent ri = new RotateImageEvent();
            IEventAggregator aggregator = GlobalEvent.GetEventAggregator();
            presenters.Add(presenterID);
            ri.Image = image;
            ri.PresenterID = presenterID;
            ri.SynchronizedPresenters = presenters;
            ri.Angle = angle;
            aggregator.GetEvent<RotateImageEvent>().Publish(ri);

        }

        public Tools GetToolEnum()
        {
            return Tools.Rotate;
        }
    }
}
