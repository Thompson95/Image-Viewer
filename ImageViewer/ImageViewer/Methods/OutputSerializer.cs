using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageViewer.Model;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;

namespace ImageViewer.Methods
{
    public class OutputSerializer
    {
        public OutputSerializer() { }

        public void SerializeList(ObservableCollection<ImageViewer.Model.Image> list, int regionWidth, int regionHeight, Thickness regionPosition, double scale)
        {

            if (regionWidth > 0 && regionHeight > 0)
            {
                int counter = 0;
                bool isWarned = false;
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.Cancel || result == System.Windows.Forms.DialogResult.None)
                        return;
                    BitmapWorker bw = new BitmapWorker();
                    foreach (Model.Image image in list)
                    {
                        bool isOutside;
                        BitmapSource bitmapSource = image.Bitmap;
                        Bitmap bitmap;
                        int width = regionWidth;
                        int height = regionHeight;
                        Thickness position = regionPosition;
                        Normalize(ref width, ref height, ref position, bitmapSource);
                        bitmap = bw.GetBitmap(bitmapSource);
                        bitmap = bw.GetBitmapFragment(bitmap, (int)position.Left, (int)position.Top, (int)width, (int)height, (int)(image.Position.Left * bitmapSource.DpiX / 96.0), (int)(image.Position.Top * bitmapSource.DpiY / 96.0), scale, out isOutside);
                        String fileName = $"Out_{++counter}.png";
                        String path = dialog.SelectedPath + $"\\{fileName}";
                        if (isWarned == false && isOutside)
                            isWarned = true;
                        if (isOutside)
                            continue;
                        if (File.Exists(path))
                        {
                            MessageBoxResult overwriteResult = MessageBox.Show($"{fileName} already exists in this location. Do you want to overwrite it?", "Confirmation", MessageBoxButton.YesNoCancel);
                            if (overwriteResult == MessageBoxResult.Yes)
                            {
                                bitmap.Save(path, ImageFormat.Png);
                            }
                            else if (overwriteResult == MessageBoxResult.No)
                            {
                                continue;
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            bitmap.Save(path, ImageFormat.Png);
                        }
                    }
                }
                if (isWarned)
                    MessageBox.Show("Region exceeds size of one or more images or at least one image's pixel format is less than 24bits. Those images will be ignored or their size will be reduced.");
            }
            else
            {
                MessageBox.Show("Region not selected.");
            }
        }
        public void SaveByRegion(Model.Image image, int id, int regionWidth, int regionHeight, Thickness regionPosition, string path, double scale)
        {

            if (regionWidth > 0 && regionHeight > 0)
            {
                BitmapWorker bw = new BitmapWorker();
                BitmapSource bitmapSource = image.Bitmap;
                Bitmap bitmap;
                int width = regionWidth;
                int height = regionHeight;
                Thickness position = regionPosition;
                Normalize(ref width, ref height, ref position, bitmapSource);
                bitmap = bw.GetBitmap(bitmapSource);
                bitmap = bw.GetBitmapFragment(bitmap, (int)position.Left, (int)position.Top, (int)width, (int)height, (int)(image.Position.Left * bitmapSource.DpiX / 96.0), (int)(image.Position.Top * bitmapSource.DpiY / 96.0), scale);
                String fileName = $"Out_{id}.png";
                path += $"\\{fileName}";
                if (File.Exists(path))
                {
                    MessageBoxResult overwriteResult = MessageBox.Show($"{fileName} already exists in this location. Do you want to overwrite it?", "Confirmation", MessageBoxButton.YesNoCancel);
                    if (overwriteResult == MessageBoxResult.Yes)
                        bitmap.Save(path, ImageFormat.Png);
                    else
                        return;
                }
                else
                {
                    bitmap.Save(path, ImageFormat.Png);
                }
            }
        }
        private void Normalize(ref int width, ref int height, ref Thickness position, BitmapSource source)
        {
            position.Left = position.Left * source.DpiX / 96.0;
            position.Top = position.Top * source.DpiY / 96.0;
            width = (int)(width * source.DpiX / 96.0);
            height = (int)(height * source.DpiY / 96.0);
        }
    }
}
