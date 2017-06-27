using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFactr.UI;

namespace iFactr.Wpf
{
    public class SearchBox : TextBoxControl, ISearchBox
    {
        public event SearchEventHandler SearchPerformed;

        public Color BackgroundColor
        {
            get { return Background.GetColor(); }
            set
            {
                if (value != BackgroundColor)
                {
                    Background = value.IsDefaultColor ? null : value.GetBrush();
                    OnPropertyChanged("BackgroundColor");
                }
            }
        }

        public Color BorderColor
        {
            get { return base.BorderBrush.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? new Color(211, 211, 211) : value;
                if (value != BorderColor)
                {
                    base.BorderBrush = value.GetBrush();
                    OnPropertyChanged("BorderColor");
                }
            }
        }

        public Color ForegroundColor
        {
            get { return Foreground.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? Color.Black : value;
                if (value != ForegroundColor)
                {
                    Foreground = value.IsDefaultColor ? System.Windows.Media.Brushes.Black : value.GetBrush();
                    OnPropertyChanged("ForegroundColor");
                }
            }
        }

        public TextCompletion TextCompletion
        {
            get { return SpellCheck.IsEnabled ? TextCompletion.OfferSuggestions : TextCompletion.Disabled; }
            set
            {
                if (value != TextCompletion)
                {
                    SpellCheck.IsEnabled = (value & TextCompletion.OfferSuggestions) != 0;
                    OnPropertyChanged("TextCompletion");
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

        private bool setFocusOnLoad;

        public SearchBox()
        {
            BorderThickness = new System.Windows.Thickness(1);
            Padding = new System.Windows.Thickness(2);

            FontSize = 12;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            TextChanged += (o, e) =>
            {
                var handler = SearchPerformed;
                if (handler != null)
                {
                    handler(Pair ?? this, new SearchEventArgs(Text));
                }
            };

            Loaded += (o, e) =>
            {
                if (setFocusOnLoad)
                {
                    setFocusOnLoad = false;
                    base.Focus();
                }
            };
        }

        public new void Focus()
        {
            setFocusOnLoad = (!base.Focus() && !IsLoaded);
        }
    }
}
