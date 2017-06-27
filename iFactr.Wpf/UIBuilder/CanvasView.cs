using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using iFactr.Core;
using iFactr.UI;

namespace iFactr.Wpf
{
    public class CanvasView : BaseView, ICanvasView
    {
        public event SaveEventHandler DrawingSaved;

        public Color StrokeColor
        {
            get { return inkCanvas.DefaultDrawingAttributes.Color.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? Color.Black : value;
                if (value != StrokeColor)
                {
                    inkCanvas.DefaultDrawingAttributes.Color = value.GetColor();
                    OnPropertyChanged("StrokeColor");
                }
            }
        }

        public double StrokeThickness
        {
            get { return inkCanvas.DefaultDrawingAttributes.Width; }
            set
            {
                if (value != StrokeThickness)
                {
                    inkCanvas.DefaultDrawingAttributes.Width = inkCanvas.DefaultDrawingAttributes.Height = value;
                    OnPropertyChanged("StrokeThickness");
                }
            }
        }

        public IToolbar Toolbar
        {
            get
            {
                var toolbar = Children.OfType<IToolbar>().FirstOrDefault();
                return toolbar == null ? null : (toolbar.Pair as IToolbar) ?? toolbar;
            }
            set
            {
                if (value != Toolbar)
                {
                    for (int i = Children.Count - 1; i >= 0; i--)
                    {
                        if (Children[i] is IToolbar)
                        {
                            Children.RemoveAt(i);
                        }
                    }

                    var toolbar = WpfFactory.GetNativeObject<UIElement>(value, "toolbar", true);
                    if (toolbar != null)
                    {
                        SetDock(toolbar, Dock.Bottom);
                        Children.Insert(Children.IndexOf(HeaderBar) + 1, toolbar);
                    }

                    OnPropertyChanged("Toolbar");
                }
            }
        }

        private InkCanvas inkCanvas;
        private Canvas backCanvas;

        public CanvasView()
        {
            backCanvas = new Canvas();
            Children.Add(backCanvas);

            inkCanvas = new InkCanvas()
            {
                Background = new System.Windows.Media.SolidColorBrush(),
                DefaultDrawingAttributes = new System.Windows.Ink.DrawingAttributes()
                {
                    StylusTip = System.Windows.Ink.StylusTip.Ellipse,
                    Height = 3,
                    Width = 3
                }
            };
            inkCanvas.SetBinding(FrameworkElement.WidthProperty, new System.Windows.Data.Binding("ActualWidth") { Source = backCanvas });
            inkCanvas.SetBinding(FrameworkElement.HeightProperty, new System.Windows.Data.Binding("ActualHeight") { Source = backCanvas });
            backCanvas.Children.Add(inkCanvas);
        }

        public void Clear()
        {
            inkCanvas.Strokes.Clear();
            if (inkCanvas.Children.Count > 0 && inkCanvas.Children[0] is System.Windows.Controls.Image)
            {
                inkCanvas.Children.RemoveAt(0);
            }
        }

        public async void Load(string fileName)
        {
            //if the first child is an image, that means something else was loaded and it should be cleared
            if (inkCanvas.Children.Count > 0 && inkCanvas.Children[0] is System.Windows.Controls.Image)
            {
                inkCanvas.Children.RemoveAt(0);
            }

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var image = new System.Windows.Controls.Image()
                {
                    Source = await WpfFactory.LoadBitmapAsync(fileName),
                    Stretch = System.Windows.Media.Stretch.None,
                };

                inkCanvas.Children.Insert(0, image);
            }
        }

        public void Save(string fileName)
        {
            Save(fileName, false);
        }

        public void Save(bool compositeBackground)
        {
            Save(Path.Combine(WpfFactory.Instance.TempImagePath, Guid.NewGuid().ToString() + ".png"), compositeBackground);
        }

        public void Save(string fileName, bool compositeBackground)
        {
            iApp.File.EnsureDirectoryExistsForFile(fileName);

            var target = compositeBackground ? backCanvas : (FrameworkElement)inkCanvas;
            var bitmap = new RenderTargetBitmap((int)target.ActualWidth, (int)target.ActualHeight,
                96, 96, System.Windows.Media.PixelFormats.Default);

            var visual = new System.Windows.Media.DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                var brush = new System.Windows.Media.VisualBrush(target);
                context.DrawRectangle(brush, null, new Rect(0, 0, target.ActualWidth, target.ActualHeight));
            }

            bitmap.Render(visual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                System.IO.File.WriteAllBytes(fileName, stream.ToArray());
            }
            
            var handler = DrawingSaved;
            if (handler != null)
            {
                handler(Pair ?? this, new SaveEventArgs(fileName));
            }
        }

        public override void SetBackground(Color color)
        {
            backCanvas.Background = color.IsDefaultColor ? null : color.GetBrush();
            backCanvas.Height = double.NaN;
            backCanvas.Width = double.NaN;
        }

        public override async void SetBackground(string imagePath, ContentStretch stretch)
        {
            System.Windows.Media.ImageBrush brush = null;
            BitmapImage bitmap = null;

            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                bitmap = await WpfFactory.LoadBitmapAsync(imagePath);
                brush = new System.Windows.Media.ImageBrush(bitmap);
            }

            if (brush == null || bitmap == null)
            {
                backCanvas.Background = null;

                backCanvas.Height = double.NaN;
                backCanvas.Width = double.NaN;
            }
            else
            {
                brush.Stretch = (System.Windows.Media.Stretch)stretch;
                backCanvas.Background = brush;

                backCanvas.Height = bitmap.Height;
                backCanvas.Width = bitmap.Width;
            }
        }
    }
}
