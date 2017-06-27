using iFactr.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.UI;

namespace iFactr.Console
{
    internal class DatePicker : Control, IDatePicker
    {
        public Color BackgroundColor { get; set; }

       
        public DateTime? Date { get; set; }

        public string DateFormat { get; set; }

        public Font Font { get; set; }

        public Color ForegroundColor { get; set; }



        public override string StringValue
        {
            get
            {
                if (!Date.HasValue)
                {
                    return string.Empty;
                }

                return Date.Value.ToString(DateFormat ?? "d");
            }
        }
        

        public event ValueChangedEventHandler<DateTime?> DateChanged;
        

        public override void NullifyEvents()
        {
            base.NullifyEvents();
            DateChanged = null;
        }
      

        public void ShowPicker()
        {
        }

        public override bool Validate(out string[] errors)
        {
            return Validate(Date, out errors);
        }
    }
}
