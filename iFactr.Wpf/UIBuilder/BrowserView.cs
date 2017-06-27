using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using iFactr.Core;
using iFactr.Core.Controls;
using iFactr.Core.Layers;
using iFactr.UI;

using Binding = System.Windows.Data.Binding;
using Thickness = System.Windows.Thickness;

namespace iFactr.Wpf
{
    public class BrowserView : BaseView, IBrowserView
    {
        public event EventHandler<LoadFinishedEventArgs> LoadFinished;

        public bool CanGoBack
        {
            get { return _webBrowser.CanGoBack; }
        }
        private bool canGoBack;

        public bool CanGoForward
        {
            get { return _webBrowser.CanGoForward; }
        }
        private bool canGoForward;

        public bool EnableDefaultControls
        {
            get { return backButton != null && backButton.Parent != null; }
            set
            {
                if (value != EnableDefaultControls)
                {
                    if (value)
                    {
                        if (backButton == null)
                        {
                            backButton = new ButtonBase()
                            {
                                Background = new Color().GetBrush(),
                                BorderThickness = new Thickness(0),
                                FontFamily = new System.Windows.Media.FontFamily("Segoe UI Symbol"),
                                FontSize = 16,
                                Margin = new Thickness(2, 2, 16, 2),
                                Padding = new Thickness(6, 0, 6, 0),
                                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                                Content = "\uE0C4",
                                ToolTip = iApp.Factory.GetResourceString("GoBack"),
                                IsEnabled = _webBrowser.CanGoBack
                            };
                            System.Windows.Controls.Grid.SetColumn(backButton, 2);
                            backButton.SetBinding(Button.ForegroundProperty, new Binding("Foreground") { Source = TitleBlock });
                            backButton.Click += (o, e) => GoBack();

                            backButton.MouseEnter += (o, e) =>
                            {
                                backButton.BorderThickness = new Thickness(1);
                                backButton.Margin = new Thickness(0, 0, 14, 0);
                            };

                            backButton.MouseLeave += (o, e) =>
                            {
                                if (!backButton.IsKeyboardFocused)
                                {
                                    backButton.BorderThickness = new Thickness(0);
                                    backButton.Margin = new Thickness(2, 2, 16, 2);
                                }
                            };

                            backButton.GotKeyboardFocus += (o, e) =>
                            {
                                backButton.BorderThickness = new Thickness(1);
                                backButton.Margin = new Thickness(0, 0, 14, 0);
                            };

                            backButton.LostKeyboardFocus += (o, e) =>
                            {
                                if (!backButton.IsMouseOver)
                                {
                                    backButton.BorderThickness = new Thickness(0);
                                    backButton.Margin = new Thickness(2, 2, 16, 2);
                                }
                            };
                        }

                        if (forwardButton == null)
                        {
                            forwardButton = new ButtonBase()
                            {
                                Background = new Color().GetBrush(),
                                BorderThickness = new Thickness(0),
                                FontFamily = new System.Windows.Media.FontFamily("Segoe UI Symbol"),
                                FontSize = 16,
                                Margin = new Thickness(2, 2, 16, 2),
                                Padding = new Thickness(6, 0, 6, 0),
                                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                                Content = "\uE0AD",
                                ToolTip = iApp.Factory.GetResourceString("GoForward"),
                                IsEnabled = _webBrowser.CanGoForward
                            };
                            System.Windows.Controls.Grid.SetColumn(forwardButton, 3);
                            forwardButton.SetBinding(Button.ForegroundProperty, new Binding("Foreground") { Source = TitleBlock });
                            forwardButton.Click += (o, e) => GoForward();

                            forwardButton.MouseEnter += (o, e) =>
                            {
                                forwardButton.BorderThickness = new Thickness(1);
                                forwardButton.Margin = new Thickness(0, 0, 14, 0);
                            };

                            forwardButton.MouseLeave += (o, e) =>
                            {
                                if (!forwardButton.IsKeyboardFocused)
                                {
                                    forwardButton.BorderThickness = new Thickness(0);
                                    forwardButton.Margin = new Thickness(2, 2, 16, 2);
                                }
                            };

                            forwardButton.GotKeyboardFocus += (o, e) =>
                            {
                                forwardButton.BorderThickness = new Thickness(1);
                                forwardButton.Margin = new Thickness(0, 0, 14, 0);
                            };

                            forwardButton.LostKeyboardFocus += (o, e) =>
                            {
                                if (!forwardButton.IsMouseOver)
                                {
                                    forwardButton.BorderThickness = new Thickness(0);
                                    forwardButton.Margin = new Thickness(2, 2, 16, 2);
                                }
                            };
                        }

                        if (!HeaderBar.Children.Contains(backButton))
                        {
                            HeaderBar.Children.Add(backButton);
                        }

                        if (!HeaderBar.Children.Contains(forwardButton))
                        {
                            HeaderBar.Children.Add(forwardButton);
                        }
                    }
                    else
                    {
                        if (HeaderBar.Children.Contains(backButton))
                        {
                            HeaderBar.Children.Remove(backButton);
                        }

                        if (HeaderBar.Children.Contains(forwardButton))
                        {
                            HeaderBar.Children.Remove(forwardButton);
                        }
                    }

                    OnPropertyChanged("EnableDefaultControls");
                }
            }
        }
        private System.Windows.Controls.Button backButton, forwardButton;

        public IMenu Menu
        {
            get
            {
                var menu = HeaderBar.Children.OfType<IMenu>().FirstOrDefault();
                return menu == null ? null : (menu.Pair as IMenu) ?? menu;
            }
            set
            {
                if (value != Menu)
                {
                    for (int i = HeaderBar.Children.Count - 1; i >= 0; i--)
                    {
                        if (HeaderBar.Children[i] is IMenu)
                        {
                            HeaderBar.Children.RemoveAt(i);
                        }
                    }

                    var menu = WpfFactory.GetNativeObject<UIElement>(value, "menu", true);
                    if (menu != null)
                    {
                        System.Windows.Controls.Grid.SetColumn(menu, HeaderBar.ColumnDefinitions.Count - 1);
                        HeaderBar.Children.Add(menu);
                    }

                    OnPropertyChanged("Menu");
                }
            }
        }

        private readonly WebBrowser _webBrowser = new WebBrowser();

        public BrowserView()
        {
            HeaderBar.ColumnDefinitions.Insert(2, new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            HeaderBar.ColumnDefinitions.Insert(2, new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            System.Windows.Controls.Grid.SetColumnSpan(TitleBlock, 5);

            _webBrowser.LoadCompleted += (o, e) =>
            {
                Title = _webBrowser.Source == null ? string.Empty : _webBrowser.Source.OriginalString;

                if(_webBrowser.Document != null)
                { 
                    PropertyInfo propertyInfo = _webBrowser.Document.GetType().GetProperty("title");
                    if (propertyInfo != null)
                    {
                        Title = (string)propertyInfo.GetValue(_webBrowser.Document, null) as string;
                    }
                }

                var loadFinished = LoadFinished;
                if (loadFinished != null) loadFinished(Pair ?? this, new LoadFinishedEventArgs(e.Uri.OriginalString));
            };

            _webBrowser.Navigating += (o, e) =>
            {
                Title = WpfFactory.Instance.GetResourceString("Loading");
            };

            _webBrowser.Navigated += (o, e) =>
            {
                if (canGoBack != _webBrowser.CanGoBack)
                {
                    canGoBack = _webBrowser.CanGoBack;
                    OnPropertyChanged("CanGoBack");
                }

                if (canGoForward != _webBrowser.CanGoForward)
                {
                    canGoForward = _webBrowser.CanGoForward;
                    OnPropertyChanged("CanGoForward");
                }

                if (backButton != null)
                {
                    backButton.IsEnabled = _webBrowser.CanGoBack;
                }

                if (forwardButton != null)
                {
                    forwardButton.IsEnabled = _webBrowser.CanGoForward;
                }
            };

            Children.Add(_webBrowser);
        }

        public void GoBack()
        {
            _webBrowser.GoBack();
        }

        public void GoForward()
        {
            _webBrowser.GoForward();
        }

        public void LaunchExternal(string url)
        {
            Parameter.CheckUrl(url);
            Launch(url);
        }

        public void Load(string url)
        {
            Parameter.CheckUrl(url);
            _webBrowser.Navigate(url);
        }

        public void LoadFromString(string html)
        {
            _webBrowser.NavigateToString(html);
        }

        public void Refresh()
        {
            _webBrowser.Refresh();
        }

        public static void Launch(string url)
        {
            var process = new System.Diagnostics.Process { StartInfo = { FileName = url, }, };
            try
            {
                process.Start();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                //don't need to do this for Vista and up...
                process.StartInfo.FileName = "rundll32.exe";
                process.StartInfo.Arguments = "shell32.dll,OpenAs_RunDLL " + url;
                process.Start();
            }
        }
    }
}
