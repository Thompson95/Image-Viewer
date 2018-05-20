using ImageViewer.Model.Event;
using MahApps.Metro.Controls;
using Prism.Events;
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
    /// Interaction logic for DisplayImageWindow.xaml
    /// </summary>
    public partial class DisplayImageWindow : MetroWindow
    {

        private static DisplayImageWindow _instance;
        protected IEventAggregator _aggregator = GlobalEvent.GetEventAggregator();

        private DisplayImageWindow()
        {
            InitializeComponent();
            Closed += new System.EventHandler(MyWindow_Closed);
        }
        public static DisplayImageWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DisplayImageWindow();
                }
                return _instance;
            }
        }

        void MyWindow_Closed(object sender, System.EventArgs e)
        {

            _instance = null;
            _aggregator.GetEvent<DisposeEvent>().Publish();
        }
        public void Dispose()
        {
            _instance = null;
            this.Close();
        }
    }
}
