using iFactr.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactr.Console
{
    internal class TabView : BaseView, ITabView
    {
    

        public int SelectedIndex { get; set; }

        public Color SelectionColor { get; set; }

        public IEnumerable<ITabItem> TabItems { get; set; }
 

    }
}
