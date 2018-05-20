using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace ImageViewer.View.ImagesWindow
{
    /// <summary>
    /// Interaction logic for SaveRegionWindow.xaml
    /// </summary>
    public partial class SaveRegionWindow : MetroWindow
    {
        private static SaveRegionWindow _instance;

        private SaveRegionWindow()
        {
            InitializeComponent();
            Closed += new System.EventHandler(MyWindow_Closed);
        }

        public static SaveRegionWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SaveRegionWindow();
                }
                return _instance;
            }
        }

        void MyWindow_Closed(object sender, System.EventArgs e)
        {

            _instance = null;
        }
    }
}
