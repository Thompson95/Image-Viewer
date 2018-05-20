using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ImageViewer.Model
{
    class ResizableGrid : Border
    {
        public double RHeight
        {
            get
            {
                return (double)GetValue(RHeightProperty);
            }
            set
            {
                SetValue(RHeightProperty, value);
            }
        }
        public double RWidth
        {
            get
            {
                return (double)GetValue(RWidthProperty);
            }
            set
            {
                SetValue(RWidthProperty, value);
            }
        }
        public ResizableGrid()
        {
            this.SizeChanged += ResizableGrid_SizeChanged;
        }

        private void ResizableGrid_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            RHeight = e.NewSize.Height;
            RWidth = e.NewSize.Width;
        }

        public static readonly DependencyProperty RHeightProperty = DependencyProperty.Register("RHeight", typeof(double), typeof(ResizableGrid));
        public static readonly DependencyProperty RWidthProperty = DependencyProperty.Register("RWidth", typeof(double), typeof(ResizableGrid));
    }
}
