using iFactr.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.Core.Controls;

namespace iFactr.Console
{
    internal class RichContentCell : IRichContentCell
    {
        public Color BackgroundColor { get; set; }

        public Color ForegroundColor { get; set; }

        public List<PanelItem> Items { get; set; }

        public double MaxHeight { get; set; }

        public MetadataCollection Metadata { get; } = new MetadataCollection();

        public double MinHeight { get; set; }
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

        public bool Equals(ICell other)
        {
            var cell = other as iFactr.UI.Cell;
            if (cell != null)
            {
                return cell.Equals(this);
            }

            return base.Equals(other);
        }

        public void Load()
        {
        }
    }
}
