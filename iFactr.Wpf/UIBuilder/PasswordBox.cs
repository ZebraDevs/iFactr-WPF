using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using iFactr.UI;
using iFactr.UI.Controls;

namespace iFactr.Wpf
{
    public class PasswordBox : IPasswordBox, ISealedElement, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler GotFocus;

        public event EventHandler LostFocus;

        public event ValidationEventHandler Validating;

        public event EventHandler<EventHandledEventArgs> ReturnKeyPressed;

        public event ValueChangedEventHandler<string> PasswordChanged;

        public UI.Color BackgroundColor
        {
            get { return passwordBox.Background.GetColor(); }
            set
            {
                if (value != BackgroundColor)
                {
                    passwordBox.Background = value.IsDefaultColor ? null : value.GetBrush();

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("BackgroundColor"));
                    }
                }
            }
        }

        //public UI.Color BorderColor
        //{
        //    get { return passwordBox.BorderBrush.GetColor(); }
        //    set
        //    {
        //        if (value != BorderColor)
        //        {
        //            passwordBox.BorderBrush = value.IsDefaultColor ? null : value.GetBrush();

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
        //    get { return passwordBox.BorderThickness.GetThickness(); }
        //    set
        //    {
        //        var thickness = value.GetThickness();
        //        if (thickness != passwordBox.BorderThickness)
        //        {
        //            passwordBox.BorderThickness = thickness;

        //            var handler = PropertyChanged;
        //            if (handler != null)
        //            {
        //                handler(this, new PropertyChangedEventArgs("BorderThickness"));
        //            }
        //        }
        //    }
        //}

        public UI.Color ForegroundColor
        {
            get { return passwordBox.Foreground.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? UI.Color.Black : value;
                if (value != ForegroundColor)
                {
                    passwordBox.Foreground = value.GetBrush();

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("ForegroundColor"));
                    }
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

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Expression"));
                    }
                }
            }
        }
        private Regex expression;

        public Font Font
        {
            get
            {
                var font = new Font(passwordBox.FontFamily.Source, (float)passwordBox.FontSize);
                int format = (passwordBox.FontStyle == FontStyles.Italic ? 2 : 0) + (passwordBox.FontWeight.ToOpenTypeWeight() == FontWeights.Bold.ToOpenTypeWeight() ? 1 : 0);
                font.Formatting = (FontFormatting)format;
                return font;
            }
            set
            {
                if (value != Font)
                {
                    passwordBox.FontFamily = new FontFamily(value.Name);
                    passwordBox.FontSize = value.Size;
                    passwordBox.FontStyle = (value.Formatting & FontFormatting.Italic) != 0 ? FontStyles.Italic : FontStyles.Normal;
                    passwordBox.FontWeight = (value.Formatting & FontFormatting.Bold) != 0 ? FontWeights.Bold : FontWeights.Normal;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Font"));
                    }
                }
            }
        }

        public bool IsEnabled
        {
            get { return passwordBox.IsEnabled; }
            set
            {
                if (value != passwordBox.IsEnabled)
                {
                    passwordBox.IsEnabled = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("IsEnabled"));
                    }
                }
            }
        }

        public bool IsFocused
        {
            get { return passwordBox.IsFocused; }
        }

        public KeyboardType KeyboardType
        {
            get { return keyboardType; }
            set
            {
                if (value != keyboardType)
                {
                    keyboardType = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("KeyboardType"));
                    }
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

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("KeyboardReturnType"));
                    }
                }
            }
        }
        private KeyboardReturnType keyboardReturnType;

        public string Placeholder
        {
            get { return placeholder; }
            set
            {
                if (value != placeholder)
                {
                    placeholder = value;
                    WatermarkService.SetWatermark(passwordBox, GetPlaceholderBlock());

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Placeholder"));
                    }
                }
            }
        }
        private string placeholder;

        public UI.Color PlaceholderColor
        {
            get { return placeholderColor; }
            set
            {
                value = value.IsDefaultColor ? new UI.Color(92, 0, 0, 0) : value;
                if (value != placeholderColor)
                {
                    placeholderColor = value;
                    WatermarkService.SetWatermark(passwordBox, GetPlaceholderBlock());

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("PlaceholderColor"));
                    }
                }
            }
        }
        private UI.Color placeholderColor = new UI.Color(92, 0, 0, 0);

        public UI.Thickness Margin
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

                    var parent = passwordBox.GetParent<IPairable>() as FrameworkElement;
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

        public object Parent
        {
            get
            {
                var parent = passwordBox.GetParent<IPairable>();
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

                    var parent = passwordBox.GetParent<IPairable>() as FrameworkElement;
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

                    var parent = passwordBox.GetParent<IPairable>() as FrameworkElement;
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

                    var parent = passwordBox.GetParent<IPairable>() as FrameworkElement;
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

                    var parent = passwordBox.GetParent<IPairable>() as FrameworkElement;
                    if (parent != null)
                    {
                        parent.InvalidateMeasure();
                    }
                }
            }
        }
        private int rowSpan;

        public iFactr.UI.HorizontalAlignment HorizontalAlignment
        {
            get { return (iFactr.UI.HorizontalAlignment)passwordBox.HorizontalAlignment; }
            set
            {
                if (value != HorizontalAlignment)
                {
                    passwordBox.HorizontalAlignment = (System.Windows.HorizontalAlignment)value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("HorizontalAlignment"));
                    }

                    var parent = passwordBox.GetParent<IPairable>() as FrameworkElement;
                    if (parent != null)
                    {
                        parent.InvalidateMeasure();
                    }
                }
            }
        }

        public iFactr.UI.VerticalAlignment VerticalAlignment
        {
            get { return (iFactr.UI.VerticalAlignment)passwordBox.VerticalAlignment; }
            set
            {
                if (value != VerticalAlignment)
                {
                    passwordBox.VerticalAlignment = (System.Windows.VerticalAlignment)value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("VerticalAlignment"));
                    }

                    var parent = passwordBox.GetParent<IPairable>() as FrameworkElement;
                    if (parent != null)
                    {
                        parent.InvalidateMeasure();
                    }
                }
            }
        }

        public UI.Visibility Visibility
        {
            get { return (UI.Visibility)passwordBox.Visibility; }
            set
            {
                if (value != Visibility)
                {
                    passwordBox.Visibility = (System.Windows.Visibility)value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Visibility"));
                    }

                    var parent = passwordBox.GetParent<IPairable>() as FrameworkElement;
                    if (parent != null)
                    {
                        parent.InvalidateMeasure();
                    }
                }
            }
        }

        public string Password
        {
            get { return passwordBox.Password; }
            set { passwordBox.Password = value ?? string.Empty; }
        }

        public string StringValue
        {
            get { return passwordBox.Password; }
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

        public FrameworkElement Element
        {
            get { return passwordBox; }
        }

        private string currentValue = string.Empty;
        private System.Windows.Controls.PasswordBox passwordBox;
        private bool setFocusOnLoad;

        public PasswordBox()
        {
            passwordBox = new System.Windows.Controls.PasswordBox();
            passwordBox.Padding = new System.Windows.Thickness(5);
            passwordBox.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            passwordBox.Tag = this;
            passwordBox.PasswordChanged += (o, e) =>
            {
                if (passwordBox.Password == currentValue)
                {
                    // password boxes have a peculiar behavior where the cursor is pushed to the beginning
                    // when used in a two-way binding.  to compensate, we need to force the cursor to the end.
                    var select = passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (select != null)
                    {
                        select.Invoke(passwordBox, new object[] { currentValue.Length, 0 });
                    }
                    return;
                }

                if (!(expression == null || expression.IsMatch(passwordBox.Password)))
                {
                    passwordBox.Password = currentValue;
                    var select = passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (select != null)
                    {
                        select.Invoke(passwordBox, new object[] { currentValue.Length, 0 });
                    }
                    return;
                }

                string oldValue = currentValue;
                currentValue = passwordBox.Password;

                var phandler = PropertyChanged;
                if (phandler != null)
                {
                    phandler(this, new PropertyChangedEventArgs("Password"));
                    phandler(this, new PropertyChangedEventArgs("StringValue"));
                }

                var handler = PasswordChanged;
                if (handler != null)
                {
                    handler(Pair ?? this, new ValueChangedEventArgs<string>(oldValue, currentValue));
                }
            };

            passwordBox.PreviewKeyDown += (o, e) =>
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

            passwordBox.Loaded += (o, e) =>
            {
                if (setFocusOnLoad)
                {
                    passwordBox.Focus();
                }
            };

            passwordBox.GotFocus += (o, e) =>
            {
                var phandler = PropertyChanged;
                if (phandler != null)
                {
                    phandler(this, new PropertyChangedEventArgs("IsFocused"));
                }

                var handler = GotFocus;
                if (handler != null)
                {
                    handler(pair ?? this, EventArgs.Empty);
                }
            };

            passwordBox.LostFocus += (o, e) =>
            {
                var phandler = PropertyChanged;
                if (phandler != null)
                {
                    phandler(this, new PropertyChangedEventArgs("IsFocused"));
                }

                var handler = LostFocus;
                if (handler != null)
                {
                    handler(pair ?? this, EventArgs.Empty);
                }
            };
        }

        public void Focus()
        {
            setFocusOnLoad = (!passwordBox.Focus() && !passwordBox.IsLoaded);
        }

        public UI.Size Measure(UI.Size constraints)
        {
            double width = passwordBox.Width;
            double height = passwordBox.Height;
            passwordBox.Width = double.NaN;
            passwordBox.Height = double.NaN;

            passwordBox.Measure(new System.Windows.Size(constraints.Width, constraints.Height));

            passwordBox.Width = width;
            passwordBox.Height = height;
            return new UI.Size(passwordBox.DesiredSize.Width, passwordBox.DesiredSize.Height);
        }

        public void SetLocation(UI.Point location, UI.Size size)
        {
            passwordBox.Width = size.Width;
            passwordBox.Height = size.Height;
            Canvas.SetLeft(passwordBox, location.X);
            Canvas.SetTop(passwordBox, location.Y);
        }

        public void NullifyEvents()
        {
            Validating = null;
            GotFocus = null;
            LostFocus = null;
            ReturnKeyPressed = null;
            PasswordChanged = null;
        }

        public bool Validate(out string[] errors)
        {
            var handler = Validating;
            if (handler != null)
            {
                var args = new ValidationEventArgs(SubmitKey, Password, StringValue);
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

        private TextBlock GetPlaceholderBlock()
        {
            return new TextBlock()
            {
                FontFamily = passwordBox.FontFamily,
                FontSize = passwordBox.FontSize,
                FontStyle = passwordBox.FontStyle,
                FontWeight = passwordBox.FontWeight,
                Foreground = placeholderColor.GetBrush(),
                Text = placeholder ?? string.Empty,
            };
        }
    }

    public interface ISealedElement
    {
        FrameworkElement Element { get; }
    }
}
