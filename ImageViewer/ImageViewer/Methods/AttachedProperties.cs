using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ImageViewer.Methods
{
    public class RowDefinitionObserver
    {

        public static readonly DependencyProperty ObserveRowProperty = DependencyProperty.RegisterAttached(
            "ObserveRow",
            typeof(bool),
            typeof(RowDefinitionObserver),
            new FrameworkPropertyMetadata(OnObserveChanged));

        public static readonly DependencyProperty ObservedRowHeightProperty = DependencyProperty.RegisterAttached(
            "ObservedRowHeight",
            typeof(double),
            typeof(RowDefinitionObserver));

        public static bool GetObserveRow(FrameworkElement frameworkElement)
        {
            return (bool)frameworkElement.GetValue(ObserveRowProperty);
        }

        public static void SetObserveRow(FrameworkElement frameworkElement, bool observe)
        {
            frameworkElement.SetValue(ObserveRowProperty, observe);
        }

        public static double GetObservedRowHeight(FrameworkElement frameworkElement)
        {
            return (double)frameworkElement.GetValue(ObservedRowHeightProperty);
        }

        public static void SetObservedRowHeight(FrameworkElement frameworkElement, double observedHeight)
        {
            frameworkElement.SetValue(ObservedRowHeightProperty, observedHeight);
        }

        private static void OnObserveChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)dependencyObject;

            if ((bool)e.NewValue)
            {
                frameworkElement.SizeChanged += OnFrameworkElementSizeChanged;
                UpdateObservedSizesForFrameworkElement(frameworkElement);
            }
            else
            {
                frameworkElement.SizeChanged -= OnFrameworkElementSizeChanged;
            }
        }

        private static void OnFrameworkElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateObservedSizesForFrameworkElement((FrameworkElement)sender);
        }

        private static void UpdateObservedSizesForFrameworkElement(FrameworkElement frameworkElement)
        {
            Grid g = frameworkElement as Grid;
            if (g != null)
            {
                if (g.RowDefinitions.Count > 1)
                {
                    SetObservedRowHeight(g, g.RowDefinitions[1].ActualHeight);
                }
            }
        }
    }
    public class ColumnDefinitionObserver
    {

        public static readonly DependencyProperty ObserveColumnProperty = DependencyProperty.RegisterAttached(
            "ObserveColumn",
            typeof(bool),
            typeof(ColumnDefinitionObserver),
            new FrameworkPropertyMetadata(OnObserveChanged));

        public static readonly DependencyProperty ObservedColumnWidthProperty = DependencyProperty.RegisterAttached(
            "ObservedColumnWidth",
            typeof(double),
            typeof(ColumnDefinitionObserver));

        public static bool GetObserveColumn(FrameworkElement frameworkElement)
        {
            return (bool)frameworkElement.GetValue(ObserveColumnProperty);
        }

        public static void SetObserveColumn(FrameworkElement frameworkElement, bool observe)
        {
            frameworkElement.SetValue(ObserveColumnProperty, observe);
        }

        public static double GetObservedColumnWidth(FrameworkElement frameworkElement)
        {
            return (double)frameworkElement.GetValue(ObservedColumnWidthProperty);
        }

        public static void SetObservedColumnWidth(FrameworkElement frameworkElement, double observedWidth)
        {
            frameworkElement.SetValue(ObservedColumnWidthProperty, observedWidth);
        }

        private static void OnObserveChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)dependencyObject;

            if ((bool)e.NewValue)
            {
                frameworkElement.SizeChanged += OnFrameworkElementSizeChanged;
                UpdateObservedSizesForFrameworkElement(frameworkElement);
            }
            else
            {
                frameworkElement.SizeChanged -= OnFrameworkElementSizeChanged;
            }
        }

        private static void OnFrameworkElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateObservedSizesForFrameworkElement((FrameworkElement)sender);
        }

        private static void UpdateObservedSizesForFrameworkElement(FrameworkElement frameworkElement)
        {
            Grid g = frameworkElement as Grid;
            if (g != null)
            {
                if (g.ColumnDefinitions.Count > 1)
                {
                    SetObservedColumnWidth(g, g.ColumnDefinitions[1].ActualWidth);
                }
            }
        }
    }
}
