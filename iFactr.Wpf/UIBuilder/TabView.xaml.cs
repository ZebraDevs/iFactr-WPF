using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using iFactr.Core;
using iFactr.UI;

namespace iFactr.Wpf
{
    public partial class TabView : TabControl, ITabView, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler Rendering;

        public new double Height
        {
            get { return ActualHeight; }
        }

        public new double Width
        {
            get { return ActualWidth; }
        }

        public Color SelectionColor
        {
            get { return selectionColor; }
            set
            {
                value = value.IsDefaultColor ? new Color(212, 223, 238) : value;
                if (value != selectionColor)
                {
                    selectionColor = value;
                    if (ItemsSource != null)
                    {
                        var brush = value.GetBrush();
                        foreach (Control control in ItemsSource)
                        {
                            if (control != null)
                            {
                                control.Foreground = brush;
                            }
                        }
                    }

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("SelectionColor"));
                    }
                }
            }
        }
        private Color selectionColor;

        public IEnumerable<ITabItem> TabItems
        {
            get
            {
                if (Items == null)
                {
                    yield break;
                }

                foreach (var item in Items.OfType<ITabItem>())
                {
                    yield return (item.Pair as ITabItem) ?? item;
                }
            }
            set
            {
                PaneManager.Instance.Clear(UI.Pane.Master);

                if (value == null)
                {
                    ItemsSource = null;
                }
                else
                {
                    int tab = 0;
                    ItemsSource = value.Select(i =>
                    {
                        PaneManager.Instance.AddStack(new Pane(tab.ToString()), new iApp.AppNavigationContext() { ActiveTab = tab++ });
                        var tabItem = WpfFactory.GetNativeObject<System.Windows.Controls.TabItem>(i, "tabItem", false);
                        tabItem.Foreground = selectionColor.IsDefaultColor ? new Color(212, 223, 238).GetBrush() : selectionColor.GetBrush();
                        return tabItem;
                    });
                }

                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs("TabItems"));
                }
            }
        }

        public Color HeaderColor
        {
            get { return headerColor; }
            set
            {
                if (value != headerColor)
                {
                    headerColor = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("HeaderColor"));
                    }
                }
            }
        }
        private Color headerColor;

        public MetadataCollection Metadata
        {
            get { return metadata ?? (metadata = new MetadataCollection()); }
        }
        private MetadataCollection metadata;

        public PreferredOrientation PreferredOrientations
        {
            get { return preferredOrientations; }
            set
            {
                if (value != preferredOrientations)
                {
                    preferredOrientations = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("PreferredOrientations"));
                    }
                }
            }
        }
        private PreferredOrientation preferredOrientations;

        public string Title
        {
            get { return title; }
            set
            {
                if (value != title)
                {
                    title = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Title"));
                    }
                }
            }
        }
        private string title;

        public Color TitleColor
        {
            get { return titleColor; }
            set
            {
                if (value != titleColor)
                {
                    titleColor = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("TitleColor"));
                    }
                }
            }
        }
        private Color titleColor;

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

        public Type ModelType
        {
            get { return model == null ? null : model.GetType(); }
        }

        private object model;

        public TabView()
        {
            InitializeComponent();

            SizeChanged += (o, e) =>
            {
                if (e.PreviousSize.Width != e.NewSize.Width)
                {
                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Width"));
                    }
                }

                if (e.PreviousSize.Height != e.NewSize.Height)
                {
                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Height"));
                    }
                }
            };

            Loaded += (o, e) =>
            {
                if (SelectedIndex != 0)
                {
                    int selectedIndex = SelectedIndex;
                    SelectedIndex = -1;
                    SelectedIndex = selectedIndex;
                }
            };
        }

        public object GetModel()
        {
            return model;
        }

        public void SetModel(object model)
        {
            this.model = model;
        }

        public void Render()
        {
            var handler = Rendering;
            if (handler != null)
            {
                handler(Pair ?? this, EventArgs.Empty);
            }
        }

        public async void SetBackground(string imagePath, ContentStretch stretch)
        {
            var brush = await imagePath.GetImageBrush();
            brush.Stretch = (System.Windows.Media.Stretch)stretch;
            Background = brush;
        }

        public void SetBackground(Color color)
        {
            Background = color.GetBrush();
        }

        public bool Equals(IView other)
        {
            var view = other as iFactr.UI.View;
            if (view != null)
            {
                return view.Equals(this);
            }

            return base.Equals(other);
        }
    }
}
