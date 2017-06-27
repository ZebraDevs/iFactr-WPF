using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iFactr.UI;

namespace iFactr.Wpf
{
    class PlatformDefaults : IPlatformDefaults
    {
        public double LargeHorizontalSpacing
        {
            get { return 10; }
        }

        public double LeftMargin
        {
            get { return 8; }
        }

        public double RightMargin
        {
            get { return 8; }
        }

        public double SmallHorizontalSpacing
        {
            get { return 4; }
        }

        public double BottomMargin
        {
            get { return 8; }
        }

        public double LargeVerticalSpacing
        {
            get { return 10; }
        }

        public double SmallVerticalSpacing
        {
            get { return 4; }
        }

        public double TopMargin
        {
            get { return 8; }
        }

        public double CellHeight
        {
            get { return 48; }
        }

        public Font ButtonFont
        {
            get { return LabelFont; }
        }

        public Font DateTimePickerFont
        {
            get { return new Font("Segoe UI", 12); }
        }

        public Font HeaderFont
        {
            get { return new Font("Segoe UI", 12); }
        }

        public Font LabelFont
        {
            get { return new Font("Segoe UI", 12); }
        }

        public Font LargeTabFont
        {
            get { return TabFont; }
        }

        public Font MessageBodyFont
        {
            get { return new Font("Segoe UI", 11); }
        }

        public Font MessageTitleFont
        {
            get { return new Font("Segoe UI", 12); }
        }

        public Font SectionHeaderFont
        {
            get { return new Font("Segoe UI", 12); }
        }

        public Font SectionFooterFont
        {
            get { return new Font("Segoe UI", 11); }
        }

        public Font SelectListFont
        {
            get { return new Font("Segoe UI", 12); }
        }

        public Font SmallFont
        {
            get { return new Font("Segoe UI", 11); }
        }

        public Font TabFont
        {
            get { return new Font("Segoe UI", 11); }
        }

        public Font TextBoxFont
        {
            get { return new Font("Segoe UI", 12); }
        }

        public Font ValueFont
        {
            get { return LabelFont; }
        }

        public Size TileLargeSize
        {
            get { return Size.Empty; }
        }

        public Size TileMediumSize
        {
            get { return Size.Empty; }
        }

        public Size TileSmallSize
        {
            get { return Size.Empty; }
        }

        public Size TileWideSize
        {
            get { return Size.Empty; }
        }
    }
}
