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
    public class SectionHeader : System.Windows.Controls.Grid, ISectionHeader, INotifyPropertyChanged
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
                    line.Stroke = textBlock.Foreground;

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
            get { return text; }
            set
            {
                if (value != text)
                {
                    text = value;
                    textBlock.Text = value ?? string.Empty;
                    line.StrokeThickness = value == null ? 0 : 0.5;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Text"));
                    }
                }
            }
        }
        private string text;

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
        private Line line;

        public SectionHeader()
        {
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            Children.Add((textBlock = new TextBlock()
            {
                Margin = new Thickness(6),
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            }));

            line = new Line();
            line.UseLayoutRounding = true;
            line.StrokeThickness = 0.5;
            line.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            line.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            line.Margin = new Thickness(0, 8, 0, 6);
            line.X1 = 0;
            Children.Add(line);

            System.Windows.Controls.Grid.SetColumn(line, 1);

            Loaded += (o, e) =>
            {
                var control = Parent as ContentControl;
                if (control != null)
                {
                    control.IsTabStop = false;
                }
            };
        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size arrangeSize)
        {
            var size = base.ArrangeOverride(arrangeSize);

            line.X2 = ColumnDefinitions[1].ActualWidth;

            return size;
        }
    }
}
