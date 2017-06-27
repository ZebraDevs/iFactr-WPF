using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using iFactr.UI;

namespace iFactr.Wpf
{
    public class Toolbar : System.Windows.Controls.DockPanel, IToolbar, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Color BackgroundColor
        {
            get { return base.Background.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? new Color(240, 240, 240) : value;
                if (value != BackgroundColor)
                {
                    base.Background = value.GetBrush();

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("BackgroundColor"));
                    }
                }
            }
        }

        public Color ForegroundColor
        {
            get { return foregroundColor; }
            set
            {
                value = value.IsDefaultColor ? Color.Black : value;
                if (value != ForegroundColor)
                {
                    foreach (var item in PrimaryItems.Where(i => i.ForegroundColor.IsDefaultColor || i.ForegroundColor == foregroundColor))
                    {
                        item.ForegroundColor = value;
                    }

                    foreach (var item in SecondaryItems.Where(i => i.ForegroundColor.IsDefaultColor || i.ForegroundColor == foregroundColor))
                    {
                        item.ForegroundColor = value;
                    }

                    foregroundColor = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("ForegroundColor"));
                    }
                }
            }
        }
        private Color foregroundColor;

        public IEnumerable<IToolbarItem> PrimaryItems
        {
            get
            {
                foreach (var item in Children.OfType<UIElement>().Where(e => GetDock(e) == Dock.Right).OfType<IToolbarItem>())
                {
                    yield return (item.Pair as IToolbarItem) ?? item;
                }
            }
            set
            {
                for (int i = Children.Count - 1; i >= 0; i--)
                {
                    var child = Children[i];
                    if (child is IToolbarItem && GetDock(child) == Dock.Right)
                    {
                        Children.RemoveAt(i);
                    }
                }

                if (value != null)
                {
                    foreach (var item in value.Reverse())
                    {
                        var element = WpfFactory.GetNativeObject<FrameworkElement>(item, "toolbarItem", false);
                        SetDock(element, Dock.Right);
                        Children.Add(element);

                        if (item.ForegroundColor.IsDefaultColor)
                        {
                            item.ForegroundColor = ForegroundColor;
                        }
                    }
                }

                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs("PrimaryItems"));
                }
            }
        }

        public IEnumerable<IToolbarItem> SecondaryItems
        {
            get
            {
                foreach (var item in Children.OfType<UIElement>().Where(e => GetDock(e) == Dock.Left).OfType<IToolbarItem>())
                {
                    yield return (item.Pair as IToolbarItem) ?? item;
                }
            }
            set
            {
                for (int i = Children.Count - 1; i >= 0; i--)
                {
                    var child = Children[i];
                    if (child is IToolbarItem && GetDock(child) == Dock.Left)
                    {
                        Children.RemoveAt(i);
                    }
                }

                if (value != null)
                {
                    foreach (var item in value)
                    {
                        var element = WpfFactory.GetNativeObject<FrameworkElement>(item, "toolbarItem", false);
                        SetDock(element, Dock.Left);
                        Children.Add(element);

                        if (item.ForegroundColor.IsDefaultColor)
                        {
                            item.ForegroundColor = ForegroundColor;
                        }
                    }
                }

                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs("SecondaryItems"));
                }
            }
        }

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

        public Toolbar()
        {
            MinHeight = 20;
            LastChildFill = false;

            var line = new Line()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                StrokeThickness = 0.5,
                Stroke = new Color(135, 133, 133).GetBrush()
            };
            
            line.SetBinding(Line.X2Property, new System.Windows.Data.Binding("ActualWidth") { Source = this });
            Children.Add(line);
            DockPanel.SetDock(line, Dock.Top);
        }

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
