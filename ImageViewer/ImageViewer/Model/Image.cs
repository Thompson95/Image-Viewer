using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageViewer.Model
{
    public class Image
    {
        private string _filePath = String.Empty;
        public string Extension { get; set; }
        public BitmapSource Bitmap { get; set; }
        public BitmapSource OriginalBitmap { get; set; }
        public BitmapSource FilteredBitmap { get; set; }
        public List<Tuple<Filter.Filters, byte>> FilterValues { get; set; }
        public Thickness Position { get; set; }
        public Rotation Rotation { get; set; }
        public string FileName { get; set; }
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                if (_filePath != null)
                {
                    OriginalBitmap = new BitmapImage(new Uri(_filePath));
                    Bitmap = OriginalBitmap.Clone();
                    FilteredBitmap = OriginalBitmap.Clone();
                }
                FilterValues = new List<Tuple<Filter.Filters, byte>>();
            }
        }

        public Image()
        {
            Position = new Thickness(0, 0, 0, 0);
            Rotation = Rotation.Rotate0;
        }

        public Image RefreshImageFilters()
        {
            for (int i = 0; i < FilterValues.Count; i++)
            {
                var item = FilterValues[i];
                BitmapSource tempBitmap = OriginalBitmap;
                if (i > 0)
                {
                    tempBitmap = Bitmap;
                }
                switch (item.Item1)
                {
                    case Filter.Filters.Brightness:
                        {
                            Bitmap = Filter.Brightness(tempBitmap, item.Item2);
                        }
                        break;
                    case Filter.Filters.Contrast:
                        {
                            Bitmap = Filter.Contrast(tempBitmap, item.Item2);
                        }
                        break;
                    case Filter.Filters.Sepia:
                        {
                            Bitmap = Filter.Sepia(tempBitmap, item.Item2);
                        }
                        break;
                    case Filter.Filters.Negative:
                        {
                            if (item.Item2 > 0)
                                Bitmap = Filter.Negative(tempBitmap);
                        }
                        break;
                    case Filter.Filters.GrayScale:
                        {
                            if (item.Item2 > 0)
                                Bitmap = Filter.GrayScale(tempBitmap);
                        }
                        break;
                    default:
                        break;
                }
            }
            return this;
        }
    }
}
