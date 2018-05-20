using ImageViewer.Model.Event;
using ImageViewer.Methods;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ImageViewer.Model
{
    class CreateRegion : ITool
    {
        public void AffectImage(Dictionary<String, Object> args)
        {
            try
            {
                IEventAggregator aggregator = GlobalEvent.GetEventAggregator();
                BitmapWorker bw = new BitmapWorker();
                BitmapSource bitmapSource = (BitmapSource)args["BitmapSource"];
                System.Windows.Point regionLocation = (System.Windows.Point)args["RegionLocation"];
                int regionWidth = (int)((int)args["RegionWidth"] * bitmapSource.DpiX / 96.0);
                int regionHeight = (int)((int)args["RegionHeight"] * bitmapSource.DpiY / 96.0);
                if (regionWidth <= 0 || regionHeight <= 0)
                    return;
                Thickness imagePosition = (Thickness)args["ImagePosition"];
                int ID = (int)args["PresenterID"];
                double scale = (double)args["Scale"];

                int imagePosX = (int)(imagePosition.Left * bitmapSource.DpiX / 96.0);
                int imagePosY = (int)(imagePosition.Top * bitmapSource.DpiY / 96.0);
                int posX = (int)(regionLocation.X * bitmapSource.DpiX / 96.0);
                int posY = (int)(regionLocation.Y * bitmapSource.DpiY / 96.0);
                if (regionWidth <= 0 || regionHeight <= 0)
                    return;

                Bitmap bitmap;
                bitmap = bw.GetBitmap(bitmapSource);
                bool isOutside = false;
                bool isTooSmall = false;
                bitmap = bw.GetBitmapFragment(bitmap, posX, posY, regionWidth, regionHeight, imagePosX, imagePosY, scale, out isOutside, out isTooSmall);

                if(isTooSmall)
                {
                    MessageBox.Show("Witdth or height of a region is less than 1 pixel.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    aggregator.GetEvent<ResetRegionsEvent>().Publish();
                    return;
                }

                bitmap.Save(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ImageViewer\temp" + ID.ToString() + ".png", ImageFormat.Png);
                bitmapSource = bw.BitmapToSource(bitmap);
                int stride;
                int size;
                stride = (bitmapSource.PixelWidth * bitmapSource.Format.BitsPerPixel + 7) / 8;
                size = bitmapSource.PixelHeight * stride;
                byte[] pixels = new byte[size];
                bitmapSource.CopyPixels(pixels, stride, 0);

                List<byte> alphas = new List<byte>();
                List<byte> reds = new List<byte>();
                List<byte> greens = new List<byte>();
                List<byte> blues = new List<byte>();

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    alphas.Add(pixels[i + 3]);
                    reds.Add(pixels[i + 2]);
                    greens.Add(pixels[i + 1]);
                    blues.Add(pixels[i]);
                }
                double[] averages = new double[4];
                averages[0] = 0;
                averages[1] = 0;
                averages[2] = 0;
                averages[3] = 0;
                for (int i = 0; i < alphas.Count; i++)
                {
                    averages[0] += reds[i];
                    averages[1] += greens[i];
                    averages[2] += blues[i];
                    averages[3] += alphas[i];
                }
                averages[0] /= reds.Count;
                averages[1] /= greens.Count;
                averages[2] /= blues.Count;
                averages[3] /= alphas.Count;

                byte[] mins = new byte[4];
                mins[0] = reds.Min();
                mins[1] = greens.Min();
                mins[2] = blues.Min();
                mins[3] = alphas.Min();

                byte[] maxs = new byte[4];
                maxs[0] = reds.Max();
                maxs[1] = greens.Max();
                maxs[2] = blues.Max();
                maxs[3] = alphas.Max();

                double[] variances = new double[4];
                double[] deviations = new double[4];

                GetVarianceAndDeviation(ref variances, ref deviations, averages, reds, greens, blues, alphas);

                Dictionary<string, Object> regionInformation = new Dictionary<string, Object>();
                regionInformation.Add("Averages", averages);
                regionInformation.Add("Mins", mins);
                regionInformation.Add("Maxs", maxs);
                regionInformation.Add("Width", (int)(bitmap.Width));
                regionInformation.Add("Height", (int)(bitmap.Height));
                regionInformation.Add("Variances", variances);
                regionInformation.Add("Deviations", deviations);
                regionInformation.Add("PresenterID", ID);

                aggregator.GetEvent<SendRegionInformationEvent>().Publish(regionInformation);

            }
            catch (KeyNotFoundException)
            {

            }
        }
        private void GetVarianceAndDeviation(ref double[] variances, ref double[] deviations, double[] averages, List<byte> reds, List<byte> greens, List<byte> blues, List<byte> alphas)
        {
            double sum;
            //Red
            sum = 0.0;
            for (int i = 0; i < reds.Count; i++)
            {
                sum += Math.Pow(reds[i] - averages[0], 2);
            }
            variances[0] = sum / reds.Count;
            deviations[0] = Math.Sqrt(variances[0]);
            //Green
            sum = 0.0;
            for (int i = 0; i < greens.Count; i++)
            {
                sum += Math.Pow(greens[i] - averages[1], 2);
            }
            variances[1] = sum / greens.Count;
            deviations[1] = Math.Sqrt(variances[1]);
            //Blue
            sum = 0.0;
            for (int i = 0; i < blues.Count; i++)
            {
                sum += Math.Pow(blues[i] - averages[2], 2);
            }
            variances[2] = sum / blues.Count;
            deviations[2] = Math.Sqrt(variances[2]);
            //Alpha
            sum = 0.0;
            for (int i = 0; i < alphas.Count; i++)
            {
                sum += Math.Pow(alphas[i] - averages[3], 2);
            }
            variances[3] = sum / alphas.Count;
            deviations[3] = Math.Sqrt(variances[3]);

        }

        public Tools GetToolEnum()
        {
            return Tools.RegionSelection;
        }
    }
}
