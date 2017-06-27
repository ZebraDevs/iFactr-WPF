using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.UI;
using iFactr.UI.Controls;

namespace iFactr.Console
{
    internal class Button : Control, IButton
    {
        public Color BackgroundColor { get; set; }

        public Font Font { get; set; }

        public Color ForegroundColor { get; set; }

        public IImage Image { get; set; }

        public Link NavigationLink { get; set; }
     
        public override string StringValue { get { return Title; } }

        public string Title { get; set; }

        public event EventHandler Clicked;

        public override void NullifyEvents()
        {
            base.NullifyEvents();
            Clicked = null;
        }

        public override bool Validate(out string[] errors)
        {
            return Validate(Title, out errors);
        }
    }
}
