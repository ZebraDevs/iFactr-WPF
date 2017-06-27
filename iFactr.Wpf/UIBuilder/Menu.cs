using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using iFactr.Core;
using iFactr.UI;
using Binding = System.Windows.Data.Binding;
using Thickness = System.Windows.Thickness;

namespace iFactr.Wpf
{
    public class Menu : ButtonBase, IMenu, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Color BackgroundColor
        {
            get { return itemsControl.Background.GetColor(); }
            set 
            {
                value = value.IsDefaultColor ? Color.White : value;
                if (value != BackgroundColor)
                {
                    itemsControl.Background = value.GetBrush();
                    foreach (System.Windows.Controls.Button button in itemsControl.Items)
                    {
                        button.Background = itemsControl.Background;
                    }

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("BackgroundColor"));
                    }
                }
            }
        }

        public int ButtonCount
        {
            get { return itemsControl.Items.Count; }
        }

        public Color ForegroundColor
        {
            get { return itemsControl.Foreground.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? Color.Black : value;
                if (value != ForegroundColor)
                {
                    itemsControl.Foreground = value.GetBrush();
                    foreach (System.Windows.Controls.Button button in itemsControl.Items)
                    {
                        button.Foreground = itemsControl.Foreground;
                    }

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("ForegroundColor"));
                    }
                }
            }
        }

        public string ImagePath
        {
            get { return imagePath; }
            set
            {
                if (value != imagePath)
                {
                    imagePath = value;
                    if (itemsControl.Items.Count != 1)
                    {
                        SetImage(imagePath);
                    }

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("ImagePath"));
                    }
                }
            }
        }
        private string imagePath;

        public Color SelectionColor
        {
            get { return HighlightColor.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? new Color(145, 200, 255) : value;
                if (value != SelectionColor)
                {
                    HighlightColor = value.GetColor();

                    foreach (ButtonBase button in itemsControl.Items)
                    {
                        button.HighlightColor = HighlightColor;
                    }

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("SelectionColor"));
                    }
                }
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                if (value != title)
                {
                    title = value;
                    if (itemsControl.Items.Count != 1 && !HasImage)
                    {
                        if (title == null)
                        {
                            Content = "\uE600";
                            FontFamily = new System.Windows.Media.FontFamily(new Uri("pack://application:,,,/iFactr.Wpf;component/"), "./Resources/#WPF-Symbol");
                            FontSize = 20;
                        }
                        else
                        {
                            Content = title;
                            FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
                            FontSize = 12;
                        }
                    }

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Title"));
                    }
                }
            }
        }
        private string title;

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

        private bool HasImage
        {
            get
            {
                var image = Content as System.Windows.Controls.Image;
                return image != null && image.Source != null; }
        }

        private Popup popup;
        private ItemsControl itemsControl;

        public Menu()
        {
            Background = System.Windows.Media.Brushes.Transparent;
            BorderThickness = new Thickness(0);
            FontSize = 20;
            MaxHeight = 40;
            Margin = new Thickness(2, 2, 10, 2);
            Padding = new Thickness(6, 0, 6, 0);
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

            itemsControl = new ItemsControl()
            {
                Background = System.Windows.Media.Brushes.White,
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                IsTabStop = false
            };
            itemsControl.SetBinding(ItemsControl.MinWidthProperty, new Binding("ActualWidth") { Source = this });
            KeyboardNavigation.SetTabNavigation(itemsControl, KeyboardNavigationMode.Continue);

            base.Loaded += (o, e) =>
            {
                var parent = this.GetParent<IView>();
                if (parent != null)
                {
                    ((INotifyPropertyChanged)this).SetBinding(new UI.Binding("Foreground", "TitleColor")
                    {
                        Source = parent,
                        Mode = UI.BindingMode.OneWayToTarget,
                        ValueConverter = new ColorToBrushConverter()
                    });
                }
            };

            base.Unloaded += (o, e) =>
            {
                ((INotifyPropertyChanged)this).ClearBinding("Foreground");
            };

            popup = new Popup()
            {
                AllowsTransparency = false,
                Child = itemsControl,
                Focusable = false,
                PlacementTarget = this,
                Placement = PlacementMode.Bottom,
                PopupAnimation = PopupAnimation.Slide,
                StaysOpen = false
            };

            popup.KeyUp += (o, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    popup.IsOpen = false;
                }
            };

            Title = null;
        }

        public void Add(IMenuButton menuButton)
        {
            var button = WpfFactory.GetNativeObject<System.Windows.Controls.Button>(menuButton, "menuButton", false);
            if (button != null)
            {
                button.Click -= ClosePopup;
                button.Click += ClosePopup;

                button.Background = itemsControl.Background;
                button.Foreground = itemsControl.Foreground;

                var buttonBase = button as ButtonBase;
                if (buttonBase != null)
                {
                    buttonBase.HighlightColor = HighlightColor;
                }

                itemsControl.Items.Add(button);
            }

            if (itemsControl.Items.Count != 1)
            {
                SetImage(imagePath);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(menuButton.ImagePath))
                {
                    SetImage(menuButton.ImagePath);
                }

                if (!HasImage)
                {
                    Content = menuButton.Title;
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
                    FontSize = 12;
                }
            }

            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs("ButtonCount"));
            }
        }

        public IMenuButton GetButton(int index)
        {
            var button = itemsControl.Items[index] as IMenuButton;
            return (button.Pair as IMenuButton) ?? button;
        }

        public bool Equals(IMenu other)
        {
            var menu = other as iFactr.UI.Menu;
            if (menu != null)
            {
                return menu.Equals(this);
            }

            return base.Equals(other);
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            BorderThickness = new Thickness(1);
            Margin = new Thickness(0, 0, 8, 0);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (!IsKeyboardFocused)
            {
                BorderThickness = new Thickness(0);
                Margin = new Thickness(2, 2, 10, 2);
            }
        }

        protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            BorderThickness = new Thickness(1);
            Margin = new Thickness(0, 0, 8, 0);
        }

        protected override void OnLostKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);

            if (!IsMouseOver)
            {
                BorderThickness = new Thickness(0);
                Margin = new Thickness(2, 2, 10, 2);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Down && itemsControl.Items.Count > 1)
            {
                e.Handled = true;

                popup.IsOpen = true;
                Keyboard.Focus(itemsControl);
            }

            base.OnKeyDown(e);
        }

        protected override void OnClick()
        {
            if (itemsControl.Items.Count == 1)
            {
                var button = itemsControl.Items[0] as System.Windows.Controls.Button;
                if (button != null && button.Command != null && button.Command.CanExecute(null))
                {
                    button.Command.Execute(null);
                }
            }
            else if (itemsControl.Items.Count > 1)
            {
                popup.IsOpen = true;
                Keyboard.Focus(itemsControl);
            }
        }

        private void ClosePopup(object sender, EventArgs e)
        {
            popup.IsOpen = false;
        }

        private async void SetImage(string uri)
        {
            Content = new System.Windows.Controls.Image() { Source = await WpfFactory.LoadBitmapAsync(uri) };

            if (!HasImage)
            {
                if (title == null)
                {
                    Content = "\uE600";
                    FontFamily = new System.Windows.Media.FontFamily(new Uri("pack://application:,,,/iFactr.Wpf;component/"), "./Resources/#WPF-Symbol");
                    FontSize = 20;
                }
                else
                {
                    Content = title;
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
                    FontSize = 12;
                }
            }
        }

        private class ColorToBrushConverter : UI.IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter)
            {
                if (value is UI.Color && targetType == typeof(System.Windows.Media.Brush))
                {
                    return ((Color)value).GetBrush();
                }

                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter)
            {
                if (value is UI.Color && targetType == typeof(System.Windows.Media.Brush))
                {
                    return ((Color)value).GetBrush();
                }

                return value;
            }
        }
    }
}
