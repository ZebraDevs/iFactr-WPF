using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace iFactr.Wpf
{
    public class LoadSpinner
    {
        private ProgressBar bar;
        private TextBlock titleBlock;
        private Window loadWindow;

        public LoadSpinner()
        {
            titleBlock = new TextBlock()
            {
                FontSize = 12,
                Margin = new Thickness(8, 8, 8, 12),
                VerticalAlignment = VerticalAlignment.Center
            };

            bar = new System.Windows.Controls.ProgressBar()
            {
                Width = 320,
                Height = 20,
                Margin = new Thickness(8),
                Maximum = 100,
                Minimum = 0,
                IsIndeterminate = true,
                Orientation = Orientation.Horizontal,
                Visibility = Visibility.Visible
            };
        }

        public void Show(string title)
        {
            titleBlock.Text = title ?? string.Empty;

            if (loadWindow == null)
            {
                loadWindow = new Window()
                {
                    BorderBrush = new SolidColorBrush(Colors.Gray),
                    BorderThickness = new Thickness(1),
                    Owner = GetTopWindow(),
                    ResizeMode = ResizeMode.NoResize,
                    SizeToContent = System.Windows.SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    WindowStyle = WindowStyle.None,
                    Content = new StackPanel()
                    {
                        Orientation = Orientation.Vertical,
                        Children =
                        {
                            titleBlock,
                            bar
                        }
                    }
                };

                loadWindow.Closed += (o, e) =>
                {
                    var panel = loadWindow.Content as StackPanel;
                    if (panel != null)
                    {
                        panel.Children.Clear();
                    }
                    loadWindow.Content = null;
                    loadWindow = null;
                };
            }
            else
            {
                loadWindow.Left = loadWindow.Owner.Left + (loadWindow.Owner.ActualWidth / 2 - loadWindow.Width / 2);
                loadWindow.Top = loadWindow.Owner.Top + (loadWindow.Owner.ActualHeight / 2 - loadWindow.Height / 2);
            }

            if (loadWindow.Visibility == Visibility.Visible)
            {
                loadWindow.Hide();
            }

            loadWindow.ShowDialog();
        }

        public void Hide()
        {
            if (loadWindow != null)
            {
                loadWindow.Hide();
            }
        }

        private static Window GetTopWindow()
        {
            var topWindow = Application.Current.MainWindow;
            while (topWindow.OwnedWindows.Count > 0)
            {
                Window visibleWindow = null;
                foreach (Window window in topWindow.OwnedWindows)
                {
                    if (window.IsVisible)
                    {
                        visibleWindow = window;
                        break;
                    }
                }

                if (visibleWindow == null)
                {
                    break;
                }
                else
                {
                    topWindow = visibleWindow;
                }
            }

            return topWindow;
        }
    }
}
