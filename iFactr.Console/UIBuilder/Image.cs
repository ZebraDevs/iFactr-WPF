using iFactr.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.Core;
using iFactr.UI;
using MonoCross;

namespace iFactr.Console
{
    internal class Image : Control, IImage
    {
      
        public Size Dimensions { get;  }

        public string FilePath { get; set; }



        public ContentStretch Stretch { get; set; }

        public override string StringValue { get { return FilePath; }  }


        public event EventHandler Clicked;
        public event EventHandler Loaded;

        public IImageData GetImageData()
        {
            return null;
        }

        public override void NullifyEvents()
        {
            base.NullifyEvents();
            Clicked = null;
            Loaded = null;
        }

        public override bool Validate(out string[] errors)
        {
            return Validate(FilePath, out errors);

        }
    }
}
