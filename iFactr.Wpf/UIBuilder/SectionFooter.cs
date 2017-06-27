using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using iFactr.UI;
using Thickness = System.Windows.Thickness;

namespace iFactr.Wpf
{
    public class SectionFooter : System.Windows.Controls.Grid, ISectionFooter, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public UI.Color BackgroundColor
        {
            get { return base.Background.GetColor(); }
            set
            {
                if (value != BackgroundColor)
                {
                    base.Background = value.IsDefaultColor ? null : value.GetBrush();

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("BackgroundColor"));
                    }
                }
            }
        }

        public UI.Color ForegroundColor
        {
            get { return textBlock.Foreground.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? new UI.Color(135, 133, 133) : value;
                if (value != ForegroundColor)
                {
                    textBlock.Foreground = value.GetBrush();

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("ForegroundColor"));
                    }
                }
            }
        }

        public Font Font
        {
            get
            {
                var font = new Font(textBlock.FontFamily.Source, (float)textBlock.FontSize);
                int format = (textBlock.FontStyle == FontStyles.Italic ? 2 : 0) + (textBlock.FontWeight.ToOpenTypeWeight() == FontWeights.Bold.ToOpenTypeWeight() ? 1 : 0);
                font.Formatting = (FontFormatting)format;
                return font;
            }
            set
            {
                if (value != Font)
                {
                    textBlock.FontFamily = new FontFamily(value.Name);
                    textBlock.FontSize = value.Size;
                    textBlock.FontStyle = (value.Formatting & FontFormatting.Italic) != 0 ? FontStyles.Italic : FontStyles.Normal;
                    textBlock.FontWeight = (value.Formatting & FontFormatting.Bold) != 0 ? FontWeights.Bold : FontWeights.Normal;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Font"));
                    }
                }
            }
        }

        public string Text
        {
            get { return textBlock.Text; }
            set
            {
                value = value ?? string.Empty;
                if (value != textBlock.Text)
                {
                    textBlock.Text = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Text"));
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

        private TextBlock textBlock;

        public SectionFooter()
        {
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            Children.Add((textBlock = new TextBlock()
            {
                Margin = new Thickness(6, 6, 6, 11),
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            }));

            Loaded += (o, e) =>
            {
                var control = Parent as ContentControl;
                if (control != null)
                {
                    control.IsTabStop = false;
                }
            };
        }
    }
}
