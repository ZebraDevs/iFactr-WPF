using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using iFactr.Core;
using iFactr.UI;
using Color = iFactr.UI.Color;
using Thickness = System.Windows.Thickness;

namespace iFactr.Wpf
{
    public partial class TabItem : System.Windows.Controls.TabItem, ITabItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler Selected;

        public string BadgeValue
        {
            get { return badgeValue; }
            set
            {
                if (value != badgeValue)
                {
                    badgeValue = value;
                    if (string.IsNullOrEmpty(badgeValue))
                    {
                        if (badgeEllipse != null)
                        {
                            header.Children.Remove(badgeEllipse);
                        }
                    }
                    else
                    {
                        if (badgeEllipse == null)
                        {
                            badgeBlock = new TextBlock()
                            {
                                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                                FontSize = 8,
                                Foreground = Brushes.White,
                            };

                            badgeEllipse = new BadgeEllipse()
                            {
                                Child = badgeBlock,
                                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                                Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(200, 0, 0)),
                                BorderBrush = Brushes.White,
                                BorderThickness = new Thickness(1),
                                Margin = new Thickness(-3, -1, -3, -1),
                                Padding = new Thickness(3, 2.5, 3, 3),
                                CornerRadius = new CornerRadius(8),
                            };

                            badgeEllipse.SetBinding(Border.MinWidthProperty, new System.Windows.Data.Binding("ActualHeight")
                            {
                                Mode = System.Windows.Data.BindingMode.OneWay,
                                Source = badgeEllipse
                            });
                        }

                        badgeBlock.Text = badgeValue;
                        if (!header.Children.Contains(badgeEllipse))
                        {
                            header.Children.Add(badgeEllipse);
                        }
                    }

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("BadgeValue"));
                    }
                }
            }
        }
        private string badgeValue;

        public string ImagePath
        {
            get { return imagePath; }
            set
            {
                if (value != imagePath)
                {
                    imagePath = value;
                    SetSource(imagePath);

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("ImagePath"));
                    }
                }
            }
        }
        private string imagePath;

        public Link NavigationLink
        {
            get { return navigationLink; }
            set
            {
                if (value != navigationLink)
                {
                    navigationLink = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("NavigationLink"));
                    }
                }
            }
        }
        private Link navigationLink;

        public string Title
        {
            get { return titleBlock.Text; }
            set
            {
                value = value ?? string.Empty;
                if (value != titleBlock.Text)
                {
                    titleBlock.Text = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Title"));
                    }
                }
            }
        }

        public Color TitleColor
        {
            get { return titleBlock.Foreground.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? Color.Black : value;
                if (value != TitleColor)
                {
                    titleBlock.Foreground = value.GetBrush();

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("TitleColor"));
                    }
                }
            }
        }

        public Font TitleFont
        {
            get
            {
                var font = new Font(titleBlock.FontFamily.Source, titleBlock.FontSize);
                int format = (titleBlock.FontStyle == FontStyles.Italic ? 2 : 0) + (titleBlock.FontWeight.ToOpenTypeWeight() == FontWeights.Bold.ToOpenTypeWeight() ? 1 : 0);
                font.Formatting = (FontFormatting)format;
                return font;
            }
            set
            {
                if (value != TitleFont)
                {
                    titleBlock.FontFamily = new FontFamily(value.Name);
                    titleBlock.FontSize = value.Size;
                    titleBlock.FontStyle = (value.Formatting & FontFormatting.Italic) != 0 ? FontStyles.Italic : FontStyles.Normal;
                    titleBlock.FontWeight = (value.Formatting & FontFormatting.Bold) != 0 ? FontWeights.Bold : FontWeights.Normal;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("TitleFont"));
                    }
                }
            }
        }

        public IPairable Pair
        {
            get { return pair; }
            set
            {
                if (pair == null && value != null)
                {
                    pair = value;
                    pair.Pair = this;
                }
            }
        }
        private IPairable pair;

        private static int previousIndex;

        private System.Windows.Controls.Grid header;
        private Border badgeEllipse;
        private TextBlock titleBlock, badgeBlock;
        private System.Windows.Controls.Image image;

        public TabItem()
        {
            InitializeComponent();

            Header = (header = new System.Windows.Controls.Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) }
                },
                Children =
                {
                    (image = new System.Windows.Controls.Image()
                    {
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Top,
                        Stretch = Stretch.None
                    }),
                    (titleBlock = new TextBlock()
                    {
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                        Foreground = Brushes.Black
                    })
                },
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                MinWidth = 64
            });

            System.Windows.Controls.Grid.SetRow(titleBlock, 2);

            var gradient = Background as LinearGradientBrush;
            if (gradient != null)
            {
                Background = new LinearGradientBrush(new GradientStopCollection(gradient.GradientStops.Select(s =>
                    new GradientStop(System.Windows.Media.Color.FromArgb(96, s.Color.R, s.Color.G, s.Color.B), s.Offset))));
            }
        }

        public bool Equals(ITabItem other)
        {
            var item = other as iFactr.UI.TabItem;
            if (item != null)
            {
                return item.Equals(this);
            }

            return base.Equals(other);
        }

        protected override void OnPreviewMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsSelected)
            {
                e.Handled = !ShouldSelect();

                if (!e.Handled)
                {
                    var handler = Selected;
                    if (handler != null)
                    {
                        handler(Pair ?? this, EventArgs.Empty);
                    }
                    else
                    {
                        iApp.Navigate(NavigationLink, this.GetParent<ITabView>() ?? WpfFactory.Instance.TabView);
                    }
                }
            }
            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            e.Handled = !ShouldSelect();
            base.OnPreviewGotKeyboardFocus(e);
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);

            if (WpfFactory.Instance.MainWindow.LayoutRoot.Children.Count > 0 && !(WpfFactory.Instance.MainWindow.LayoutRoot.Children[0] is GridSplitter))
            {
                WpfFactory.Instance.MainWindow.LayoutRoot.Children.RemoveAt(0);
            }

            var parent = this.GetParent<ITabView>() ?? WpfFactory.Instance.TabView;
            if (parent == null)
            {
                return;
            }

            int index = parent.TabItems.IndexOf(Pair ?? this);
            var stack = PaneManager.Instance.FromNavContext(UI.Pane.Master, index);
            WpfFactory.Instance.MainWindow.LayoutRoot.Children.Insert(0, stack as UIElement);

            var detail = PaneManager.Instance.FromNavContext(UI.Pane.Detail, 0);
            if (detail != null && detail.FindPane() == UI.Pane.Detail)
            {
                detail.PopToRoot();
            }

            Content = WpfFactory.Instance.MainWindow.LayoutRoot;
            PaneManager.Instance.CurrentTab = index;

            var handler = Selected;
            if (handler != null)
            {
                handler(Pair ?? this, EventArgs.Empty);
            }
            else if (stack.CurrentView == null)
            {
                iApp.Navigate(NavigationLink, parent);
            }
        }

        private bool ShouldSelect()
        {
            var parent = this.GetParent<ITabView>();
            if (parent == null)
            {
                return false;
            }

            int index = parent.TabItems.IndexOf(Pair ?? this);
            if (index < 0)
            {
                return false;
            }

            previousIndex = parent.SelectedIndex;

            Link link = null;
            if (IsSelected && NavigationLink != null)
            {
                link = (Link)NavigationLink.Clone();
            }
            else
            {
                var newStack = PaneManager.Instance.FromNavContext(UI.Pane.Master, index);
                if (newStack != null && newStack.CurrentView != null)
                {
                    link = new Link(PaneManager.Instance.GetNavigatedURI(newStack.CurrentView), new Dictionary<string, string>());
                }
                else if (NavigationLink != null)
                {
                    link = (Link)NavigationLink.Clone();
                }
            }

            if (!PaneManager.Instance.ShouldNavigate(link, UI.Pane.Tabs, NavigationType.Tab))
            {
                return false;
            }

            PaneManager.Instance.CurrentTab = index;
            return true;
        }

        private async void SetSource(string uri)
        {
            image.Source = await WpfFactory.LoadBitmapAsync(uri);
        }

        private class BadgeEllipse : Border
        {
            protected override Geometry GetLayoutClip(System.Windows.Size layoutSlotSize)
            {
                return null;
            }
        }
    }
}