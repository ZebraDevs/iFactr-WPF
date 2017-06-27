using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Drawing;
using iFactr.Core.Styles;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;

namespace iFactr.Wpf.NinePatch
{
    public class NinePatchImage
    {
        // create an empty NinePatch
        public NinePatchImage()
        {
            _DynamicResizeMethod = DynamicResizeMethod.Tile;
        }

        // create a NinePatch from file
        public NinePatchImage(string fileName, DynamicResizeMethod DynamicResizeMethod)
        {
            if (String.IsNullOrWhiteSpace(fileName))
            {
                _DynamicResizeMethod = DynamicResizeMethod.Tile;
            }
            else
            {
                _DynamicResizeMethod = DynamicResizeMethod;

                const uint BLACK_PIXEL_FULL_ALPHA = 0xFF000000;

                Bitmap npi = null;
                try
                {
                    npi = new Bitmap(String.Format("{0}\\{1}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), fileName));
                }
                catch (Exception ex)
                {
                    throw new Exception("Bitmap could not be created from PNG. The PNG file may be missing from the container.", ex);
                }

                npi.SetResolution(72, 72);
                var bitmapData = npi.LockBits(new Rectangle(0, 0, npi.Width, npi.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                IntPtr dataPointer = bitmapData.Scan0;

                int topStart = 0;
                int topEnd = npi.Width;
                int leftStart = 0;
                int leftEnd = npi.Height;
                int bottomStart = 0;
                int bottomEnd = npi.Width;
                int rightStart = 0;
                int rightEnd = npi.Height;

                bool noPaddingSpecified = false;
                int centerHeight = 0;
                int centerWidth = 0;

                unsafe
                {
                    // calculate top stretch line
                    int firstPixel = npi.Width;
                    int lastPixel = 0;
                    uint* imagePointer = (uint*)(void*)dataPointer;
                    for (int xx = 0; xx < npi.Width; xx++, imagePointer++)
                    {
                        uint thisValue = *imagePointer;
                        if (*imagePointer == BLACK_PIXEL_FULL_ALPHA)
                        {
                            if (xx < firstPixel)
                                firstPixel = xx;
                            if (xx > lastPixel)
                                lastPixel = xx;
                        }
                    }
                    topStart = firstPixel;
                    topEnd = lastPixel;
                    _leftWidth = topStart - 1;
                    _rightWidth = (npi.Width - 2) - topEnd; // assumes padding lines (-1 if not)!
                    centerWidth = (npi.Width - 2) - (_leftWidth + _rightWidth);

                    // calculate left side stretch line
                    firstPixel = npi.Height;
                    lastPixel = 0;
                    imagePointer = (uint*)(void*)dataPointer;
                    for (int xx = 0; xx < npi.Height; xx++, imagePointer += npi.Width)
                    {
                        uint thisValue = *imagePointer;
                        if (thisValue == BLACK_PIXEL_FULL_ALPHA)
                        {
                            if (xx < firstPixel)
                                firstPixel = xx;
                            if (xx > lastPixel)
                                lastPixel = xx;
                        }
                    }
                    leftStart = firstPixel;
                    leftEnd = lastPixel;
                    _upperHeight = leftStart - 1;
                    _lowerHeight = (npi.Height - 2) - leftEnd;
                    centerHeight = (npi.Height - 2) - (_upperHeight + _lowerHeight);

                    // calculate right side padding line
                    firstPixel = npi.Height;
                    lastPixel = 0;
                    imagePointer = ((uint*)(void*)dataPointer) + (npi.Width - 1);
                    for (int xx = 0; xx < npi.Height; xx++, imagePointer += npi.Width)
                    {
                        uint thisValue = *imagePointer;
                        if (thisValue == BLACK_PIXEL_FULL_ALPHA)
                        {
                            if (xx < firstPixel)
                                firstPixel = xx;
                            if (xx > lastPixel)
                                lastPixel = xx;
                        }
                    }
                    if (lastPixel == 0)
                    {
                        noPaddingSpecified = true;
                    }
                    rightStart = firstPixel;
                    rightEnd = lastPixel;

                    // calculate bottom padding line
                    firstPixel = npi.Width;
                    lastPixel = 0;
                    imagePointer = ((uint*)(void*)dataPointer) + (npi.Width * (npi.Height - 1));
                    for (int xx = 0; xx < npi.Width; xx++, imagePointer++)
                    {
                        uint thisValue = *imagePointer;
                        if (*imagePointer == BLACK_PIXEL_FULL_ALPHA)
                        {
                            if (xx < firstPixel)
                                firstPixel = xx;
                            if (xx > lastPixel)
                                lastPixel = xx;
                        }
                    }
                    if (lastPixel == 0)
                    {
                        noPaddingSpecified = true;
                    }
                    bottomStart = firstPixel;
                    bottomEnd = lastPixel;
                }
                npi.UnlockBits(bitmapData);

                _IsEmpty = true;

                // upper section bitmaps
                Rectangle imageRect = new Rectangle(1, 1, _leftWidth, _upperHeight);
                //Debug.WriteLine("Upper Left: " + imageRect.ToString());
                _upperLeft = IsRect(imageRect) ? npi.Crop(imageRect) : null;
                if (_upperLeft != null) _IsEmpty = false;

                imageRect = new Rectangle(topStart, 1, centerWidth, _upperHeight);
                //Debug.WriteLine("Upper: " + imageRect.ToString());
                _upper = IsRect(imageRect) ? npi.Crop(imageRect) : null;
                if (_upper != null) _IsEmpty = false;

                imageRect = new Rectangle(topEnd + 1, 1, _rightWidth, _upperHeight);
                //Debug.WriteLine("Upper Right: " + imageRect.ToString());
                _upperRight = IsRect(imageRect) ? npi.Crop(imageRect) : null;
                if (_upperRight != null) _IsEmpty = false;

                // center section bitmaps
                imageRect = new Rectangle(1, leftStart, _leftWidth, centerHeight);
                //Debug.WriteLine("Left: " + imageRect.ToString());
                _left = IsRect(imageRect) ? npi.Crop(imageRect) : null;
                if (_left != null) _IsEmpty = false;

                imageRect = new Rectangle(topStart, leftStart, centerWidth, centerHeight);
                //Debug.WriteLine("Center: " + imageRect.ToString());
                _center = IsRect(imageRect) ? npi.Crop(imageRect) : null;
                if (_center != null) _IsEmpty = false;

                imageRect = new Rectangle(topEnd + 1, leftStart, _rightWidth, centerHeight);
                //Debug.WriteLine("Right: " + imageRect.ToString());
                _right = IsRect(imageRect) ? npi.Crop(imageRect) : null;
                if (_right != null) _IsEmpty = false;

                // lower section bitmaps
                imageRect = new Rectangle(1, leftEnd + 1, _leftWidth, _lowerHeight);
                //Debug.WriteLine("Lower Left: " + imageRect.ToString());
                _lowerLeft = IsRect(imageRect) ? npi.Crop(imageRect) : null;
                if (_lowerLeft != null) _IsEmpty = false;

                imageRect = new Rectangle(topStart, leftEnd + 1, centerWidth, _lowerHeight);
                //Debug.WriteLine("Lower: " + imageRect.ToString());
                _lower = IsRect(imageRect) ? npi.Crop(imageRect) : null;
                if (_lower != null) _IsEmpty = false;

                imageRect = new Rectangle(topEnd + 1, leftEnd + 1, _rightWidth, _lowerHeight);
                //Debug.WriteLine("Lower Right: " + imageRect.ToString());
                _lowerRight = IsRect(imageRect) ? npi.Crop(imageRect) : null;
                if (_lowerRight != null) _IsEmpty = false;

#if DEBUG
                // Debugging code to check that the 9 images are correctly sliced out of the original image.
                // Writes out a [Guid.NewGuid().ToString()].png to the app container root (check bin/Debug).
                //Bitmap bmp = new Bitmap(_upperLeft.Width + _upper.Width + _upperRight.Width, _upperLeft.Height + _left.Height + _lowerLeft.Height);
                //bmp.SetResolution(72, 72);
                //Graphics g = Graphics.FromImage(bmp);
                //g.DrawImage(_upperLeft, 0, 0);
                //g.DrawImage(_upper, _upperLeft.Width, 0);
                //g.DrawImage(_upperRight, _upperLeft.Width + _upper.Width, 0);
                //g.DrawImage(_left, 0, _upperLeft.Height);
                //g.DrawImage(_center, _left.Width, _upper.Height);
                //g.DrawImage(_right, _left.Width + _center.Width, _upperRight.Height);
                //g.DrawImage(_lowerLeft, 0, _upperLeft.Height + _left.Height);
                //g.DrawImage(_lower, _lowerLeft.Width, _upper.Height + _center.Height);
                //g.DrawImage(_lowerRight, _lowerLeft.Width + _lower.Width, _upperRight.Height + _right.Height);
                //bmp.Save(String.Format("{0}\\{1}.png", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Guid.NewGuid().ToString()), System.Drawing.Imaging.ImageFormat.Png);
#endif

                if (noPaddingSpecified)
                    // uses the center area defined by the top and left margins
                    _paddingBounds = new Rectangle(_upperLeft.Width, _upperLeft.Height, _center.Width, _center.Height);
                else
                    // uses the area defined by the bottom and right margins
                    _paddingBounds = new Rectangle(bottomStart - 1, rightStart - 1, bottomEnd - bottomStart, rightEnd - rightStart);

                //Debug.WriteLine("Content/Padding: " + _paddingBounds.ToString());
            }
        }

        public Bitmap GetBitmap()
        {
            var rectangle = new Rectangle(0, 0, IntrinsicWidth, IntrinsicHeight);
            System.Drawing.Size size = rectangle.Size;
            int offsetX = rectangle.Left;
            int offsetY = rectangle.Top;

            // validate that requested size is greater than the original image
            int centerWidth = size.Width - _leftWidth + _rightWidth;
            int centerHeight = size.Height - _upperHeight + _lowerHeight;

            if (centerWidth <= 0 || centerHeight <= 0)
            {
                throw new ArgumentException("Specified size is too small for specified nine-patch image.");
            }

            // create a bitmap to draw on
            var bitmap = new Bitmap(size.Width, size.Height);
            // tell the Graphics object to draw on the bitmap
            Graphics g = Graphics.FromImage(bitmap);

            // draw each image in the appropriate rectangle
            if (_upperLeft != null)
                g.DrawImage(_upperLeft, new PointF(offsetX, offsetY));
            if (_upper != null)
                g.DrawImage(_upper, new RectangleF(offsetX + _leftWidth, offsetY, centerWidth, _upper.Height));
            if (_upperRight != null)
                g.DrawImage(_upperRight, new PointF(offsetX + _leftWidth + centerWidth, offsetY));

            if (_left != null)
                g.DrawImage(_left, new RectangleF(offsetX, offsetY + _upperHeight, _left.Width, centerHeight));
            if (_center != null)
                g.DrawImage(_center, new RectangleF(offsetX + _leftWidth, offsetY + _upperHeight, centerWidth, centerHeight));
            if (_right != null)
                g.DrawImage(_right, new RectangleF(offsetX + _leftWidth + centerWidth, offsetY + _upperHeight, _right.Width, centerHeight));

            if (_lowerLeft != null)
                g.DrawImage(_lowerLeft, new PointF(offsetX, offsetY + _upperHeight + centerHeight));
            if (_lower != null)
                g.DrawImage(_lower, new RectangleF(offsetX + _leftWidth, offsetY + _upperHeight + centerHeight, centerWidth, _lower.Height));
            if (_lowerRight != null)
                g.DrawImage(_lowerRight, new PointF(offsetX + _leftWidth + centerWidth, offsetY + _upperHeight + centerHeight));

            // return the final nine patch bitmap that has all of the 9 chunks drawn on it
            return bitmap;
        }

        #region Internal Class Members
        private int _upperHeight;
        private int _lowerHeight;
        private int _leftWidth;
        private int _rightWidth;

        private Bitmap _upperLeft;
        private Bitmap _upper;
        private Bitmap _upperRight;
        private Bitmap _right;
        private Bitmap _lowerRight;
        private Bitmap _lower;
        private Bitmap _lowerLeft;
        private Bitmap _left;
        private Bitmap _center;
        private Rectangle _paddingBounds;
        private DynamicResizeMethod _DynamicResizeMethod;

        private static bool IsRect(RectangleF rect)
        {
            return (rect.Height > 0 && rect.Width > 0);
        }
        #endregion

        public ImageSource TopLeft
        {
            get
            {
                if (_upperLeft == null) return null;
                return _upperLeft.ToImageSource();
            }
        }
        public ImageSource TopCenter
        {
            get
            {
                if (_upper == null) return null;
                return _upper.ToImageSource();
            }
        }
        public ImageSource TopRight
        {
            get
            {
                if (_upperRight == null) return null;
                return _upperRight.ToImageSource();
            }
        }
        public ImageSource MiddleLeft
        {
            get
            {
                if (_left == null) return null;
                return _left.ToImageSource();
            }
        }
        public ImageSource MiddleCenter
        {
            get
            {
                if (_center == null) return null;
                return _center.ToImageSource();
            }
        }
        public ImageSource MiddleRight
        {
            get
            {
                if (_right == null) return null;
                return _right.ToImageSource();
            }
        }
        public ImageSource BottomLeft
        {
            get
            {
                if (_lowerLeft == null) return null;
                return _lowerLeft.ToImageSource();
            }
        }
        public ImageSource BottomCenter
        {
            get
            {
                if (_lower == null) return null;
                return _lower.ToImageSource();
            }
        }
        public ImageSource BottomRight
        {
            get
            {
                if (_lowerRight == null) return null;
                return _lowerRight.ToImageSource();
            }
        }

        private bool _IsEmpty;
        public bool IsEmpty
        {
            get { return _IsEmpty; }
        }

        /// <summary>
        /// Retrieves the height of the source .png file (before resizing).
        /// </summary>
        public int IntrinsicHeight { get { return _upperHeight + _center.Height + _lowerHeight; } }

        /// <summary>
        /// Retrieves the width of the source .png file (before resizing).
        /// </summary>
        public int IntrinsicWidth { get { return _leftWidth + _center.Width + _rightWidth; } }

        /// <summary>
        /// Returns the minimum height suggested by this Drawable.
        /// </summary>
        public int MinimumHeight { get { return _upperHeight + _lowerHeight; } }

        /// <summary>
        /// Returns the minimum width suggested by this Drawable.
        /// </summary>
        public int MinimumWidth { get { return _leftWidth + _rightWidth; } }

        /// <summary>
        /// Returns the size/location of the content center relative to the top corner
        /// <//summary>
        public RectangleF ContentRectangle { get { return _paddingBounds; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size">
        /// A <see cref="SizeF"/>
        /// </param>
        /// <returns>
        /// A <see cref="RectangleF"/>
        /// </returns>
        public RectangleF GetContentRectangleForSize(SizeF size)
        {
            // start with the current padding rect
            var content = ContentRectangle;

            // add to it the size increase
            content.Height += size.Height - IntrinsicHeight;
            content.Width += size.Width - IntrinsicWidth;
            return content;
        }
    }

    public static class ImageExtensions
    {
        // returns a cropped bitmap
        public static Bitmap Crop(this Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            bmp.SetResolution(72, 72);

            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }
    }

    public static class BitmapExtensions
    {
        public static ImageSource ToImageSource(this Bitmap bitmap)
        {
            var hbitmap = bitmap.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height));
            }
            finally
            {
                NativeMethods.DeleteObject(hbitmap);
            }
        }
    }

    public static class NativeMethods
    {
        [DllImport("gdi32")]
        public static extern int DeleteObject(IntPtr hObject);
    }
}