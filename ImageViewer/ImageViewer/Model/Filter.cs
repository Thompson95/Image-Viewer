using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageViewer.Model
{
    public static class Filter
    {
       public enum Filters
        {
            None, Brightness, Contrast, Sepia, Negative, GreyScale
        } 

        public static BitmapSource Negative(BitmapSource source)
        {
            int stride = (int)(source.PixelWidth * 4);
            int size = (int)(source.PixelHeight * stride);
            byte[] pixels = new byte[size];
            source.CopyPixels(pixels, stride, 0);
            for (int i = 0; i < size; i++)
            {
                if (i % 4 != 3)
                    pixels[i] =  (byte)(255 -pixels[i]);
            }
            BitmapSource result = BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format, source.Palette, pixels, stride);
            return result;
        }

        public static BitmapSource Sepia(BitmapSource source)
        {
            return source;
        }
        public static BitmapSource Brightness(BitmapSource source)
        {
            return source;
        }
        public static BitmapSource GreyScale(BitmapSource source)
        {
            return source;
        }
        public static BitmapSource Contrast(BitmapSource source)
        {
            return source;
        }
    }
}
