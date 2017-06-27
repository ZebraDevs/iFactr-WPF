using iFactr.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactr.Console
{
    internal class SectionHeader : ISectionHeader
    {
        public Color BackgroundColor { get; set; }

        public Font Font { get; set; }

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

        public string Text { get; set; }

    }
}
