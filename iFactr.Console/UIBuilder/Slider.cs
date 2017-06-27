using iFactr.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.UI;

namespace iFactr.Console
{
    internal class Slider : Control, ISlider
    {
    

        public Color MaximumTrackColor { get; set; }


        public double MaxValue { get; set; }


        public Color MinimumTrackColor { get; set; }

        public double MinValue { get; set; }

        public override string StringValue { get { return Value.ToString(); } }


        public double Value { get; set; }

        public event ValueChangedEventHandler<double> ValueChanged;

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
