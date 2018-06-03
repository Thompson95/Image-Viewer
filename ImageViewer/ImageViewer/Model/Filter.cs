using ImageViewer.Methods;
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
            None, Brightness, Contrast, Sepia, Negative, GrayScale
        } 

        public static BitmapSource Negative(BitmapSource source)
        {
            AsmProxy proxy = new AsmProxy();
            int size, stride;
            byte[] pixels = new BitmapWorker().GetByteArray(source, out size, out stride);
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
            AsmProxy proxy = new AsmProxy();
            int size, stride;
            byte[] pixels = new BitmapWorker().GetByteArray(source, out size, out stride);
            unsafe
            {
                fixed (byte* array = pixels)
                {
                    proxy.executeAsmBrightnessFilter(array, size, value);
                }

            }
            BitmapSource result = BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format, source.Palette, pixels, stride);
            return result;
        }
        public static BitmapSource GrayScale(BitmapSource source)
        {
            return source;
        }
        public static BitmapSource Contrast(BitmapSource source, Byte value)
        {
            AsmProxy proxy = new AsmProxy();
            int size, stride;
            int c = value - 128;
            float factor = (float)(259 * (c + 255)) / (float)(255 * (259 - c));
            Debug.WriteLine($"\nFactor is ---- {factor} ----\n");
            byte[] pixels = new BitmapWorker().GetByteArray(source, out size, out stride);
            float[] float_v = new float[pixels.Length];
            float[] factor_v = new float[8];
            for (int i = 0; i < pixels.Length; i++)
                float_v[i] = (float)pixels[i];
            for (int i = 0; i < 8; i++)
                factor_v[i] = factor;
            unsafe
            {
                fixed (float* array = float_v, coeff = factor_v)
                {
                    proxy.executeAsmContrastFilter(array, coeff, pixels.Length * 4);
                }

            }
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = (byte)float_v[i];
            BitmapSource result = BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format, source.Palette, pixels, stride);
            return result;
        }
    }
}
