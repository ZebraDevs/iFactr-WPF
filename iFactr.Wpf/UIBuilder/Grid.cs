using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using iFactr.UI;
using iFactr.UI.Controls;

using Point = iFactr.UI.Point;
using Size = iFactr.UI.Size;

namespace iFactr.Wpf
{
    //public class Grid : Canvas, IGrid, INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    public ColumnCollection Columns { get; private set; }

    //    public UI.Thickness Padding
    //    {
    //        get { return padding; }
    //        set
    //        {
    //            if (value != padding)
    //            {
    //                padding = value;

    //                var handler = PropertyChanged;
    //                if (handler != null)
    //                {
    //                    handler(this, new PropertyChangedEventArgs("Padding"));
    //                }
    //            }
    //        }
    //    }
    //    private UI.Thickness padding;

    //    public RowCollection Rows { get; private set; }

    //    public new IEnumerable<IElement> Children
    //    {
    //        get
    //        {
    //            var controls = GetControls(this).Select(c => (c.Pair as IElement) ?? c);
    //            foreach (var control in controls)
    //            {
    //                yield return control;
    //            }
    //        }
    //    }

    //    public new UI.Thickness Margin
    //    {
    //        get { return margin; }
    //        set
    //        {
    //            if (value != margin)
    //            {
    //                margin = value;

    //                var handler = PropertyChanged;
    //                if (handler != null)
    //                {
    //                    handler(this, new PropertyChangedEventArgs("Margin"));
    //                }

    //                var parent = this.GetParent<IPairable>() as FrameworkElement;
    //                if (parent != null)
    //                {
    //                    parent.InvalidateMeasure();
    //                }
    //            }
    //        }
    //    }
    //    private UI.Thickness margin;

    //    public MetadataCollection Metadata
    //    {
    //        get { return metadata ?? (metadata = new MetadataCollection()); }
    //    }
    //    private MetadataCollection metadata;

    //    public string ID
    //    {
    //        get { return id; }
    //        set
    //        {
    //            if (value != id)
    //            {
    //                id = value;

    //                var handler = PropertyChanged;
    //                if (handler != null)
    //                {
    //                    handler(this, new PropertyChangedEventArgs("ID"));
    //                }
    //            }
    //        }
    //    }
    //    private string id;

    //    public new object Parent
    //    {
    //        get
    //        {
    //            var parent = this.GetParent<IPairable>();
    //            return parent == null ? null : (parent.Pair ?? parent);
    //        }
    //    }

    //    public int ColumnIndex
    //    {
    //        get { return columnIndex; }
    //        set
    //        {
    //            if (value != columnIndex)
    //            {
    //                columnIndex = value;

    //                var handler = PropertyChanged;
    //                if (handler != null)
    //                {
    //                    handler(this, new PropertyChangedEventArgs("ColumnIndex"));
    //                }

    //                var parent = this.GetParent<IPairable>() as FrameworkElement;
    //                if (parent != null)
    //                {
    //                    parent.InvalidateMeasure();
    //                }
    //            }
    //        }
    //    }
    //    private int columnIndex;

    //    public int ColumnSpan
    //    {
    //        get { return columnSpan; }
    //        set
    //        {
    //            if (value != columnSpan)
    //            {
    //                columnSpan = value;

    //                var handler = PropertyChanged;
    //                if (handler != null)
    //                {
    //                    handler(this, new PropertyChangedEventArgs("ColumnSpan"));
    //                }

    //                var parent = this.GetParent<IPairable>() as FrameworkElement;
    //                if (parent != null)
    //                {
    //                    parent.InvalidateMeasure();
    //                }
    //            }
    //        }
    //    }
    //    private int columnSpan;

    //    public int RowIndex
    //    {
    //        get { return rowIndex; }
    //        set
    //        {
    //            if (value != rowIndex)
    //            {
    //                rowIndex = value;

    //                var handler = PropertyChanged;
    //                if (handler != null)
    //                {
    //                    handler(this, new PropertyChangedEventArgs("RowIndex"));
    //                }

    //                var parent = this.GetParent<IPairable>() as FrameworkElement;
    //                if (parent != null)
    //                {
    //                    parent.InvalidateMeasure();
    //                }
    //            }
    //        }
    //    }
    //    private int rowIndex;

    //    public int RowSpan
    //    {
    //        get { return rowSpan; }
    //        set
    //        {
    //            if (value != rowSpan)
    //            {
    //                rowSpan = value;

    //                var handler = PropertyChanged;
    //                if (handler != null)
    //                {
    //                    handler(this, new PropertyChangedEventArgs("RowSpan"));
    //                }

    //                var parent = this.GetParent<IPairable>() as FrameworkElement;
    //                if (parent != null)
    //                {
    //                    parent.InvalidateMeasure();
    //                }
    //            }
    //        }
    //    }
    //    private int rowSpan;

    //    public new iFactr.UI.HorizontalAlignment HorizontalAlignment
    //    {
    //        get { return (iFactr.UI.HorizontalAlignment)base.HorizontalAlignment; }
    //        set
    //        {
    //            if (value != HorizontalAlignment)
    //            {
    //                base.HorizontalAlignment = (System.Windows.HorizontalAlignment)value;

    //                var handler = PropertyChanged;
    //                if (handler != null)
    //                {
    //                    handler(this, new PropertyChangedEventArgs("HorizontalAlignment"));
    //                }

    //                var parent = this.GetParent<IPairable>() as FrameworkElement;
    //                if (parent != null)
    //                {
    //                    parent.InvalidateMeasure();
    //                }
    //            }
    //        }
    //    }

    //    public new iFactr.UI.VerticalAlignment VerticalAlignment
    //    {
    //        get { return (iFactr.UI.VerticalAlignment)base.VerticalAlignment; }
    //        set
    //        {
    //            if (value != VerticalAlignment)
    //            {
    //                base.VerticalAlignment = (System.Windows.VerticalAlignment)value;

    //                var handler = PropertyChanged;
    //                if (handler != null)
    //                {
    //                    handler(this, new PropertyChangedEventArgs("VerticalAlignment"));
    //                }

    //                var parent = this.GetParent<IPairable>() as FrameworkElement;
    //                if (parent != null)
    //                {
    //                    parent.InvalidateMeasure();
    //                }
    //            }
    //        }
    //    }

    //    public new UI.Visibility Visibility
    //    {
    //        get { return (UI.Visibility)base.Visibility; }
    //        set
    //        {
    //            if (value != Visibility)
    //            {
    //                base.Visibility = (System.Windows.Visibility)value;

    //                var handler = PropertyChanged;
    //                if (handler != null)
    //                {
    //                    handler(this, new PropertyChangedEventArgs("Visibility"));
    //                }

    //                var parent = this.GetParent<IPairable>() as FrameworkElement;
    //                if (parent != null)
    //                {
    //                    parent.InvalidateMeasure();
    //                }
    //            }
    //        }
    //    }

    //    public IPairable Pair
    //    {
    //        get { return pair; }
    //        set
    //        {
    //            if (pair == null && value != null)
    //            {
    //                pair = value;
    //                pair.Pair = this;
    //            }
    //        }
    //    }
    //    private IPairable pair;

    //    public Grid()
    //    {
    //        Columns = new ColumnCollection();
    //        Rows = new RowCollection();
    //    }

    //    public void AddChild(IElement element)
    //    {
    //        var e = WpfFactory.GetNativeObject<FrameworkElement>(element, "element", false);
    //        if (e != null)
    //        {
    //            if (e.Parent is Panel)
    //            {
    //                ((Panel)e.Parent).Children.Remove(e);
    //            }

    //            base.Children.Add(e);

    //            var handler = PropertyChanged;
    //            if (handler != null)
    //            {
    //                handler(this, new PropertyChangedEventArgs("Children"));
    //            }
    //        }
    //    }

    //    public void RemoveChild(IElement element)
    //    {
    //        var e = WpfFactory.GetNativeObject<FrameworkElement>(element, "element", true);
    //        if (e != null)
    //        {
    //            base.Children.Remove(e);

    //            var handler = PropertyChanged;
    //            if (handler != null)
    //            {
    //                handler(this, new PropertyChangedEventArgs("Children"));
    //            }
    //        }
    //    }

    //    public Size Measure(Size constraints)
    //    {
    //        return this.PerformLayout(Size.Empty, constraints);
    //    }

    //    public void SetLocation(Point location, Size size)
    //    {
    //        Width = size.Width;
    //        Height = size.Height;
    //        Canvas.SetLeft(this, location.X);
    //        Canvas.SetTop(this, location.Y);
    //    }

    //    public bool Equals(IElement other)
    //    {
    //        var control = other as Element;
    //        if (control != null)
    //        {
    //            return control.Equals(this);
    //        }

    //        return base.Equals(other);
    //    }

    //    //protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
    //    //{
    //    //    var size = this.PerformLayout(Size.Empty, new Size(constraint.Width, constraint.Height));
    //    //    return new System.Windows.Size(size.Width, size.Height);
    //    //}

    //    private static IList<IElement> GetControls(DependencyObject parent)
    //    {
    //        var children = new List<IElement>();

    //        int count = VisualTreeHelper.GetChildrenCount(parent);
    //        for (int i = 0; i < count; i++)
    //        {
    //            var control = VisualTreeHelper.GetChild(parent, i);
    //            if (control is IElement)
    //            {
    //                children.Add((IElement)control);
    //            }
    //            else if (control is FrameworkElement)
    //            {
    //                var element = (FrameworkElement)control;
    //                if (element.Tag is IElement)
    //                {
    //                    children.Add((IElement)element.Tag);
    //                }
    //            }

    //            children.AddRange(GetControls(control));
    //        }

    //        return children;
    //    }
    //}
}
