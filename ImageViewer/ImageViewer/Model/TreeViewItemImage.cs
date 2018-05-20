using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace ImageViewer.Model
{
    class TreeViewItemImage : TreeViewItem
    {
        private ImageSource imageSource;

        public ImageSource ImageSource
        {
            get
            {
                return imageSource;
            }
            set
            {
                imageSource = value;
            }
        }

        public void AddItem(TreeViewItemImage item)
        {
            try
            {
                string head = item.Header.ToString();
                Icon icon = Icon.ExtractAssociatedIcon(item.Tag.ToString());
                using (Bitmap bmp = icon.ToBitmap())
                {
                    var stream = new MemoryStream();
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    item.ImageSource = BitmapFrame.Create(stream);
                }
            }
            catch (Exception)
            {

                item.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/FolderIcon.png", UriKind.Absolute));
            }
            finally
            {
                this.Items.Add(item);
            }
        }
    }
}
