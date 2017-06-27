using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using iFactr.Core;
using iFactr.UI;
using MonoCross.Navigation;
using Thickness = System.Windows.Thickness;

namespace iFactr.Wpf
{
    public class MenuButton : ButtonBase, IMenuButton, INotifyPropertyChanged
    {
        public event EventHandler Clicked;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ImagePath
        {
            get { return imagePath; }
            set
            {
                if (value != imagePath)
                {
                    imagePath = value;
                    SetImage(imagePath);

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
            get
            {
                var grid = Content as System.Windows.Controls.Grid;
                if (grid == null)
                    return null;

                var textBlock = grid.Children.OfType<TextBlock>().FirstOrDefault();
                return textBlock == null ? null : textBlock.Text;
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

        public MenuButton(string title)
        {
            BorderThickness = new System.Windows.Thickness(0);
            Command = new ButtonCommand(this);
            HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
            FontSize = 12;
            IsEnabled = true;
            MaxHeight = 40;
            Margin = new Thickness(2);
            Padding = new Thickness(5, 5, 35, 5);

            var grid = new System.Windows.Controls.Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = new System.Windows.GridLength(16, System.Windows.GridUnitType.Pixel) },
                    new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) }
                }
            };
            Content = grid;

            var textBlock = new TextBlock()
            {
                Margin = new Thickness(8, 0, 0, 0),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Text = title
            };
            System.Windows.Controls.Grid.SetColumn(textBlock, 1);
            grid.Children.Add(textBlock);
        }

        public bool Equals(IMenuButton other)
        {
            var item = other as iFactr.UI.MenuButton;
            if (item != null)
            {
                return item.Equals(this);
            }

            return base.Equals(other);
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            BorderThickness = new Thickness(1);
            Margin = new Thickness(0);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (!IsKeyboardFocused)
            {
                BorderThickness = new Thickness(0);
                Margin = new Thickness(2);
            }
        }

        protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            BorderThickness = new Thickness(1);
            Margin = new Thickness(0);
        }

        protected override void OnLostKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);

            if (!IsMouseOver)
            {
                BorderThickness = new Thickness(0);
                Margin = new Thickness(2);
            }
        }

        private async void SetImage(string uri)
        {
            var grid = Content as System.Windows.Controls.Grid;
            if (grid != null)
            {
                if (grid.Children.Count > 1)
                {
                    grid.Children.RemoveAt(1);
                }

                grid.Children.Insert(1, new System.Windows.Controls.Image() { Source = await WpfFactory.LoadBitmapAsync(uri) });
            }
        }

        private class ButtonCommand : ICommand
        {
#pragma warning disable 67
            public event EventHandler CanExecuteChanged;
#pragma warning restore 67

            private MenuButton parent;

            public ButtonCommand(MenuButton parent)
            {
                this.parent = parent;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                var handler = parent.Clicked;
                if (handler != null)
                {
                    handler(parent.Pair ?? parent, EventArgs.Empty);
                }
                else
                {
                    iApp.Navigate(parent.NavigationLink, parent.GetParent<IMXView>());
                }
            }
        }
    }
}
