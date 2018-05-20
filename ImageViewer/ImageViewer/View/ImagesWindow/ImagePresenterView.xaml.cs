using Prism.Events;
using ImageViewer.Model.Event;
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
using System.Windows.Shapes;
using ImageViewer.Model;
using System.Windows.Threading;

namespace ImageViewer.View.ImagesWindow
{
    /// <summary>
    /// Interaction logic for ImagePresenterView.xaml
    /// </summary>
    public partial class ImagePresenterView : UserControl
    {
        protected IEventAggregator _aggregator = GlobalEvent.GetEventAggregator();
        private ResizableGrid _resizableGrid;
        public ImagePresenterView()
        {
            InitializeComponent();
            _resizableGrid = (ResizableGrid)this.FindName("resizableGrid");
        }
    }
}
