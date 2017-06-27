using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using iFactr.UI;
using iFactr.UI.Controls;

namespace iFactr.Wpf
{
    public class TextBox : TextBoxControl, ITextBox
    {
        public event ValidationEventHandler Validating;

        public new event EventHandler GotFocus;

        public new event EventHandler LostFocus;

        public event EventHandler<EventHandledEventArgs> ReturnKeyPressed;

        public new event ValueChangedEventHandler<string> TextChanged;

        public UI.Color BackgroundColor
        {
            get { return base.Background.GetColor(); }
            set
            {
                if (value != BackgroundColor)
                {
                    base.Background = value.IsDefaultColor ? null : value.GetBrush();
                    OnPropertyChanged("BackgroundColor");
                }
            }
        }

        //public UI.Color BorderColor
        //{
        //    get { return base.BorderBrush.GetColor(); }
        //    set
        //    {
        //        if (value != BorderColor)
        //        {
        //            base.BorderBrush = value.IsDefaultColor ? null : value.GetBrush();
        //            OnPropertyChanged("BorderColor");
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
        //            OnPropertyChanged("BorderThickness");
        //        }
        //    }
        //}

        public UI.Color ForegroundColor
        {
            get { return base.Foreground.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? UI.Color.Black : value;
                if (value != ForegroundColor)
                {
                    base.Foreground = value.GetBrush();
                    OnPropertyChanged("ForegroundColor");
                }
            }
        }

        public string Expression
        {
            get { return expression == null ? null : expression.ToString(); }
            set
            {
                if (value != Expression)
                {
                    expression = value == null ? null : new Regex(value);
                    OnPropertyChanged("Expression");
                }
            }
        }
        private Regex expression;

        public Font Font
        {
            get
            {
                var font = new Font(FontFamily.Source, (float)FontSize);
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

                    OnPropertyChanged("Font");
                }
            }
        }

        public KeyboardType KeyboardType
        {
            get { return keyboardType; }
            set
            {
                if (value != keyboardType)
                {
                    keyboardType = value;
                    OnPropertyChanged("KeyboardType");
                }
            }
        }
        private KeyboardType keyboardType;

        public KeyboardReturnType KeyboardReturnType
        {
            get { return keyboardReturnType; }
            set
            {
                if (value != keyboardReturnType)
                {
                    keyboardReturnType = value;
                    OnPropertyChanged("KeyboardReturnType");
                }
            }
        }
        private KeyboardReturnType keyboardReturnType;

        public new UI.Thickness Margin
        {
            get { return margin; }
            set
            {
                if (value != margin)
                {
                    margin = value;
                    OnPropertyChanged("Margin");

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
                    OnPropertyChanged("IsEnabled");
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
                    OnPropertyChanged("ID");
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

                    OnPropertyChanged("ColumnIndex");

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

                    OnPropertyChanged("ColumnSpan");

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

                    OnPropertyChanged("RowIndex");

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

                    OnPropertyChanged("RowSpan");

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

                    OnPropertyChanged("HorizontalAlignment");

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

                    OnPropertyChanged("VerticalAlignment");

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

                    OnPropertyChanged("Visibility");

                    var parent = this.GetParent<IPairable>() as FrameworkElement;
                    if (parent != null)
                    {
                        parent.InvalidateMeasure();
                    }
                }
            }
        }

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

                    OnPropertyChanged("TextAlignment");
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
                    OnPropertyChanged("SubmitKey");
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

        private string currentValue = string.Empty;
        private bool setFocusOnLoad;

        public TextBox()
        {
            Padding = new System.Windows.Thickness(5);
            base.TextChanged += (o, e) =>
            {
                if (base.Text != currentValue && !(expression == null || expression.IsMatch(base.Text)))
                {
                    base.Text = currentValue;
                    base.Select(currentValue.Length, 0);
                    return;
                }

                string oldValue = currentValue;
                currentValue = base.Text;

                OnPropertyChanged("Text");
                OnPropertyChanged("StringValue");

                var handler = TextChanged;
                if (handler != null)
                {
                    handler(Pair ?? this, new ValueChangedEventArgs<string>(oldValue, currentValue));
                }
            };

            PreviewKeyDown += (o, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Return)
                {
                    var handler = ReturnKeyPressed;
                    if (handler != null)
                    {
                        var args = new EventHandledEventArgs();
                        handler(pair ?? this, args);
                        e.Handled = args.IsHandled;
                    }
                }
            };

            Loaded += (o, e) =>
            {
                if (setFocusOnLoad)
                {
                    base.Focus();
                }
            };
        }

        public new void Focus()
        {
            setFocusOnLoad = (!base.Focus() && !IsLoaded);
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
            GotFocus = null;
            LostFocus = null;
            ReturnKeyPressed = null;
            TextChanged = null;
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

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            OnPropertyChanged("IsFocused");

            var handler = GotFocus;
            if (handler != null)
            {
                handler(pair ?? this, EventArgs.Empty);
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            OnPropertyChanged("IsFocused");

            var handler = LostFocus;
            if (handler != null)
            {
                handler(pair ?? this, EventArgs.Empty);
            }
        }
    }
}
