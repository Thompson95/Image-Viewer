using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageViewer.Methods
{
    class BitmapWorker
    {
        public System.Drawing.Bitmap GetBitmap(BitmapSource src)
        {
            try
            {
                MemoryStream TransportStream = new MemoryStream();
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(src));
                enc.Save(TransportStream);
                Bitmap bmp = new Bitmap(TransportStream);
                TransportStream.Close();
                TransportStream.Dispose();
                return bmp;
            }
            catch
            {
                MessageBox.Show("failed");
                return null;
            }
        }
        public Bitmap GetBitmapFragment(Bitmap bmp1, int posX, int posY, int width, int height, int offsetX, int offsetY, double scale)
        {
            try
            {
                System.Drawing.Size s1 = bmp1.Size;
                PixelFormat fmt1 = bmp1.PixelFormat;

                PixelFormat fmt = PixelFormat.Format32bppArgb;
                Bitmap bmp2 = new Bitmap((int)(width / scale), (int)(height / scale), fmt);
                posX = posX - offsetX;
                posY = posY - offsetY;

                if (posX >= bmp1.Width * scale || posY >= bmp1.Height * scale || posX + width <= 0 || posY + height <= 0)
                    return bmp2;
                BitmapData bmp2Data = bmp2.LockBits(new Rectangle(0, 0, (int)(width / scale), (int)(height / scale)), ImageLockMode.ReadWrite, fmt);
                int startX = (int)(posX / scale);
                int startY = (int)(posY / scale);
                width = (int)(width / scale);
                height = (int)(height / scale);
                int endX = startX + width;
                int endY = startY + height;
                //endX = (int)(endX / scale);
                if (posX < 0)
                {
                    startX = 0;
                }
                if (posY < 0)
                {
                    startY = 0;
                }
                if (endX > bmp1.Width)
                    endX = bmp1.Width;
                if (endY > bmp1.Height)
                    endY = bmp1.Height;


                //if (startX + width > bmp1.Width)
                {
                    width = endX - startX;
                }
                //if (startY + height > bmp1.Height)
                {
                    height = endY - startY;
                }

                Rectangle rect = new Rectangle(startX, startY, width, height);
                BitmapData bmp1Data = bmp1.LockBits(rect, ImageLockMode.ReadOnly, fmt1);

                byte bpp1 = 4;
                byte bpp3 = 4;

                if (fmt1 == PixelFormat.Format24bppRgb) bpp1 = 3;
                else if (fmt1 == PixelFormat.Format32bppPArgb
                        || fmt1 == PixelFormat.Format32bppArgb
                        || fmt1 == PixelFormat.Format32bppRgb)
                    bpp1 = 4;
                else return bmp2;

                //int size1 = bmp1Data.Stride * bmp1Data.Height;
                int size1 = (height - 1) * bmp1Data.Stride + width * bpp1;
                int size = bmp1Data.Stride * bmp1Data.Height;
                int size2 = bmp2Data.Stride * bmp2Data.Height;
                byte[] data1 = new byte[size1];
                byte[] data2 = new byte[size2];

                Marshal.Copy(bmp1Data.Scan0, data1, 0, size1);
                Marshal.Copy(bmp2Data.Scan0, data2, 0, size2);

                for (int y = 0; y < bmp2Data.Height; y++)
                {
                    for (int x = 0; x < bmp2Data.Width; x++)
                    {
                        int row = y + (posY > 0 ? 0 : posY);
                        int column = x + (posX > 0 ? 0 : posX);
                        int index3 = y * bmp2Data.Stride + x * bpp3;
                        if (row >= height || row < 0 || column >= width || column < 0)
                        {
                            continue;
                        }
                        else
                        {
                            int index1 = row * bmp1Data.Stride + column * bpp1;
                            System.Drawing.Color c1;

                            if (bpp1 == 4)
                                c1 = Color.FromArgb(data1[index1 + 3], data1[index1 + 2], data1[index1 + 1], data1[index1 + 0]);
                            else c1 = Color.FromArgb(255, data1[index1 + 2], data1[index1 + 1], data1[index1 + 0]);

                            if(c1.A == 0)
                            {
                                data2[index3 + 0] = 0;
                                data2[index3 + 1] = 0;
                                data2[index3 + 2] = 0;
                                data2[index3 + 3] = c1.A;
                            }
                            else
                            {
                                data2[index3 + 0] = c1.B;
                                data2[index3 + 1] = c1.G;
                                data2[index3 + 2] = c1.R;
                                data2[index3 + 3] = c1.A;
                            }
                        }
                    }
                }


                Marshal.Copy(data2, 0, bmp2Data.Scan0, data2.Length);
                bmp1.UnlockBits(bmp1Data);
                bmp2.UnlockBits(bmp2Data);
                return bmp2;
            }
            catch (Exception e)
            {
                var st = new StackTrace(e, true);
                var frame = st.GetFrame(0);
                var line = frame.GetFileLineNumber();
                throw;
            }
        }
        public Bitmap GetBitmapFragment(Bitmap bmp1, int posX, int posY, int width, int height, int offsetX, int offsetY, double scale, out bool isOutside, out bool isTooSmall)
        {
            try
            {
                System.Drawing.Size s1 = bmp1.Size;
                PixelFormat fmt1 = bmp1.PixelFormat;

                PixelFormat fmt = PixelFormat.Format32bppArgb;
                if ((int)(width / scale) <= 0 || (int)(height / scale) <= 0)
                {
                    isTooSmall = true;
                    isOutside = false;
                    return null;
                }
                Bitmap bmp2 = new Bitmap((int)(width / scale), (int)(height / scale), fmt);
                posX = posX - offsetX;
                posY = posY - offsetY;

                if (posX >= bmp1.Width * scale || posY >= bmp1.Height * scale || posX + width <= 0 || posY + height <= 0)
                {
                    isTooSmall = false;
                    isOutside = true;
                    return bmp2;
                }
                BitmapData bmp2Data = bmp2.LockBits(new Rectangle(0, 0, (int)(width / scale), (int)(height / scale)), ImageLockMode.ReadWrite, fmt);
                int startX = (int)(posX / scale);
                int startY = (int)(posY / scale);
                width = (int)(width / scale);
                height = (int)(height / scale);
                int endX = startX + width;
                int endY = startY + height;
                if (posX < 0)
                {
                    startX = 0;
                }
                if (posY < 0)
                {
                    startY = 0;
                }
                if (endX > bmp1.Width)
                    endX = bmp1.Width;
                if (endY > bmp1.Height)
                    endY = bmp1.Height;

                width = endX - startX;
                height = endY - startY;

                Rectangle rect = new Rectangle(startX, startY, width, height);
                BitmapData bmp1Data = bmp1.LockBits(rect, ImageLockMode.ReadOnly, fmt1);

                byte bpp1 = 4;
                byte bpp3 = 4;

                if (fmt1 == PixelFormat.Format24bppRgb) bpp1 = 3;
                else if (fmt1 == PixelFormat.Format32bppPArgb
                        || fmt1 == PixelFormat.Format32bppArgb
                        || fmt1 == PixelFormat.Format32bppRgb)
                    bpp1 = 4;
                else
                {
                    isTooSmall = false;
                    isOutside = true;
                    return bmp2;
                }

                int size1 = (height - 1) * bmp1Data.Stride + width * bpp1;
                int size = bmp1Data.Stride * bmp1Data.Height;
                int size2 = bmp2Data.Stride * bmp2Data.Height;
                byte[] data1 = new byte[size1];
                byte[] data2 = new byte[size2];

                Marshal.Copy(bmp1Data.Scan0, data1, 0, size1);
                Marshal.Copy(bmp2Data.Scan0, data2, 0, size2);

                for (int y = 0; y < bmp2Data.Height; y++)
                {
                    for (int x = 0; x < bmp2Data.Width; x++)
                    {
                        int row = y + (posY > 0 ? 0 : posY);
                        int column = x + (posX > 0 ? 0 : posX);
                        int index3 = y * bmp2Data.Stride + x * bpp3;
                        if (row >= height || row < 0 || column >= width || column < 0)
                        {
                            continue;
                        }
                        else
                        {
                            int index1 = row * bmp1Data.Stride + column * bpp1;
                            System.Drawing.Color c1;

                            if (bpp1 == 4)
                                c1 = Color.FromArgb(data1[index1 + 3], data1[index1 + 2], data1[index1 + 1], data1[index1 + 0]);
                            else c1 = Color.FromArgb(255, data1[index1 + 2], data1[index1 + 1], data1[index1 + 0]);

                            data2[index3 + 0] = c1.B;
                            data2[index3 + 1] = c1.G;
                            data2[index3 + 2] = c1.R;
                            data2[index3 + 3] = c1.A;
                        }
                    }
                }


                Marshal.Copy(data2, 0, bmp2Data.Scan0, data2.Length);
                bmp1.UnlockBits(bmp1Data);
                bmp2.UnlockBits(bmp2Data);
                isTooSmall = false;
                isOutside = false;
                return bmp2;
            }
            catch (Exception e)
            {
                var st = new StackTrace(e, true);
                var frame = st.GetFrame(0);
                var line = frame.GetFileLineNumber();
                throw;
            }
        }
        public Bitmap GetBitmapFragment(Bitmap bmp1, int posX, int posY, int width, int height, int offsetX, int offsetY, double scale, out bool isOutside)
        {
            try
            {
                System.Drawing.Size s1 = bmp1.Size;
                PixelFormat fmt1 = bmp1.PixelFormat;

                PixelFormat fmt = PixelFormat.Format32bppArgb;
                if((int)(width / scale) <= 0 || (int)(height / scale) <= 0)
                {
                    isOutside = true;
                    return null;
                }
                Bitmap bmp2 = new Bitmap((int)(width / scale), (int)(height / scale), fmt);
                posX = posX - offsetX;
                posY = posY - offsetY;

                if (posX >= bmp1.Width * scale || posY >= bmp1.Height * scale || posX + width <= 0 || posY + height <= 0)
                {
                    isOutside = true;
                    return bmp2;
                }
                BitmapData bmp2Data = bmp2.LockBits(new Rectangle(0, 0, (int)(width / scale), (int)(height / scale)), ImageLockMode.ReadWrite, fmt);
                int startX = (int)(posX / scale);
                int startY = (int)(posY / scale);
                width = (int)(width / scale);
                height = (int)(height / scale);
                int endX = startX + width;
                int endY = startY + height;
                if (posX < 0)
                {
                    startX = 0;
                }
                if (posY < 0)
                {
                    startY = 0;
                }
                if (endX > bmp1.Width)
                    endX = bmp1.Width;
                if (endY > bmp1.Height)
                    endY = bmp1.Height;

                width = endX - startX;
                height = endY - startY;

                Rectangle rect = new Rectangle(startX, startY, width, height);
                BitmapData bmp1Data = bmp1.LockBits(rect, ImageLockMode.ReadOnly, fmt1);

                byte bpp1 = 4;
                byte bpp3 = 4;

                if (fmt1 == PixelFormat.Format24bppRgb) bpp1 = 3;
                else if (fmt1 == PixelFormat.Format32bppPArgb
                        || fmt1 == PixelFormat.Format32bppArgb
                        || fmt1 == PixelFormat.Format32bppRgb)
                    bpp1 = 4;
                else
                {
                    isOutside = true;
                    return bmp2;
                }

                int size1 = (height - 1) * bmp1Data.Stride + width * bpp1;
                int size = bmp1Data.Stride * bmp1Data.Height;
                int size2 = bmp2Data.Stride * bmp2Data.Height;
                byte[] data1 = new byte[size1];
                byte[] data2 = new byte[size2];

                Marshal.Copy(bmp1Data.Scan0, data1, 0, size1);
                Marshal.Copy(bmp2Data.Scan0, data2, 0, size2);

                for (int y = 0; y < bmp2Data.Height; y++)
                {
                    for (int x = 0; x < bmp2Data.Width; x++)
                    {
                        int row = y + (posY > 0 ? 0 : posY);
                        int column = x + (posX > 0 ? 0 : posX);
                        int index3 = y * bmp2Data.Stride + x * bpp3;
                        if (row >= height || row < 0 || column >= width || column < 0)
                        {
                            continue;
                        }
                        else
                        {
                            int index1 = row * bmp1Data.Stride + column * bpp1;
                            System.Drawing.Color c1;

                            if (bpp1 == 4)
                                c1 = Color.FromArgb(data1[index1 + 3], data1[index1 + 2], data1[index1 + 1], data1[index1 + 0]);
                            else c1 = Color.FromArgb(255, data1[index1 + 2], data1[index1 + 1], data1[index1 + 0]);

                            data2[index3 + 0] = c1.B;
                            data2[index3 + 1] = c1.G;
                            data2[index3 + 2] = c1.R;
                            data2[index3 + 3] = c1.A;
                        }
                    }
                }


                Marshal.Copy(data2, 0, bmp2Data.Scan0, data2.Length);
                bmp1.UnlockBits(bmp1Data);
                bmp2.UnlockBits(bmp2Data);
                isOutside = false;
                return bmp2;
            }
            catch (Exception e)
            {
                var st = new StackTrace(e, true);
                var frame = st.GetFrame(0);
                var line = frame.GetFileLineNumber();
                throw;
            }
        }
        public BitmapSource BitmapToSource(System.Drawing.Bitmap bitmap)
        {
            try
            {
                var bitmapData = bitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

                var bitmapSource = BitmapSource.Create(
                    bitmapData.Width, bitmapData.Height,
                    bitmap.HorizontalResolution, bitmap.VerticalResolution,
                    System.Windows.Media.PixelFormats.Bgr32, null,
                    bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

                bitmap.UnlockBits(bitmapData);
                return bitmapSource;
            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}
