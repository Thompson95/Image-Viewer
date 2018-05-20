using ImageViewer.Model;
using ImageViewer.Model.Event;
using ImageViewer.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

public static class ImageSaver
{
    public static void SafeToText(ObservableCollection<ObservableCollection<Image>> list)
    {
        string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ImageViewer";
        string filename = @"\ImagesKeeper.xml";
        string path = directory + filename;
        try
        {
            System.IO.File.WriteAllBytes(path, new byte[0]);
        }
        catch (DirectoryNotFoundException)
        {
            Directory.CreateDirectory(directory);
            if (!File.Exists(path))
                File.Create(path).Close();
        }
        finally
        {
            XDocument xdoc = new XDocument(
                     new XDeclaration("1.0", "utf-8", "yes"),
                     new XElement("Images"));
            XElement items = xdoc.Root;

            foreach (ObservableCollection<Image> x in list)
            {
                XElement parent = new XElement("ImageList");

                foreach (var image in x)
                {

                    XElement person = new XElement("Image",
                    new XElement("Extension", image.Extension),
                    new XElement("FilePath", image.FilePath),
                    new XElement("FileName", image.FileName));
                    parent.Add(person);
                }
                items.Add(parent);
            }
            xdoc.Save(path);
        }
    }


    public static void SendTheLoadedImages(ObservableCollection<ObservableCollection<Image>> list)
    {

        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ImageViewer\ImagesKeeper.xml";
        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlElement root = xmlDoc.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("//Images/ImageList");

            foreach (XmlNode node in nodes)
            {
                ObservableCollection<Image> imageList = new ObservableCollection<Image>();
                foreach (XmlNode node2 in node)
                {
                    Image image = new Image();
                    image.Extension = node2["Extension"].InnerText;
                    image.FileName = node2["FileName"].InnerText;
                    if (!File.Exists(node2["FilePath"].InnerText))
                        continue;
                    image.FilePath = node2["FilePath"].InnerText;
                    imageList.Add(image);

                }
                if(imageList.Count > 0)
                    list.Add(imageList);
            }
        }
        catch (FileNotFoundException)
        {
            return;
        }
        catch (DirectoryNotFoundException)
        {
            return;
        }
    }



}
