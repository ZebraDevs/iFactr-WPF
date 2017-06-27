using iFactr.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.UI;

namespace iFactr.Console
{
    internal class TimePicker : Control, ITimePicker
    {
        public Color BackgroundColor { get; set; }

        public Font Font { get; set; }

        public Color ForegroundColor { get; set; }

        public override string StringValue
        {
            get
            {
                if (!Time.HasValue)
                {
                    return string.Empty;
                }

                return Time.Value.ToString(TimeFormat ?? "t");
            }
        }

        public DateTime? Time { get; set; }

        public string TimeFormat { get; set; }


        public event ValueChangedEventHandler<DateTime?> TimeChanged;

        public override void NullifyEvents()
        {
            base.NullifyEvents();
            TimeChanged = null;
        }

        public void ShowPicker()
        { }

        public override bool Validate(out string[] errors)
        {
            return Validate(Time, out errors);
        }
    }
}
