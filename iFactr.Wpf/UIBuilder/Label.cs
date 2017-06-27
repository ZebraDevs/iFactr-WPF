using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using iFactr.UI;
using iFactr.UI.Controls;

namespace iFactr.Wpf
{
    public class Label : TextBlock, ILabel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event ValidationEventHandler Validating;

        public event ValueChangedEventHandler<string> ValueChanged;

        //public UI.Color BorderColor
        //{
        //    get { return base.BorderBrush.GetColor(); }
        //    set
        //    {
        //        if (value != BorderColor)
        //        {
        //            base.BorderBrush = value.IsDefaultColor ? null : value.GetBrush();

        //            var handler = PropertyChanged;
        //            if (handler != null)
        //            {
        //                handler(this, new PropertyChangedEventArgs("BorderColor"));
        //            }
        //        }
        //    }
        //}

        //public UI.Thickness BorderThickness
        //{
        //    get { return base.BorderThickness.GetThickness(); }
        //    set
        //    {
        //        var thickness = value.GetThickness();
        //        if (thickness != base.BorderThickness)
        //        {
        //            base.BorderThickness = thickness;

        //            var handler = PropertyChanged;
        //            if (handler != null)
        //            {
        //                handler(this, new PropertyChangedEventArgs("BorderThickness"));
        //            }
        //        }
        //    }
        //}

        public Font Font
        {
            get
            {
                var font = new Font(FontFamily.Source, FontSize);
                int format = (FontStyle == FontStyles.Italic ? 2 : 0) + (FontWeight.ToOpenTypeWeight() == FontWeights.Bold.ToOpenTypeWeight() ? 1 : 0);
                font.Formatting = (FontFormatting)format;
                return font;
            }
            set
            {
                if (value != Font)
                {
                    FontFamily = new FontFamily(value.Name);
                    FontSize = value.Size;
                    FontStyle = (value.Formatting & FontFormatting.Italic) != 0 ? FontStyles.Italic : FontStyles.Normal;
                    FontWeight = (value.Formatting & FontFormatting.Bold) != 0 ? FontWeights.Bold : FontWeights.Normal;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Font"));
                    }
                }
            }
        }

        public UI.Color ForegroundColor
        {
            get { return foregroundColor; }
            set
            {
                value = value.IsDefaultColor ? UI.Color.Black : value;
                if (value != foregroundColor)
                {
                    foregroundColor = value;
                    Foreground = value.GetBrush();

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("ForegroundColor"));
                    }
                }
            }
        }
        private UI.Color foregroundColor = UI.Color.Black;

        public UI.Color HighlightColor
        {
            get { return highlightColor; }
            set
            {
                if (value != highlightColor)
                {
                    highlightColor = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("HighlightColor"));
                    }
                }
            }
        }
        private UI.Color highlightColor;

        public new UI.Thickness Margin
        {
            get { return margin; }
            set
            {
                if (value != margin)
                {
                    margin = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Margin"));
                    }

                    var parent = this.GetParent<IPairable>() as FrameworkElement;
                    if (parent != null)
                    {
                        parent.InvalidateMeasure();
                    }
                }
            }
        }
        private UI.Thickness margin;

        public MetadataCollection Metadata
        {
            get { return metadata ?? (metadata = new MetadataCollection()); }
        }
        private MetadataCollection metadata;

        public new bool IsEnabled
        {
            get { return base.IsEnabled; }
            set
            {
                if (value != base.IsEnabled)
                {
                    base.IsEnabled = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("IsEnabled"));
                    }
                }
            }
        }

        public string ID
        {
            get { return id; }
            set
            {
                if (value != id)
                {
                    id = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("ID"));
                    }
                }
            }
        }
        private string id;

        public new object Parent
        {
            get
            {
                var parent = this.GetParent<IPairable>();
                return parent == null ? null : (parent.Pair ?? parent);
            }
        }

        public int ColumnIndex
        {
            get { return columnIndex; }
            set
            {
                if (value != columnIndex)
                {
                    columnIndex = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("ColumnIndex"));
                    }

                    var parent = this.GetParent<IPairable>() as FrameworkElement;
                    if (parent != null)
                    {
                        parent.InvalidateMeasure();
                    }
                }
            }
        }
        private int columnIndex;

        public int ColumnSpan
        {
            get { return columnSpan; }
            set
            {
                if (value != columnSpan)
                {
                    columnSpan = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("ColumnSpan"));
                    }

                    var parent = this.GetParent<IPairable>() as FrameworkElement;
                    if (parent != null)
                    {
                        parent.InvalidateMeasure();
                    }
                }
            }
        }
        private int columnSpan;

        public int RowIndex
        {
            get { return rowIndex; }
            set
            {
                if (value != rowIndex)
                {
                    rowIndex = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("RowIndex"));
                    }

                    var parent = this.GetParent<IPairable>() as FrameworkElement;
                    if (parent != null)
                    {
                        parent.InvalidateMeasure();
                    }
                }
            }
        }
        private int rowIndex;

        public int RowSpan
        {
            get { return rowSpan; }
            set
            {
                if (value != rowSpan)
                {
                    rowSpan = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("RowSpan"));
                    }

                    var parent = this.GetParent<IPairable>() as FrameworkElement;
                    if (parent != null)
                    {
                        parent.InvalidateMeasure();
                    }
                }
            }
        }
        private int rowSpan;

        public new iFactr.UI.HorizontalAlignment HorizontalAlignment
        {
            get { return (iFactr.UI.HorizontalAlignment)base.HorizontalAlignment; }
            set
            {
                if (value != HorizontalAlignment)
                {
                    base.HorizontalAlignment = (System.Windows.HorizontalAlignment)value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("HorizontalAlignment"));
                    }

                    var parent = this.GetParent<IPairable>() as FrameworkElement;
                    if (parent != null)
                    {
                        parent.InvalidateMeasure();
                    }
                }
            }
        }

        public new iFactr.UI.VerticalAlignment VerticalAlignment
        {
            get { return (iFactr.UI.VerticalAlignment)base.VerticalAlignment; }
            set
            {
                if (value != VerticalAlignment)
                {
                    base.VerticalAlignment = (System.Windows.VerticalAlignment)value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("VerticalAlignment"));
                    }

                    var parent = this.GetParent<IPairable>() as FrameworkElement;
                    if (parent != null)
                    {
                        parent.InvalidateMeasure();
                    }
                }
            }
        }

        public new UI.Visibility Visibility
        {
            get { return (UI.Visibility)base.Visibility; }
            set
            {
                if (value != Visibility)
                {
                    base.Visibility = (System.Windows.Visibility)value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Visibility"));
                    }

                    var parent = this.GetParent<IPairable>() as FrameworkElement;
                    if (parent != null)
                    {
                        parent.InvalidateMeasure();
                    }
                }
            }
        }

        public int Lines
        {
            get { return lines; }
            set
            {
                if (value != lines)
                {
                    lines = value;
                    if (lines == 1)
                    {
                        TextWrapping = TextWrapping.NoWrap;
                    }
                    else
                    {
                        TextWrapping = TextWrapping.Wrap;
                        if (lines <= 0)
                        {
                            Height = double.NaN;
                        }
                    }

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Lines"));
                    }
                }
            }
        }
        private int lines;

        public new UI.TextAlignment TextAlignment
        {
            get
            {
                switch (base.TextAlignment)
                {
                    case System.Windows.TextAlignment.Center:
                        return UI.TextAlignment.Center;
                    case System.Windows.TextAlignment.Justify:
                        return UI.TextAlignment.Justified;
                    case System.Windows.TextAlignment.Right:
                        return UI.TextAlignment.Right;
                    default:
                        return UI.TextAlignment.Left;
                }
            }
            set
            {
                if (value != TextAlignment)
                {
                    switch (value)
                    {
                        case UI.TextAlignment.Center:
                            base.TextAlignment = System.Windows.TextAlignment.Center;
                            break;
                        case UI.TextAlignment.Justified:
                            base.TextAlignment = System.Windows.TextAlignment.Justify;
                            break;
                        case UI.TextAlignment.Left:
                            base.TextAlignment = System.Windows.TextAlignment.Left;
                            break;
                        case UI.TextAlignment.Right:
                            base.TextAlignment = System.Windows.TextAlignment.Right;
                            break;
                    }

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("TextAlignment"));
                    }
                }
            }
        }

        public new string Text
        {
            get { return base.Text; }
            set
            {
                string oldValue = base.Text;
                base.Text = value ?? string.Empty;

                if (oldValue != base.Text)
                {
                    var phandler = PropertyChanged;
                    if (phandler != null)
                    {
                        phandler(this, new PropertyChangedEventArgs("Text"));
                        phandler(this, new PropertyChangedEventArgs("StringValue"));
                    }

                    var handler = ValueChanged;
                    if (handler != null)
                    {
                        handler(this, new ValueChangedEventArgs<string>(oldValue, base.Text));
                    }

                    var parent = this.GetParent<IPairable>() as FrameworkElement;
                    if (parent != null)
                    {
                        parent.InvalidateMeasure();
                    }
                }
            }
        }

        public string StringValue
        {
            get { return Text; }
        }

        public string SubmitKey
        {
            get { return submitKey; }
            set
            {
                if (value != submitKey)
                {
                    submitKey = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("SubmitKey"));
                    }
                }
            }
        }
        private string submitKey;

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

        public Label()
        {
            Foreground = Brushes.Black;
            IsHitTestVisible = false;
            TextTrimming = TextTrimming.WordEllipsis;
            TextWrapping = TextWrapping.Wrap;
        }

        public UI.Size Measure(UI.Size constraints)
        {
            if (string.IsNullOrEmpty(Text))
            {
                return UI.Size.Empty;
            }

            base.Width = double.NaN;
            base.Height = double.NaN;
            base.Measure(new System.Windows.Size(constraints.Width, constraints.Height));

            var size = new UI.Size(base.DesiredSize.Width, base.DesiredSize.Height);

            if (lines > 0)
            {
                string measure = "M";
                for (int i = 1; i < lines; i++)
                {
                    measure += "\nM";
                }

                string originalValue = base.Text;
                base.Text = measure;
                base.Measure(new System.Windows.Size(constraints.Width, constraints.Height));
                size.Height = Math.Min(size.Height, base.DesiredSize.Height);
                base.Text = originalValue;
            }

            return size;
        }

        public void SetLocation(UI.Point location, UI.Size size)
        {
            Width = Math.Ceiling(size.Width);
            Height = Math.Ceiling(size.Height);
            Canvas.SetLeft(this, location.X);
            Canvas.SetTop(this, location.Y);
        }

        public void NullifyEvents()
        {
            Validating = null;
            ValueChanged = null;
        }

        public bool Validate(out string[] errors)
        {
            var handler = Validating;
            if (handler != null)
            {
                var args = new ValidationEventArgs(SubmitKey, Text, StringValue);
                handler(Pair ?? this, args);

                if (args.Errors.Count > 0)
                {
                    errors = new string[args.Errors.Count];
                    args.Errors.CopyTo(errors, 0);
                    return false;
                }
            }

            errors = null;
            return true;
        }

        public bool Equals(IElement other)
        {
            var control = other as Element;
            if (control != null)
            {
                return control.Equals(this);
            }

            return base.Equals(other);
        }
    }
}
