using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using iFactr.UI;
using iFactr.UI.Controls;

namespace iFactr.Wpf
{
    public class Switch : CheckBox, ISwitch, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event ValidationEventHandler Validating;

        public event ValueChangedEventHandler<bool> ValueChanged;

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

        public Color FalseColor
        {
            get { return falseColor; }
            set
            {
                if (value != falseColor)
                {
                    falseColor = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("FalseColor"));
                    }

                    if (!IsChecked.Value)
                    {
                        Background = value.IsDefaultColor ? null : value.GetBrush();
                    }
                }
            }
        }
        private Color falseColor;

        public Color ForegroundColor
        {
            get { return foregroundColor; }
            set
            {
                if (value != foregroundColor)
                {
                    foregroundColor = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("ForegroundColor"));
                    }
                }
            }
        }
        private Color foregroundColor;

        public Color TrueColor
        {
            get { return trueColor; }
            set
            {
                if (value != trueColor)
                {
                    trueColor = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("TrueColor"));
                    }

                    if (IsChecked.Value)
                    {
                        Background = value.IsDefaultColor ? null : value.GetBrush();
                    }
                }
            }
        }
        private Color trueColor;

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

        public bool Value
        {
            get { return IsChecked.Value; }
            set { IsChecked = value; }
        }

        public string StringValue
        {
            get { return IsChecked.Value ? "true" : "false"; }
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

        public Switch()
        {
        }

        public UI.Size Measure(UI.Size constraints)
        {
            double width = Width;
            double height = Height;
            Width = double.NaN;
            Height = double.NaN;

            base.Measure(new System.Windows.Size(constraints.Width, constraints.Height));

            Width = width;
            Height = height;
            return new UI.Size(DesiredSize.Width, DesiredSize.Height);
        }

        public void SetLocation(UI.Point location, UI.Size size)
        {
            Width = size.Width;
            Height = size.Height;
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
                var args = new ValidationEventArgs(SubmitKey, Value, StringValue);
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

        protected override void OnChecked(System.Windows.RoutedEventArgs e)
        {
            base.OnChecked(e);

            var phandler = PropertyChanged;
            if (phandler != null)
            {
                phandler(this, new PropertyChangedEventArgs("Value"));
                phandler(this, new PropertyChangedEventArgs("StringValue"));
            }

            var handler = ValueChanged;
            if (handler != null)
            {
                handler(Pair ?? this, new ValueChangedEventArgs<bool>(!Value, Value));
            }

            Background = trueColor.IsDefaultColor ? null : trueColor.GetBrush();
        }

        protected override void OnUnchecked(System.Windows.RoutedEventArgs e)
        {
            base.OnUnchecked(e);

            var phandler = PropertyChanged;
            if (phandler != null)
            {
                phandler(this, new PropertyChangedEventArgs("Value"));
                phandler(this, new PropertyChangedEventArgs("StringValue"));
            }

            var handler = ValueChanged;
            if (handler != null)
            {
                handler(Pair ?? this, new ValueChangedEventArgs<bool>(!Value, Value));
            }

            Background = falseColor.IsDefaultColor ? null : falseColor.GetBrush();
        }
    }
}
