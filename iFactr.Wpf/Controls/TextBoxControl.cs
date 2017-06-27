using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace iFactr.Wpf
{
    public abstract class TextBoxControl : System.Windows.Controls.TextBox, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Placeholder
        {
            get { return placeholder; }
            set
            {
                value = value ?? string.Empty;
                if (value != placeholder)
                {
                    placeholder = value;
                    WatermarkService.SetWatermark(this, GetPlaceholderBlock());

                    OnPropertyChanged("Placeholder");
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
                    WatermarkService.SetWatermark(this, GetPlaceholderBlock());

                    OnPropertyChanged("PlaceholderColor");
                }
            }
        }
        private UI.Color placeholderColor = new UI.Color(92, 0, 0, 0);

        public new string Text
        {
            get { return base.Text; }
            set { base.Text = value ?? string.Empty; }
        }

        public TextBoxControl()
            : base()
        {
            Foreground = Brushes.Black;
            placeholder = string.Empty;
        }

        protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            if (e.OldFocus == null)
            {
                SelectionStart = Text == null ? 0 : Text.Length;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private TextBlock GetPlaceholderBlock()
        {
            return new TextBlock()
            {
                FontFamily = this.FontFamily,
                FontSize = this.FontSize,
                FontStyle = this.FontStyle,
                FontWeight = this.FontWeight,
                Foreground = placeholderColor.GetBrush(),
                Text = placeholder ?? string.Empty,
                TextWrapping = this.TextWrapping
            };
        }
    }
}
