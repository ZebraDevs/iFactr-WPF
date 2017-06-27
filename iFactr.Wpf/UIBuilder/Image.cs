using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using iFactr.Core;
using MonoCross.Utilities;
using MonoCross;
using iFactr.UI;
using iFactr.UI.Controls;

using Size = iFactr.UI.Size;

namespace iFactr.Wpf
{
    public class Image : System.Windows.Controls.Grid, IImage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler Clicked;

        public new event EventHandler Loaded;

        public event ValidationEventHandler Validating;

        //public UI.Color BorderColor
        //{
        //    get { return image.BorderBrush.GetColor(); }
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

        public string FilePath
        {
            get { return filePath; }
            set
            {
                if (filePath != value)
                {
                    filePath = value;
                    SetSource(filePath);
                    
                    var phandler = PropertyChanged;
                    if (phandler != null)
                    {
                        phandler(this, new PropertyChangedEventArgs("FilePath"));
                        phandler(this, new PropertyChangedEventArgs("StringValue"));
                    }

                    if (image.Source != null && !filePath.IsRemotePath())
                    {
                        var handler = Loaded;
                        if (handler != null)
                        {
                            handler(Pair ?? this, EventArgs.Empty);
                        }
                    }
                }
            }
        }
        private string filePath;

        public Size Dimensions
        {
            get
            {
                var source = image.Source as BitmapImage;
                return source == null ? Size.Empty : new Size(source.PixelWidth, source.PixelHeight);
            }
        }

        public ContentStretch Stretch
        {
            get
            {
                if (image.StretchDirection == StretchDirection.DownOnly)
                {
                    return ContentStretch.None;
                }
                
                return (ContentStretch)image.Stretch;
            }
            set
            {
                if (value != Stretch)
                {
                    if (value == ContentStretch.None)
                    {
                        image.Stretch = System.Windows.Media.Stretch.Uniform;
                        image.StretchDirection = StretchDirection.DownOnly;
                    }
                    else
                    {
                        image.Stretch = (Stretch)value;
                        image.StretchDirection = StretchDirection.Both;
                    }

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Stretch"));
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
                if (value != image.IsEnabled)
                {
                    image.IsEnabled = value;

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

        public string StringValue
        {
            get { return FilePath; }
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

        private System.Windows.Controls.Image image;
        private ImageCreationOptions creationOptions;
        private bool hovering;

        public Image()
        {
            image = new System.Windows.Controls.Image()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            };

            image.MouseDown += (o, e) => hovering = true;
            image.MouseLeave += (o, e) => hovering = false;
            image.MouseUp += (o, e) =>
            {
                if (hovering)
                {
                    hovering = false;

                    var handler = Clicked;
                    if (handler != null)
                    {
                        e.Handled = true;
                        handler(pair ?? this, EventArgs.Empty);
                    }
                }
            };

            Children.Add(image);
            Stretch = ContentStretch.None;
        }

        public Image(ImageCreationOptions options)
            : this()
        {
            creationOptions = options;
        }

        public Image(IImageData imageData)
            : this()
        {
            var data = imageData as ImageData;
            if (data != null)
            {
                image.Source = data.Source;
            }
        }

        public IImageData GetImageData()
        {
            return new ImageData(image.Source as BitmapImage, filePath);
        }

        public UI.Size Measure(UI.Size constraints)
        {
            var dimensions = Dimensions;
            if (dimensions == UI.Size.Empty)
            {
                return UI.Size.Empty;
            }

            var stretch = Stretch;
            double scale = 1;
            if (constraints.Width < dimensions.Width || stretch != ContentStretch.None)
            {
                scale = constraints.Width / dimensions.Width;
            }
            if (constraints.Height < dimensions.Height || stretch != ContentStretch.None)
            {
                scale = Math.Min(scale, constraints.Height / dimensions.Height);
            }

            return dimensions * scale;
        }

        public void SetLocation(UI.Point location, UI.Size size)
        {
            Canvas.SetLeft(this, location.X);
            Canvas.SetTop(this, location.Y);
            Width = size.Width;
            Height = size.Height;
        }

        public void NullifyEvents()
        {
            Clicked = null;
            Loaded = null;
            Validating = null;
        }

        public bool Validate(out string[] errors)
        {
            var handler = Validating;
            if (handler != null)
            {
                var args = new ValidationEventArgs(SubmitKey, FilePath, StringValue);
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

        private async void SetSource(string uri)
        {
            var imageData = uri == null ? null : Device.ImageCache.Get(uri);
            if (imageData != null && (creationOptions & ImageCreationOptions.IgnoreCache) != 0)
            {
                Device.ImageCache.Remove(uri);
                imageData = null;
            }

            BitmapImage source = null;
            if (imageData == null)
            {
                if (uri != null && (uri.StartsWith("http") || uri.StartsWith("ftp")))
                {
                    source = new BitmapImage(new Uri(uri), new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.BypassCache));
                }
                else
                {
                    source = await WpfFactory.LoadBitmapAsync(uri);
                }

                if (source != null)
                {
                    source.DownloadCompleted -= OnDownloadCompleted;
                    source.DownloadCompleted += OnDownloadCompleted;

                    if ((creationOptions & ImageCreationOptions.IgnoreCache) == 0)
                    {
                        Device.ImageCache.Add(uri, new ImageData(source));
                    }
                }
            }
            else
            {
                var data = imageData as ImageData;
                source = data == null ? null : data.Source;
            }

            image.Source = source;

            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs("Dimensions"));
            }

            var parent = this.GetParent<IPairable>() as FrameworkElement;
            if (parent != null)
            {
                parent.InvalidateMeasure();
            }
        }

        private void OnDownloadCompleted(object sender, EventArgs args)
        {
            var phandler = PropertyChanged;
            if (phandler != null)
            {
                phandler(this, new PropertyChangedEventArgs("Dimensions"));
            }

            var handler = Loaded;
            if (handler != null)
            {
                handler(Pair ?? this, EventArgs.Empty);
            }

            var parent = image.GetParent<IGridBase>();
            if (parent != null)
            {
                var element = parent as FrameworkElement;
                if (element == null)
                {
                    return;
                }

                var cell = parent as ICell;
                if (cell != null)
                {
                    parent.PerformLayout(new Size(element.ActualWidth, cell.MinHeight),
                        new Size(element.ActualWidth, cell.MaxHeight));
                }
                else
                {
                    var view = parent as IGridView;
                    if (view != null)
                    {
                        parent.PerformLayout(new UI.Size(view.Width, view.Height),
                            new UI.Size(view.HorizontalScrollingEnabled ? double.PositiveInfinity : view.Width,
                                view.VerticalScrollingEnabled ? double.PositiveInfinity : view.Height));
                    }
                }
            }
        }
    }
}
