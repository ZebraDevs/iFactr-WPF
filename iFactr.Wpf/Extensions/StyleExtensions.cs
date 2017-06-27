using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Style = iFactr.Core.Styles.Style;

namespace iFactr.Wpf
{
    public static class StyleExtensions
    {
        public static readonly ColorConverter ColorConverter = new ColorConverter();

        public static void Skin(this FrameworkElement control, Style style)
        {
            control.SetBinding(Control.ForegroundProperty, new Binding("TextColor") { Source = style, Converter = ColorConverter, });
            control.SetBinding(Control.BackgroundProperty, new Binding("LayerItemBackgroundColor") { Source = style, Converter = ColorConverter, });
        }

        public static async void SkinLayer(this FrameworkElement control, Style style)
        {
            control.SetBinding(Control.ForegroundProperty, new Binding("TextColor") { Source = style, Converter = ColorConverter });
            if (string.IsNullOrWhiteSpace(style.LayerBackgroundImage))
            {
                control.SetBinding(Control.BackgroundProperty, new Binding("LayerBackgroundColor") { Source = style, Converter = ColorConverter, });
            }
            else
            {
                control.SetValue(Control.BackgroundProperty, new ImageBrush
                {
                    ImageSource = await WpfFactory.LoadBitmapAsync(style.LayerBackgroundImage),
                    Stretch = Stretch.UniformToFill,
                });
            }
        }

        public static void SkinSectionHeader(this FrameworkElement control, Style style)
        {
            control.SetBinding(Control.ForegroundProperty, new Binding("SectionHeaderTextColor") { Source = style, Converter = ColorConverter, });
            control.SetBinding(Control.BackgroundProperty, new Binding("SectionHeaderColor") { Source = style, Converter = ColorConverter, ConverterParameter = true, });
        }

        public static void SkinOverlay(this FrameworkElement control, Style style)
        {
            control.SetBinding(Control.ForegroundProperty, new Binding("TextColor") { Source = style, Converter = ColorConverter, });
            control.SetValue(Control.BackgroundProperty, InvisibleBrush);
        }

        public static Brush InvisibleBrush { get { return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)); } }

        public static Brush GetBrush(this UI.Color color)
        {
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        public static System.Windows.Media.Color GetColor(this UI.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static UI.Color GetColor(this System.Windows.Media.Color color)
        {
            return new UI.Color(color.A, color.R, color.G, color.B);
        }

        public static UI.Color GetColor(this Brush brush)
        {
            var solidBrush = brush as SolidColorBrush;
            return solidBrush == null ? new UI.Color() : solidBrush.Color.GetColor();
        }

        public static UI.Thickness GetThickness(this System.Windows.Thickness thickness)
        {
            return new UI.Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
        }

        public static System.Windows.Thickness GetThickness(this UI.Thickness thickness)
        {
            return new System.Windows.Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
        }

        public async static Task<ImageBrush> GetImageBrush(this string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
                return null;

            return new ImageBrush(await WpfFactory.LoadBitmapAsync(uri));
        }

        public static string GetImageUri(this Brush brush)
        {
            var imageBrush = brush as ImageBrush;
            if (imageBrush == null)
                return null;

            var source = imageBrush.ImageSource as BitmapImage;
            return source == null || source.UriSource == null ? null : source.UriSource.OriginalString;
        }
    }

    public class ColorConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var color = (UI.Color)value;
            if (parameter != null && (bool)parameter)
            {
                return new LinearGradientBrush(new GradientStopCollection
                {
                    new GradientStop
                    {
                        Color = Color.FromArgb(color.A, FindHighlight(color.R), FindHighlight(color.G), FindHighlight(color.B)),
                        Offset = 0,     
                    },
                    new GradientStop
                    {
                        Color = Color.FromArgb(color.A, color.R, color.G, color.B),
                        Offset = 1,
                    }
                }, 90);
            }
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var color = ((SolidColorBrush)value).Color;
            return new UI.Color(color.A, color.R, color.G, color.B);
        }

        private byte FindHighlight(byte colorByte)
        {
            return (byte)(colorByte + (byte.MaxValue - colorByte) / 2);
        }
    }
}