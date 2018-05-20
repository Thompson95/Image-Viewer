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
                if(_filePath!= null)
                {
                    OriginalBitmap = new BitmapImage(new Uri(_filePath));
                    Bitmap = OriginalBitmap.Clone();
                }
            }
        }
        public string Extension { get ; set; }
        public BitmapSource Bitmap { get; set; }
        public BitmapSource OriginalBitmap { get; set; }
        public Thickness Position { get; set; }
        public Rotation Rotation { get; set; }

        public Image()
        {
            Position = new Thickness(0, 0, 0, 0);
            Rotation = Rotation.Rotate0;
        }
    }
}
