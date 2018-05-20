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
    public struct RegionImage
    {
        public string FileName { get; }
        public string FilePath { get; }
        public string Extension { get; }

        public  RegionImage(string fileName, string filePath, string extension)
        {
            FileName = fileName;
            FilePath = filePath;
            Extension = extension;
        }
    }
}
