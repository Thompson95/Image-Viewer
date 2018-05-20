using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ImageViewer.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        App()
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ImageViewer\Regions"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ImageViewer\Regions");

            foreach (var file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ImageViewer\Regions"))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception)
                {

                }
            }
        }
        ~App()
        {
            foreach(var file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ImageViewer\Regions"))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception)
                {

                }
            }
            foreach (var file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ImageViewer"))
            {
                try
                {
                    if(Path.GetExtension(file).ToLower() == ".png")
                    File.Delete(file);
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
