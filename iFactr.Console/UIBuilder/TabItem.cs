using iFactr.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactr.Console
{
    internal class TabItem : ITabItem
    {
        public string BadgeValue { get; set; }
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
        public Color TitleColor { get; set; }
        public Font TitleFont { get; set; }

        public event EventHandler Selected;

        public bool Equals(ITabItem other)
        {
            var item = other as iFactr.UI.TabItem;
            if (item != null)
            {
                return item.Equals(this);
            }

            return base.Equals(other); ;
        }
    }
}
