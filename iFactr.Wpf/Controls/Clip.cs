using System;
using System.Windows;

namespace iFactr.Wpf
{
    public class Clip
    {
        public static bool GetToBounds(DependencyObject depObj)
        {
            return (bool)depObj.GetValue(ToBoundsProperty);
        }

        public static void SetToBounds(DependencyObject depObj, bool clipToBounds)
        {
            depObj.SetValue(ToBoundsProperty, clipToBounds);
        }

        /// <summary>
        /// Identifies the ToBounds Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ToBoundsProperty = DependencyProperty.RegisterAttached("ToBounds", typeof(bool), typeof(Clip), new PropertyMetadata(false, OnToBoundsPropertyChanged));

        private static void OnToBoundsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if (fe == null) return;
            ClipToBounds(fe);
            fe.Loaded += new RoutedEventHandler(ClipEvent);
            fe.SizeChanged += new SizeChangedEventHandler(ClipEvent);
        }

        /// <summary>
        /// Creates a rectangular clipping geometry which matches the geometry of the passed element
        /// </summary>
        private static void ClipToBounds(FrameworkElement fe)
        {
            fe.Clip = GetToBounds(fe) ?
                new System.Windows.Media.RectangleGeometry() { Rect = new Rect(0, 0, fe.ActualWidth, fe.ActualHeight) } :
                null;
        }

        private static void ClipEvent(object sender, EventArgs e)
        {
            ClipToBounds(sender as FrameworkElement);
        }
    }
}