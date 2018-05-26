using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            AsmProxy proxy = new AsmProxy();
            int stride = (int)(source.PixelWidth * 4);
            int size = (int)(source.PixelHeight * stride);
            byte[] pixels = new byte[size];
            source.CopyPixels(pixels, stride, 0);
            unsafe
            {
                fixed(byte* array = pixels)
                {
                    proxy.executeAsmNegativeFilter(array, size);
                }
                
            }
            BitmapSource result = BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format, source.Palette, pixels, stride);
            return result;
        }

        public static BitmapSource Sepia(BitmapSource source, Byte value)
        {
            return source;
        }
        public static BitmapSource Brightness(BitmapSource source, Byte value)
        {
            return source;
        }
        public static BitmapSource GreyScale(BitmapSource source)
        {
            return source;
        }
        public static BitmapSource Contrast(BitmapSource source, Byte value)
        {
            return source;
        }
    }
}
