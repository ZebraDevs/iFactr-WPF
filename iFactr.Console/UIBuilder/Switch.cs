using iFactr.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.UI;

namespace iFactr.Console
{
    internal class Switch : Control, ISwitch
    {
       
        public Color FalseColor { get; set; }


        public Color ForegroundColor { get; set; }


        public override string StringValue { get { return Value.ToString(); }  }

        public Color TrueColor { get; set; }

        public bool Value { get; set; }

        public event ValueChangedEventHandler<bool> ValueChanged;


        public override void NullifyEvents()
        {
            base.NullifyEvents();
            ValueChanged = null;
        }

        public override bool Validate(out string[] errors)
        {
            return Validate(Value, out errors);
        }
    }
}
