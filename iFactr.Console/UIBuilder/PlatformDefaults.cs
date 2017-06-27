using iFactr.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactr.Console
{
    internal class PlatformDefaults : IPlatformDefaults
    {
        public double BottomMargin { get; }

        public Font ButtonFont { get; }

        public double CellHeight { get; }

        public Font DateTimePickerFont { get; }

        public Font HeaderFont { get; }

        public Font LabelFont { get; }

        public double LargeHorizontalSpacing { get; }

        public double LargeVerticalSpacing { get; }

        public double LeftMargin { get; }

        public Font MessageBodyFont { get; }

        public Font MessageTitleFont { get; }

        public double RightMargin { get; }

        public Font SectionFooterFont { get; }

        public Font SectionHeaderFont { get; }

        public Font SelectListFont { get; }

        public Font SmallFont { get; }

        public double SmallHorizontalSpacing { get; }

        public double SmallVerticalSpacing { get; }

        public Font TabFont { get; }

        public Font TextBoxFont { get; }

        public double TopMargin { get; }

        public Font ValueFont { get; }
    }
}
