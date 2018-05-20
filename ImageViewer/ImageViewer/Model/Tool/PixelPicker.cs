using ImageViewer.Model.Event;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageViewer.Model
{
    class PixelPicker : ITool
    {
        public PixelPicker()
        {

        }
        public void AffectImage(Dictionary<String, Object> args)
        {
            BitmapSource bitmapSource = (BitmapSource)args["BitmapSource"];
            

            int mouseX = (int)((int)args["MouseX"] * bitmapSource.DpiX / 96);
            int mouseY = (int)((int)args["MouseY"] * bitmapSource.DpiY / 96);
            double scale = (double)args["Scale"];
            Thickness imagePosition = (Thickness)args["ImagePosition"];

            var targetBitmap = new TransformedBitmap(bitmapSource, new ScaleTransform(scale, scale));
            bitmapSource = targetBitmap;

            try
            {
                int stride = (int) (bitmapSource.PixelWidth * 4);
                int size = (int) (bitmapSource.PixelHeight * stride);
                byte[] pixels = new byte[size];
                bitmapSource.CopyPixels(pixels, stride, 0);
                int row = mouseY - (int)(imagePosition.Top * bitmapSource.DpiY / 96);
                int column = mouseX - (int)(imagePosition.Left * bitmapSource.DpiX / 96);
                int index = row * stride + 4 * column;
                byte red;
                byte green;
                byte blue;
                byte alpha;

                Dictionary<string, int> pixelInformation = new Dictionary<string, int>();
                if (row >= bitmapSource.PixelHeight || row < 0 || column >= bitmapSource.PixelWidth || column < 0)
                {
                    red = 0;
                    green = 0;
                    blue = 0;
                    alpha = 0;
                    pixelInformation.Add("MouseX", -1);
                    pixelInformation.Add("MouseY", -1);
                }
                else
                {
                    red = pixels[index + 2];
                    green = pixels[index + 1];
                    blue = pixels[index];
                    alpha = pixels[index + 3];
                    pixelInformation.Add("MouseX", (int)((mouseX - (int)(imagePosition.Left * bitmapSource.DpiX / 96))/scale));
                    pixelInformation.Add("MouseY", (int)((mouseY - (int)(imagePosition.Top * bitmapSource.DpiY / 96))/scale));
                }
                pixelInformation.Add("Alpha", alpha);
                pixelInformation.Add("Red", red);
                pixelInformation.Add("Green", green);
                pixelInformation.Add("Blue", blue);

                IEventAggregator aggregator = GlobalEvent.GetEventAggregator();
                aggregator.GetEvent<SendPixelInformationEvent>().Publish(pixelInformation);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (IndexOutOfRangeException)
            {

            }
        }

        public Tools GetToolEnum()
        {
            return Tools.PixelInformations;
        }
    }
}
