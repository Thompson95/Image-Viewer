using ImageViewer.ViewModel.ImageWindowViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageViewer.Model
{
    public class Image
    {
        private BitmapSource _bitmap;
        private BitmapSource _originalBitmap;
        private string _filePath = String.Empty;
        public string Extension { get; set; }
        public BitmapSource Bitmap
        {
            get
            {
                return _bitmap;
            }
            set
            {
                _bitmap = value;
            }
        }
        public BitmapSource OriginalBitmap
        {
            get
            {
                return _originalBitmap;
            }
            set
            {
                _originalBitmap = value;
            }
        }
        public ObservableCollection<FilterControlViewModel> FilterValues { get; set; }
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
                }
                FilterValues = new ObservableCollection<FilterControlViewModel>();
            }
        }

        public Image()
        {
            Position = new Thickness(0, 0, 0, 0);
            Rotation = Rotation.Rotate0;
        }

        public Image RefreshImageFilters()
        {
            using (var fileStream = new FileStream("C:\\Users\\taras\\Desktop\\filtered bitmaps\\bitmap.png", FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(Bitmap));
                encoder.Save(fileStream);
            }
            _bitmap = new Rotate().SingleBitmapRotation((int)this.Rotation * 90, this.OriginalBitmap);
            for (int i = 0; i < FilterValues.Count; i++)
            {
                var item = FilterValues[i];
                switch (item.Filter)
                {
                    case Filter.Filters.Brightness:
                        {
                            Bitmap = Filter.Brightness(_bitmap, item.Value);
                        }
                        break;
                    case Filter.Filters.Contrast:
                        {
                            Bitmap = Filter.Contrast(_bitmap, item.Value);
                        }
                        break;
                    case Filter.Filters.Sepia:
                        {
                            Bitmap = Filter.Sepia(_bitmap, item.Value);
                        }
                        break;
                    case Filter.Filters.Negative:
                        {
                            if (item.Value > 128)
                                Bitmap = Filter.Negative(_bitmap);
                            else
                                Bitmap = _bitmap;
                        }
                        break;
                    case Filter.Filters.GrayScale:
                        {
                            if (item.Value > 128)
                                Bitmap = Filter.GrayScale(_bitmap);
                            else
                                Bitmap = _bitmap;
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
