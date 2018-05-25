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
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ImageViewer.View.ImagesWindow
{
    /// <summary>
    /// Interaction logic for DisplayImageWindow.xaml
    /// </summary>
    public partial class FilterControlWindow : MetroWindow, IDisposable
    {

        private static FilterControlWindow _instance;
        protected IEventAggregator _aggregator = GlobalEvent.GetEventAggregator();

        private FilterControlWindow()
        {
            InitializeComponent();
            _aggregator.GetEvent<DisposeEvent>().Subscribe(Dispose);
            this.StateChanged += (s, e) => Window_StateChanged();
        }

        private void Window_StateChanged()
        {
            switch (this.WindowState)
            {
                case WindowState.Normal:
                    break;
                case WindowState.Minimized:
                    {
                        this.WindowState = WindowState.Normal;
                        this.Visibility = Visibility.Collapsed;
                    }
                    break;
                case WindowState.Maximized:
                    break;
                default:
                    break;
            }
        }

        public static FilterControlWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FilterControlWindow();
                }
                return _instance;
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void inputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            String text = inputBox.Text;
            double actualValue = 0;
            if (text != String.Empty)
                actualValue = double.Parse(text);
            if (actualValue > 255)
            {
                actualValue = 255;
            }
            byte value = (byte)actualValue;
            inputBox.Text = value.ToString();
        }

        public void Dispose()
        {
            _instance = null;
            this.Close();
        }

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    typeof(Window).GetField("_isClosing", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, false);
        //    e.Cancel = true;
        //    this.Hide();
        //}
        //protected override void OnClosed(EventArgs e)
        //{
        //    //base.OnClosed(e);
        //}

        //~FilterControlWindow()
        //{
        //    this.Dispose();
        //}
        //public void Dispose()
        //{
        //    _instance = null;
        //    this.Close();
        //}
    }
}