using iFactr.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.UI.Controls;
using System.Collections;

namespace iFactr.Console
{
    internal class GridCell : IGridCell
    {
        public Link AccessoryLink { get; set; }

        public Color BackgroundColor { get; set; }

        public IEnumerable<IElement> Children { get; } = new List<IElement>();

        public ColumnCollection Columns { get; } = new ColumnCollection();

        public double MaxHeight { get; set; }

        public MetadataCollection Metadata { get; } = new MetadataCollection();

        public double MinHeight { get; set; }

        public Link NavigationLink { get; set; }

        public Thickness Padding { get; set; }

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

        public RowCollection Rows { get; } = new RowCollection();


        public Color SelectionColor { get; set; }

        public SelectionStyle SelectionStyle { get; set; }

        public event EventHandler AccessorySelected;
        public event EventHandler Selected;

        public void AddChild(IElement element)
        {
            ((IList)Children).Add(element);
        }

        public bool Equals(ICell other)
        {
            var cell = other as iFactr.UI.Cell;
            if (cell != null)
            {
                return cell.Equals(this);
            }

            return base.Equals(other);
        }

        public void NullifyEvents()
        {
            AccessorySelected = null;
            Selected = null;
        }

        public void RemoveChild(IElement element)
        {
            ((IList)Children).Remove(element);
        }

        public void Select()
        {
            Selected?.Invoke(Pair ?? this, EventArgs.Empty);
        }
    }
}
