using iFactr.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactr.Console
{
    internal class Toolbar : IToolbar
    {
        public Color BackgroundColor { get; set; }

        public Color ForegroundColor { get; set; }

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

        public IEnumerable<IToolbarItem> PrimaryItems { get; set; }

        public IEnumerable<IToolbarItem> SecondaryItems { get; set; }

        public bool Equals(IToolbar other)
        {
            var toolbar = other as iFactr.UI.Toolbar;
            if (toolbar != null)
            {
                return toolbar.Equals(this);
            }

            return base.Equals(other);
        }
    }
}
