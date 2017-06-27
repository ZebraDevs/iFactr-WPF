using iFactr.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.UI;
using System.Collections;

namespace iFactr.Console
{
    internal class SelectList : Control, ISelectList
    {
        public Color BackgroundColor { get; set; }


        public Font Font { get; set; }

        public Color ForegroundColor { get; set; }


        public IEnumerable Items { get; set; }


        public int SelectedIndex { get; set; }

        public object SelectedItem { get; set; }

        public override string StringValue
        {
            get { return SelectedItem == null ? null : SelectedItem.ToString(); }
        }

        public event ValueChangedEventHandler<object> SelectionChanged;


        public override void NullifyEvents()
        {
            base.NullifyEvents();
            SelectionChanged = null;
        }

        public void ShowList()
        {
        }

        public override bool Validate(out string[] errors)
        {
            return Validate(SelectedItem, out errors);
        }
    }
}
