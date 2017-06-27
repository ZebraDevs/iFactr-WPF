using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.UI;

namespace iFactr.Console
{
    internal class SearchBox : ISearchBox
    {
        public Color BackgroundColor { get; set; }

        public Color BorderColor { get; set; }

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

        public string Placeholder { get; set; }

        public string Text { get; set; }

        public TextCompletion TextCompletion { get; set; }

        public event SearchEventHandler SearchPerformed;

        public void Focus()
        {
        }
    }
}
