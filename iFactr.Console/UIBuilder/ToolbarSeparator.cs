using iFactr.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactr.Console
{
    internal class ToolbarSeparator : IToolbarSeparator
    {
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

        public bool Equals(IToolbarSeparator other)
        {
            var item = other as iFactr.UI.ToolbarSeparator;
            if (item != null)
            {
                return item.Equals(this);
            }

            return base.Equals(other);
        }
    }
}
