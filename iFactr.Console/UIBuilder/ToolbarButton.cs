using iFactr.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactr.Console
{
    internal class ToolbarButton : IToolbarButton
    {
        public Color ForegroundColor { get; set; }

        public string ImagePath { get; set; }

        public Link NavigationLink { get; set; }

        public IPairable Pair
        {
            get { return pair; }
            set
            {
                if (pair == null && value != null)
                {
                    pair = value;
                    pair.Pair = this;
                }
            }
        }
        private IPairable pair;

        public string Title { get; set; }

        public event EventHandler Clicked;

        public bool Equals(IToolbarButton other)
        {
            var item = other as iFactr.UI.ToolbarButton;
            if (item != null)
            {
                return item.Equals(this);
            }

            return base.Equals(other);
        }
    }
}
