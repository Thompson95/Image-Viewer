using ImageViewer.Model;
using ImageViewer.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.Methods
{
    public class FileDialogMethod
    {
        public void ReturnFilesFromDialog(ObservableCollection<Image> list)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Filter = "JPG / PNG / BMP / TIFF (*.jpg; *.png; *.bmp; *.tiff)|*.jpg; *.png; *.bmp; *.tiff| JPG|*.jpg|PNG|*.png|BMP|*.bmpf|TIFF|*.tiff";
            if (fileDialog.ShowDialog() == true)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    foreach (string item in fileDialog.FileNames)
                    {
                        Image image = new Image() { FileName = Path.GetFileName(item), FilePath = item, Extension = Path.GetExtension(item) };
                        if (list.Count != 0 && CheckExtension(item))
                        {
                            //contains = list.Any(x => x.FilePath == item);
                            //if (contains == false)
                                list.Add(image);
                        }
                        else if (CheckExtension(item))
                            list.Add(image);
                    }
                });
            }

        }

        public bool CheckExtension(string path)
        {
            bool correctExtension = false;
            Path.GetFullPath(path);
            var siema = Path.GetExtension(path);
            if (string.Equals(Path.GetExtension(path), ".jpg", StringComparison.CurrentCultureIgnoreCase)
                || string.Equals(Path.GetExtension(path), ".tiff", StringComparison.CurrentCultureIgnoreCase) 
                || string.Equals(Path.GetExtension(path), ".png", StringComparison.CurrentCultureIgnoreCase)
                || string.Equals(Path.GetExtension(path), ".bmp", StringComparison.CurrentCultureIgnoreCase))
                correctExtension = true;

            return correctExtension;
        }
    }
}
