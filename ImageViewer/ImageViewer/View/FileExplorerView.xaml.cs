using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Drawing;
using ImageViewer.Model;
using ImageViewer.Model.Event;
using Prism.Events;
using System.Collections.ObjectModel;

namespace ImageViewer.View
{
    /// <summary>
    /// Interaction logic for FileExplorerView.xaml
    /// </summary>
    public partial class FileExplorerView : UserControl
    {
        private IEventAggregator _aggregator = GlobalEvent.GetEventAggregator();

        public FileExplorerView()
        {
            _aggregator.GetEvent<CollapseAllEvent>().Subscribe(CollapseAll);
            InitializeComponent();

        }
#region Methods
        private void Expand(object sender)
        {
            TreeViewItemImage treeViewItem = (TreeViewItemImage)sender;
            if (treeViewItem.Items.Count != 1 || treeViewItem.Items[0] != null)
            {
                return;
            }

            treeViewItem.Items.Clear();
            string folderName = (string)treeViewItem.Tag;
            var directories = new List<string>();

            try
            {
                List<string> dirs = Directory.GetDirectories(folderName).Where(x => 
                {
                    DirectoryInfo di = new DirectoryInfo(x);
                    return !di.Attributes.HasFlag(FileAttributes.ReparsePoint) && !di.Attributes.HasFlag(FileAttributes.Hidden);
                }
                ).ToList();

                if (dirs.Count > 0)
                {
                    directories.AddRange(dirs);
                }
            }
            catch (Exception)
            {
                CollapseAll();
            }

            directories.ForEach(directoryPath =>
            {
                TreeViewItemImage subItem = new TreeViewItemImage()
                {
                    Header = GetPath(directoryPath),
                    Tag = directoryPath
                };

                if (Path.GetExtension(subItem.Tag.ToString()) == "")
                {
                    if (!CheckIfEmpty(subItem.Tag.ToString()))
                    {
                        subItem.Items.Add(null);
                    }
                }

                subItem.Expanded += Folder_Expanded;
                treeViewItem.AddItem(subItem);
            });
            GetFiles(treeViewItem);
        }

        public string GetPath(string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            int index = path.LastIndexOf('\\');
            if (index <= 0)
            {
                return path;
            }
            return path.Substring(index + 1);
        }

        private List<string> GetImages(string path)
        {
            return Directory.GetFiles(path).Where(x => Path.GetExtension(x).ToLower() == ".jpg" || Path.GetExtension(x).ToLower() == ".bmp" || Path.GetExtension(x).ToLower() == ".png"
                    || Path.GetExtension(x).ToLower() == ".tiff" || Path.GetExtension(x).ToLower() == ".jpeg").ToList();
        }

        private void GetFiles(TreeViewItemImage item)
        {
            List<string> files = new List<string>();
            string folderName = (string)item.Tag;
            try
            {
                var dirs = Directory.GetFiles(folderName);
                var images = GetImages(folderName);

                if (images.Count > 0)
                {
                    files.AddRange(images);
                }
            }
            catch (Exception)
            {
                CollapseAll();
            }

            files.ForEach(directoryPath =>
            {
                TreeViewItemImage subItem = new TreeViewItemImage()
                {
                    Header = GetPath(directoryPath),
                    Tag = directoryPath,
                };
                item.AddItem(subItem);
            });
        }

        private bool CheckIfEmpty(string folderPath)
        {
            try
            {
                var files = Directory.EnumerateFileSystemEntries(folderPath).ToArray();
                if (files.Where(x => Path.GetExtension(x) == "").Any())
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return true;
            }

            var dirs = Directory.GetFiles(folderPath);
            var images = GetImages(folderPath);

            return images.Count == 0 ? true : false;
        }

        #endregion

#region Events
        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => Expand(sender)
            );
        }
        private void CollapseAll()
        {
            FolderTreeView.Items.Clear();
            FileExplorer_Loaded(null, null);
        }

        private void FileExplorer_Loaded(object sender, RoutedEventArgs e)
        {
            DriveInfo[] logicalDrives = DriveInfo.GetDrives().Where(x => x.DriveType != DriveType.CDRom).ToArray();
            foreach (var drive in Directory.GetLogicalDrives())
            {
                var item = new TreeViewItemImage()
                {
                    Header = drive,
                    Tag = drive,
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/DriveIcon.png", UriKind.Absolute))
                };

                if (!CheckIfEmpty(item.Tag.ToString()))
                {
                    item.Items.Add(null);
                }
                item.Expanded += Folder_Expanded;
                FolderTreeView.Items.Add(item);
            }
            String path;
            TreeViewItemImage specialItem;
            path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            specialItem = new TreeViewItemImage()
            {
                Header = "Desktop",
                Tag = path,
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/MyComputerIcon.png", UriKind.Absolute))
            };
            if (!CheckIfEmpty(specialItem.Tag.ToString()))
            {
                specialItem.Items.Add(null);
            }
            specialItem.Expanded += Folder_Expanded;
            FolderTreeView.Items.Add(specialItem);

            path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            specialItem = new TreeViewItemImage()
            {
                Header = "My Documents",
                Tag = path,
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/FolderIcon.png", UriKind.Absolute))
            };
            if (!CheckIfEmpty(specialItem.Tag.ToString()))
            {
                specialItem.Items.Add(null);
            }
            specialItem.Expanded += Folder_Expanded;
            FolderTreeView.Items.Add(specialItem);

            path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            specialItem = new TreeViewItemImage()
            {
                Header = "My Pictures",
                Tag = path,
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/FolderIcon.png", UriKind.Absolute))
            };
            if (!CheckIfEmpty(specialItem.Tag.ToString()))
            {
                specialItem.Items.Add(null);
            }
            specialItem.Expanded += Folder_Expanded;
            FolderTreeView.Items.Add(specialItem);
        }
        public void TreeView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var clickedItem = TryGetClickedItem(e);
            if (clickedItem == null || Path.GetExtension(clickedItem.Header.ToString()) == "")
                return;

            if ((e.Source as TreeViewItemImage).IsSelected)
            {
                e.Handled = true; // to cancel expanded/collapsed toggle
                Model.Image image = new Model.Image();
                try
                {
                    image.FileName = clickedItem.Header.ToString();
                    image.FilePath = clickedItem.Tag.ToString();
                    image.Extension = Path.GetExtension(image.FilePath);
                    if (image.Extension != "" && image.Extension != ".tmp")
                    {
                        ObservableCollection<Model.Image> temp = new ObservableCollection<Model.Image>();
                        temp.Add(image);
                        _aggregator.GetEvent<SendImage>().Publish(temp);
                    }
                }
                catch (Exception)
                {
                    CollapseAll();
                }
            }
        }
        TreeViewItemImage TryGetClickedItem(MouseButtonEventArgs e)
        {
            var hit = e.OriginalSource as DependencyObject;
            while (hit != null && !(hit is TreeViewItemImage))
                hit = VisualTreeHelper.GetParent(hit);

            return hit as TreeViewItemImage;
        }
        private void TreeViewItemImage_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                TreeViewItemImage tr = sender as TreeViewItemImage;
                if (tr != null && e.LeftButton == MouseButtonState.Pressed)
                {
                    var hit = e.OriginalSource as DependencyObject;
                    while (hit != null && !(hit is TreeViewItemImage))
                        hit = VisualTreeHelper.GetParent(hit);

                    DragDrop.DoDragDrop(tr, hit, DragDropEffects.Copy);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        #endregion
    }
}
