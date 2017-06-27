using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iFactr.Wpf
{
    /// <summary>
    /// Interaction logic for ButtonBase.xaml
    /// </summary>
    public partial class ButtonBase : System.Windows.Controls.Button
    {
        private const byte HighlightBorderAlpha = 200;
        private const byte HighlightFillAlpha = 28;
        private const byte SelectBorderAlpha = 255;
        private const byte SelectFillAlpha = 90;

        public Color HighlightColor
        {
            get { return highlightColor; }
            set
            {
                highlightColor = value;
                if (IsPressed)
                {
                    var border = GetTemplateChild("border") as Border;
                    if (border != null)
                    {
                        value.A = SelectBorderAlpha;
                        border.BorderBrush = new SolidColorBrush(value);

                        value.A = SelectFillAlpha;
                        border.Background = new SolidColorBrush(value);
                    }
                }
                else if (IsMouseOver)
                {
                    var border = GetTemplateChild("border") as Border;
                    if (border != null)
                    {
                        value.A = HighlightBorderAlpha;
                        border.BorderBrush = new SolidColorBrush(value);

                        value.A = HighlightFillAlpha;
                        border.Background = new SolidColorBrush(value);
                    }
                }
            }
        }
        private Color highlightColor;

        public ButtonBase()
        {
            InitializeComponent();
            HighlightColor = Color.FromRgb(145, 200, 255);
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (!IsPressed)
            {
                var border = GetTemplateChild("border") as Border;
                if (border != null)
                {
                    var value = highlightColor;
                    value.A = HighlightBorderAlpha;
                    border.BorderBrush = new SolidColorBrush(value);

                    value.A = HighlightFillAlpha;
                    border.Background = new SolidColorBrush(value);
                }

                var background = GetTemplateChild("background") as Border;
                if (background != null)
                {
                    background.BorderBrush = null;
                }
            }
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (!IsKeyboardFocused)
            {
                var border = GetTemplateChild("border") as Border;
                if (border != null)
                {
                    if (IsPressed)
                    {
                        var value = highlightColor;
                        value.A = SelectBorderAlpha;
                        border.BorderBrush = new SolidColorBrush(value);

                        value.A = SelectFillAlpha;
                        border.Background = new SolidColorBrush(value);
                    }
                    else
                    {
                        border.BorderBrush = null;
                        border.Background = null;
                    }

                    var background = GetTemplateChild("background") as Border;
                    if (background != null)
                    {
                        background.BorderBrush = border.BorderBrush == null ? BorderBrush : null;
                    }
                }
            }
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            if (!IsPressed)
            {
                var border = GetTemplateChild("border") as Border;
                if (border != null)
                {
                    var value = highlightColor; 
                    value.A = HighlightBorderAlpha;
                    border.BorderBrush = new SolidColorBrush(value);

                    value.A = HighlightFillAlpha;
                    border.Background = new SolidColorBrush(value);
                }

                var background = GetTemplateChild("background") as Border;
                if (background != null)
                {
                    background.BorderBrush = null;
                }
            }
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            if (!IsMouseOver)
            {
                var border = GetTemplateChild("border") as Border;
                if (border != null)
                {
                    if (IsPressed)
                    {
                        var value = highlightColor;
                        value.A = SelectBorderAlpha;
                        border.BorderBrush = new SolidColorBrush(value);

                        value.A = SelectFillAlpha;
                        border.Background = new SolidColorBrush(value);
                    }
                    else
                    {
                        border.BorderBrush = null;
                        border.Background = null;
                    }

                    var background = GetTemplateChild("background") as Border;
                    if (background != null)
                    {
                        background.BorderBrush = border.BorderBrush == null ? BorderBrush : null;
                    }
                }
            }
        }

        protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsPressedChanged(e);

            var border = GetTemplateChild("border") as Border;
            if (border != null)
            {
                if (IsPressed)
                {
                    var value = highlightColor;
                    value.A = SelectBorderAlpha;
                    border.BorderBrush = new SolidColorBrush(value);

                    value.A = SelectFillAlpha;
                    border.Background = new SolidColorBrush(value);
                }
                else if (IsMouseOver)
                {
                    var value = highlightColor;
                    value.A = HighlightBorderAlpha;
                    border.BorderBrush = new SolidColorBrush(value);

                    value.A = HighlightFillAlpha;
                    border.Background = new SolidColorBrush(value);
                }
                else
                {
                    border.BorderBrush = null;
                    border.Background = null;
                }

                var background = GetTemplateChild("background") as Border;
                if (background != null)
                {
                    background.BorderBrush = border.BorderBrush == null ? BorderBrush : null;
                }
            }
        }
    }
}
