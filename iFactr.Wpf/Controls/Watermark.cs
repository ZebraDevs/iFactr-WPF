using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace iFactr.Wpf
{
    /// <summary>
    /// Class that provides the Watermark attached property
    /// </summary>
    internal static class WatermarkService
    {
        /// <summary>
        /// Watermark Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
           "Watermark",
           typeof(object),
           typeof(WatermarkService),
           new FrameworkPropertyMetadata((object)null, new PropertyChangedCallback(OnWatermarkChanged)));

        #region Private Fields

        /// <summary>
        /// Dictionary of ItemsControls
        /// </summary>
        private static readonly Dictionary<object, ItemsControl> itemsControls = new Dictionary<object, ItemsControl>();

        #endregion

        /// <summary>
        /// Gets the Watermark property.  This dependency property indicates the watermark for the control.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/> to get the property from</param>
        /// <returns>The value of the Watermark property</returns>
        public static object GetWatermark(DependencyObject d)
        {
            return (object)d.GetValue(WatermarkProperty);
        }

        /// <summary>
        /// Sets the Watermark property.  This dependency property indicates the watermark for the control.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/> to set the property on</param>
        /// <param name="value">value of the property</param>
        public static void SetWatermark(DependencyObject d, object value)
        {
            d.SetValue(WatermarkProperty, value);
        }

        /// <summary>
        /// Handles changes to the Watermark property.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/> that fired the event</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control control = (Control)d;
            control.Loaded -= Control_Loaded;
            control.Loaded += Control_Loaded;

            control.GotKeyboardFocus -= Control_GotKeyboardFocus;
            control.GotKeyboardFocus += Control_GotKeyboardFocus;

            control.LostKeyboardFocus -= Control_Loaded;
            control.LostKeyboardFocus += Control_Loaded;

            var textBox = d as System.Windows.Controls.TextBox;
            if (textBox != null)
            {
                textBox.TextChanged -= TextBox_TextChanged;
                textBox.TextChanged += TextBox_TextChanged;
            }
            else
            {
                var pwBox = d as System.Windows.Controls.PasswordBox;
                if (pwBox != null)
                {
                    pwBox.PasswordChanged -= PasswordBox_PasswordChanged;
                    pwBox.PasswordChanged += PasswordBox_PasswordChanged;
                }
            }

            RemoveWatermark(d as UIElement);
            if (ShouldShowWatermark(control))
            {
                ShowWatermark(control);
            }
        }

        static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var pwBox = sender as System.Windows.Controls.PasswordBox;
            if (pwBox != null && !pwBox.IsKeyboardFocused)
            {
                if (string.IsNullOrEmpty(pwBox.Password))
                {
                    ShowWatermark(pwBox);
                }
                else
                {
                    RemoveWatermark(pwBox);
                }
            }
        }

        static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            if (textBox != null && !textBox.IsKeyboardFocused)
            {
                var change = e.Changes.FirstOrDefault();
                if (string.IsNullOrEmpty(textBox.Text) && change != null && change.RemovedLength > 0)
                {
                    ShowWatermark(textBox);
                }
                else if (change != null && change.AddedLength == textBox.Text.Length)
                {
                    RemoveWatermark(textBox);
                }
            }
        }

        /// <summary>
        /// Handle the GotFocus event on the control
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="RoutedEventArgs"/> that contains the event data.</param>
        private static void Control_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            Control c = (Control)sender;
            RemoveWatermark(c);
        }

        /// <summary>
        /// Handle the Loaded and LostFocus event on the control
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="RoutedEventArgs"/> that contains the event data.</param>
        private static void Control_Loaded(object sender, RoutedEventArgs e)
        {
            Control control = (Control)sender;
            if (ShouldShowWatermark(control))
            {
                ShowWatermark(control);
            }
        }

        /// <summary>
        /// Remove the watermark from the specified element
        /// </summary>
        /// <param name="control">Element to remove the watermark from</param>
        private static void RemoveWatermark(UIElement control)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);

            // layer could be null if control is no longer in the visual tree
            if (layer != null)
            {
                Adorner[] adorners = layer.GetAdorners(control);
                if (adorners == null)
                {
                    return;
                }

                foreach (Adorner adorner in adorners)
                {
                    if (adorner is WatermarkAdorner)
                    {
                        adorner.Visibility = Visibility.Hidden;
                        layer.Remove(adorner);
                    }
                }
            }
        }

        /// <summary>
        /// Show the watermark on the specified control
        /// </summary>
        /// <param name="control">Control to show the watermark on</param>
        private static void ShowWatermark(Control control)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);

            // layer could be null if control is no longer in the visual tree
            if (layer != null)
            {
                layer.Add(new WatermarkAdorner(control, GetWatermark(control)));
            }
        }

        /// <summary>
        /// Indicates whether or not the watermark should be shown on the specified control
        /// </summary>
        /// <param name="c"><see cref="Control"/> to test</param>
        /// <returns>true if the watermark should be shown; false otherwise</returns>
        private static bool ShouldShowWatermark(Control c)
        {
            if (System.Windows.Input.Keyboard.FocusedElement == c)
            {
                return false;
            }

            if (c is TextBoxBase)
            {
                return string.IsNullOrEmpty((c as System.Windows.Controls.TextBox).Text);
            }
            else if (c is System.Windows.Controls.PasswordBox)
            {
                return string.IsNullOrEmpty((c as System.Windows.Controls.PasswordBox).Password);
            }
            else
            {
                return false;
            }
        }
    }
}
