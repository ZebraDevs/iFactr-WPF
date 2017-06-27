using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using iFactr.Core;
using iFactr.Core.Layers;
using iFactr.UI;
using MonoCross.Navigation;
using Image = System.Windows.Controls.Image;
using Thickness = System.Windows.Thickness;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "iFactr.Wpf")]
namespace iFactr.Wpf
{
    [ContentProperty("Source")]
    public class InlineImage : Figure
    {
        #region DependencyProperty 'Height'

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public new FigureLength Height
        {
            get { return (FigureLength)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set the height.
        /// </summary>
        public static new readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(FigureLength), typeof(InlineImage), new FrameworkPropertyMetadata(new FigureLength(1, FigureUnitType.Auto), OnHeightChanged));

        private static void OnHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs basevalue)
        {
            var inlineImage = (InlineImage)sender;
            if (inlineImage.Height.IsAuto || inlineImage.Blocks.Count == 0) return;
            var image = (Image)((BlockUIContainer)inlineImage.Blocks.FirstBlock).Child;
            image.MaxHeight = inlineImage.Height.Value;
            inlineImage.Height = new FigureLength(1, FigureUnitType.Auto);
        }
        #endregion
        #region DependencyProperty 'Tab'

        /// <summary>
        /// Gets or sets the tab.
        /// </summary>
        public int Tab
        {
            get { return (int)GetValue(TabProperty); }
            set { SetValue(TabProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set the tab.
        /// </summary>
        public static readonly DependencyProperty TabProperty = DependencyProperty.Register("Tab", typeof(int), typeof(InlineImage));

        #endregion
        #region DependencyProperty 'Pane'

        /// <summary>
        /// Gets or sets the tab.
        /// </summary>
        public UI.Pane Pane
        {
            get { return (UI.Pane)GetValue(PaneProperty); }
            set { SetValue(PaneProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set the tab.
        /// </summary>
        public static readonly DependencyProperty PaneProperty = DependencyProperty.Register("Pane", typeof(UI.Pane), typeof(InlineImage));

        #endregion
        #region DependencyProperty 'Source'

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set the source.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(InlineImage), new FrameworkPropertyMetadata(null, OnSourceChanged));

        private static async void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var inlineImage = (InlineImage)sender;
            var bitmapImage = await WpfFactory.LoadBitmapAsync(inlineImage.Source);

            if (bitmapImage == null)
            {
                inlineImage.Height = new FigureLength(0);
                inlineImage.Width = new FigureLength(0);
                inlineImage.Margin = new Thickness(0);
                inlineImage.Padding = new Thickness(0);
                return;
            }
            var setImage = new Action<object, EventArgs>((ob, ev) =>
            {
                var image = new System.Windows.Controls.Image
                {
                    Source = bitmapImage,
                    Stretch = Stretch.Uniform,
                    StretchDirection = StretchDirection.Both,
                    Margin = new Thickness(0),
                    MaxHeight = inlineImage.Height.IsAuto ? bitmapImage.PixelHeight : inlineImage.Height.Value,
                    MaxWidth = inlineImage.Width.IsAuto ? bitmapImage.PixelWidth : inlineImage.Width.Value,
                };

                inlineImage.Width = new FigureLength(image.MaxWidth, FigureUnitType.Pixel);
                inlineImage.Blocks.Clear();
                inlineImage.Blocks.Add(new BlockUIContainer { Child = image, Padding = new Thickness(0), Margin = new Thickness(0), });
                inlineImage.ToolTip = inlineImage.ToolTip;
            });

            if (bitmapImage.IsDownloading)
                bitmapImage.DownloadCompleted += (ob, ev) => setImage(ob, ev);
            else
                setImage(bitmapImage, EventArgs.Empty);
        }
        #endregion
        #region DependencyProperty 'NavigateUri'

        /// <summary>
        /// Gets or sets the navigation uri.
        /// </summary>
        public string NavigateUri
        {
            get { return (string)GetValue(NavigateUriProperty); }
            set { SetValue(NavigateUriProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set the navigation uri.
        /// </summary>
        public static readonly DependencyProperty NavigateUriProperty = DependencyProperty.Register("NavigateUri", typeof(string), typeof(InlineImage), new FrameworkPropertyMetadata(null, OnNavigateUriChanged));

        private static void OnNavigateUriChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var inlineImage = (InlineImage)sender;
            inlineImage.MouseDown -= OnClick;
            if (inlineImage.NavigateUri == null)
            {
                inlineImage.Cursor = Cursors.Arrow;
            }
            else
            {
                inlineImage.Cursor = Cursors.Hand;
                inlineImage.MouseDown += OnClick;
            }
        }
        #endregion
        #region DependencyProperty 'Confirmation'

        /// <summary>
        /// Gets or sets the confirmation text.
        /// </summary>
        public string Confirmation
        {
            get { return (string)GetValue(ConfirmationProperty); }
            set { SetValue(ConfirmationProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set the confirmation text.
        /// </summary>
        public static readonly DependencyProperty ConfirmationProperty = DependencyProperty.Register("Confirmation", typeof(string), typeof(InlineImage));

        #endregion

        public static void OnClick(object o, MouseEventArgs e)
        {
            var inlineImage = (InlineImage)o;
            iApp.Navigate(new Link(inlineImage.NavigateUri) { ConfirmationText = inlineImage.Confirmation }, inlineImage.GetParent<IMXView>());
        }
    }
}