using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.ObjectModel;
using ImageViewer.View;
using System.Threading;

namespace ImageViewer.Model
{
    public class Region
    {
        public RegionImage Image { get; set; }
        public Thickness ImagePosition { get; set; }
        public Point Position { get; set; }
        public Size Size { get; set; }
        public int DpiX { get; set;}
        public int DpiY { get; set; }
        public ObservableCollection<Image> ImageList { get; set; }
        public Image AttachedImage { get; set; }
        public int PresenterID { get; set; }
        public int ImageIndex { get; set; }
        public double Zoom { get; set; }

        public Region(Point position, Size size, string name, Vector Dpi, ObservableCollection<Image> imageList, Image image, int id, int index, double zoom) 
        {
            Position = position;
            Size = size;
            Image = new RegionImage();
            Image = new RegionImage(name, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ImageViewer\Regions\" + name + ".png", ".png");
            DpiX = (int)Dpi.X;
            DpiY = (int)Dpi.Y;
            ImageList = imageList;
            AttachedImage = image;
            PresenterID = id;
            ImagePosition = AttachedImage.Position;
            ImageIndex = index;
            Zoom = zoom;
        }

        public bool Save()
        {
            try
            {
                if(File.Exists(Image.FilePath))
                {
                    File.Delete(Image.FilePath);
                }

                if(!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ImageViewer\Regions"))
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ImageViewer\Regions");
                }
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ImageViewer\temp" + PresenterID.ToString() + ".png");
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.None;
                bitmap.EndInit();
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                using (var fileStream = new System.IO.FileStream(Image.FilePath, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                    fileStream.Close();
                }
                return true;
            }
            catch(Exception)
            {
                MessageBox.Show("Save failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public void Dispose()
        {
        }
    }
}
