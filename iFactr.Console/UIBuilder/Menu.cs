using iFactr.UI;
using iFactr.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactr.Console
{
    internal class Menu : IMenu
    {
        private List<IMenuButton> Buttons = new List<IMenuButton>();
        public Color BackgroundColor { get; set; }

        public int ButtonCount { get { return Buttons.Count; } }

        public Color ForegroundColor { get; set; }

        public string ImagePath { get; set; }

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


        public Color SelectionColor { get; set; }

        public string Title { get; set; }
 
        public void Add(IMenuButton menuButton)
        {
            Buttons.Add(menuButton);
        }
        public bool Equals(IMenu other)
        {
            var menu = other as iFactr.UI.Menu;
            if (menu != null)
            {
                return menu.Equals(this);
            }

            return base.Equals(other);
        }

        public IMenuButton GetButton(int index)
        {
            return Buttons.ElementAtOrDefault(index);
        }
    }
}
