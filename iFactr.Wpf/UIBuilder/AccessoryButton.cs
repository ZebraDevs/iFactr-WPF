using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace iFactr.Wpf
{
    public class AccessoryButton : System.Windows.Controls.Button
    {
        static AccessoryButton()
        {
            AccessoryButton.BorderThicknessProperty.OverrideMetadata(typeof(AccessoryButton), new FrameworkPropertyMetadata(OnBorderThicknessChanged));
        }

        public AccessoryButton()
        {
            HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            Width = 20;
            Background = Brushes.Transparent;
            BorderThickness = new System.Windows.Thickness(1);
            FontFamily = new System.Windows.Media.FontFamily(new Uri("pack://application:,,,/iFactr.Wpf;component/"), "./Resources/#WPF-Symbol");
            Content = "\uE602";
        }

        private static void OnBorderThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var button = obj as AccessoryButton;
            if (button != null)
            {
                button.Padding = new Thickness(4 - button.BorderThickness.Left, 4 - button.BorderThickness.Top,
                    4 - button.BorderThickness.Right, 4 - button.BorderThickness.Bottom);
            }
        }
    }
}
