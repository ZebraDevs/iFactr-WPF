using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using iFactr.Core;
using iFactr.UI;
using MonoCross.Navigation;
using Thickness = System.Windows.Thickness;

namespace iFactr.Wpf
{
    public class ToolbarButton : ButtonBase, IToolbarButton, INotifyPropertyChanged
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
                    SetBackground(imagePath);

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
            get { return Content as string; }
            set
            {
                if (value != Title)
                {
                    Content = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Title"));
                    }
                }
            }
        }

        public Color ForegroundColor
        {
            get { return Foreground.GetColor(); }
            set
            {
                if (value != ForegroundColor)
                {
                    Foreground = value.IsDefaultColor ? null : value.GetBrush();

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("ForegroundColor"));
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

        public ToolbarButton()
        {
            Background = new UI.Color(240, 240, 240).GetBrush();
            BorderBrush = new UI.Color(172, 172, 172).GetBrush();
            Foreground = null;
            Margin = new Thickness(5, 8, 5, 8);
            MinWidth = 75;
            Padding = new Thickness(15, 5, 15, 5);
            VerticalAlignment = System.Windows.VerticalAlignment.Center;
        }

        public bool Equals(IToolbarButton other)
        {
            var item = other as iFactr.UI.ToolbarButton;
            if (item != null)
            {
                return item.Equals(this);
            }

            return base.Equals(other);
        }

        protected override void OnClick()
        {
            var handler = Clicked;
            if (handler != null)
            {
                handler(Pair ?? this, EventArgs.Empty);
            }
            else
            {
                iApp.Navigate(NavigationLink, this.GetParent<IMXView>());
            }
        }

        private async void SetBackground(string uri)
        {
            Background = await uri.GetImageBrush();
        }
    }

    // yeah, this implementation is kind of lame.  go ahead and replace it if you are so inclined
    public class ToolbarSeparator : System.Windows.Controls.TextBlock, IToolbarSeparator, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Color ForegroundColor
        {
            get { return Foreground.GetColor(); }
            set
            {
                if (value != ForegroundColor)
                {
                    Foreground = value.IsDefaultColor ? null : value.GetBrush();

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("ForegroundColor"));
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

        public ToolbarSeparator()
        {
            Text = "|";
            FontSize = 20;
            FontFamily = new System.Windows.Media.FontFamily("Webdings");
            FontWeight = FontWeights.UltraLight;
            Foreground = null;
            Margin = new Thickness(2);
            VerticalAlignment = System.Windows.VerticalAlignment.Center;
        }

        public bool Equals(IToolbarSeparator other)
        {
            var item = other as iFactr.UI.ToolbarSeparator;
            if (item != null)
            {
                return item.Equals(this);
            }

            return base.Equals(other);
        }
    }
}
