using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using iFactr.UI;
using iFactr.UI.Controls;

namespace iFactr.Wpf
{
    public partial class TimePicker : System.Windows.Controls.DatePicker, ITimePicker, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TimeFormatProperty = DependencyProperty.Register("TimeFormat", typeof(string), typeof(TimePicker), new PropertyMetadata(default(string)));

        public event PropertyChangedEventHandler PropertyChanged;

        public event ValidationEventHandler Validating;

        public event ValueChangedEventHandler<DateTime?> TimeChanged;

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

        public UI.Color ForegroundColor
        {
            get { return base.Foreground.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? new UI.Color(255, 51, 51, 51) : value;
                if (value != ForegroundColor)
                {
                    base.Foreground = value.GetBrush();

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

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Font"));
                    }
                }
            }
        }

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

        public DateTime? Time
        {
            get { return SelectedDate; }
            set
            {
                // the date picker has some peculiar behavior where the time is stripped out when the date changes.
                // if the date doesn't change, then the time is kept.
                // to work around this, we're setting the date twice to ensure that the time is kept like it should be.
                SelectedDate = value;
                SelectedDate = value;
            }
        }

        public string TimeFormat
        {
            get { return GetValue(TimeFormatProperty) as string; }
            set
            {
                value = value ?? "t";
                if (value != TimeFormat)
                {
                    SetValue(TimeFormatProperty, value);

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("TimeFormat"));
                    }
                }
            }
        }

        public string StringValue
        {
            get
            {
                if (!Time.HasValue)
                {
                    return string.Empty;
                }

                return Time.Value.ToString(TimeFormat ?? "t");
            }
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

        private DateTime? currentValue;

        public TimePicker()
        {
            InitializeComponent();

            MinWidth = 128;
            Time = DateTime.Now;
            TimeFormat = null;
        }

        public void ShowPicker()
        {
            base.IsDropDownOpen = true;
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
            TimeChanged = null;
        }

        public bool Validate(out string[] errors)
        {
            var handler = Validating;
            if (handler != null)
            {
                var args = new ValidationEventArgs(SubmitKey, Time, StringValue);
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

        protected override void OnSelectedDateChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectedDateChanged(e);

            var pHandler = PropertyChanged;
            if (pHandler != null)
            {
                pHandler(this, new PropertyChangedEventArgs("Time"));
                pHandler(this, new PropertyChangedEventArgs("StringValue"));
            }

            var handler = TimeChanged;
            if (handler != null)
            {
                handler(Pair ?? this, new ValueChangedEventArgs<DateTime?>(currentValue, SelectedDate));
            }

            currentValue = SelectedDate;
        }
    }

    public class TimeSelector : System.Windows.Controls.ContentControl
    {
        private const double ListWidth = 72;
        private ListBox hours, minutes, designators;
        private bool is24;

        public TimeSelector()
        {
            MaxHeight = 256;

            is24 = CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern.Contains("H");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (hours == null)
            {
                hours = new ListBox() { MinWidth = ListWidth };
                hours.SelectionChanged += TimeSelected;
            }
            else
            {
                hours.Items.Clear();
            }

            for (int i = 0; i < (is24 ? 24 : 12); i++)
            {
                hours.Items.Add((!is24 && i == 0) ? "12" : i.ToString());
            }

            if (minutes == null)
            {
                minutes = new ListBox() { MinWidth = ListWidth };
                minutes.SelectionChanged += TimeSelected;
            }
            else
            {
                minutes.Items.Clear();
            }

            for (int i = 0; i < 60; i++)
            {
                minutes.Items.Add(string.Format("{0}{1}", i < 10 ? "0" : string.Empty, i.ToString()));
            }

            if (designators == null)
            {
                designators = new ListBox() { MinWidth = ListWidth };
                designators.SelectionChanged += TimeSelected;
            }
            else
            {
                designators.Items.Clear();
            }

            if (is24)
            {
                designators.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                designators.Visibility = System.Windows.Visibility.Visible;
                designators.Items.Add(CultureInfo.CurrentUICulture.DateTimeFormat.AMDesignator);
                designators.Items.Add(CultureInfo.CurrentUICulture.DateTimeFormat.PMDesignator);
            }

            Content = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Children = { hours, minutes, designators }
            };
        }

        private void TimeSelected(object sender, SelectionChangedEventArgs e)
        {
            var calendar = this.GetParent<System.Windows.Controls.Calendar>();
            if (calendar != null)
            {
                int hour = Math.Max(hours.SelectedIndex, 0);
                if (!is24 && designators.SelectedIndex == 1)
                {
                    hour += 12;
                }

                int minute = Math.Max(minutes.SelectedIndex, 0);
                calendar.SelectedDate = new DateTime(1, 1, 1, hour, minute, 0);
            }
        }
    }
}
