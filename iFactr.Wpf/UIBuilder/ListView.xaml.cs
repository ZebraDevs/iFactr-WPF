using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using iFactr.Core;
using iFactr.UI;
using iFactr.UI.Controls;
using Binding = System.Windows.Data.Binding;
using Thickness = System.Windows.Thickness;

namespace iFactr.Wpf
{
    public partial class ListView : BaseView, IListView
    {
        public event EventHandler Scrolled;

        public event SubmissionEventHandler Submitting;

        public static DependencyProperty VerticalOffsetProperty = DependencyProperty.RegisterAttached("VerticalOffset",
            typeof(double), typeof(ListView), new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));

        public static void SetVerticalOffset(FrameworkElement target, double value)
        {
            target.SetValue(VerticalOffsetProperty, value);
        }

        public static double GetVerticalOffset(FrameworkElement target)
        {
            return (double)target.GetValue(VerticalOffsetProperty);
        }

        private static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = target as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }

        public ColumnMode ColumnMode
        {
            get { return columnMode; }
            set
            {
                if (value != columnMode)
                {
                    columnMode = value;
                    OnPropertyChanged("ColumnMode");
                }
            }
        }
        private ColumnMode columnMode;

        public IMenu Menu
        {
            get
            {
                var menu = HeaderBar.Children.OfType<IMenu>().FirstOrDefault();
                return menu == null ? null : (menu.Pair as IMenu) ?? menu;
            }
            set
            {
                if (value != Menu)
                {
                    for (int i = HeaderBar.Children.Count - 1; i >= 0; i--)
                    {
                        if (HeaderBar.Children[i] is IMenu)
                        {
                            HeaderBar.Children.RemoveAt(i);
                        }
                    }

                    var menu = WpfFactory.GetNativeObject<UIElement>(value, "menu", true);
                    if (menu != null)
                    {
                        System.Windows.Controls.Grid.SetColumn(menu, HeaderBar.ColumnDefinitions.Count - 1);
                        HeaderBar.Children.Add(menu);
                    }

                    OnPropertyChanged("Menu");
                }
            }
        }

        public ISearchBox SearchBox
        {
            get
            {
                var box = searchBox as ISearchBox;
                return box == null ? null : (box.Pair ?? box) as ISearchBox;
            }
            set
            {
                if (value != SearchBox)
                {
                    if (searchBox != null)
                    {
                        Children.Remove(searchBox);
                    }

                    searchBox = WpfFactory.GetNativeObject<FrameworkElement>(value, "searchBox", true);
                    if (searchBox != null)
                    {
                        SetDock(searchBox, Dock.Top);
                        Children.Insert(1, searchBox);
                    }

                    OnPropertyChanged("SearchBox");
                }
            }
        }
        private FrameworkElement searchBox;

        public UI.Color SeparatorColor
        {
            get { return separatorColor; }
            set
            {
                if (value != separatorColor)
                {
                    separatorColor = value;
                    OnPropertyChanged("SeparatorColor");
                }
            }
        }
        private UI.Color separatorColor;

        public new ListViewStyle Style { get; private set; }

        public SectionCollection Sections { get; private set; }

        public ValidationErrorCollection ValidationErrors { get; private set; }

        public CellDelegate CellRequested
        {
            get { return cellRequested; }
            set
            {
                if (value != cellRequested)
                {
                    cellRequested = value;
                    OnPropertyChanged("CellRequested");
                }
            }
        }
        private CellDelegate cellRequested;

        public ItemIdDelegate ItemIdRequested
        {
            get { return itemIdRequested; }
            set
            {
                if (value != itemIdRequested)
                {
                    itemIdRequested = value;
                    OnPropertyChanged("ItemIdRequested");
                }
            }
        }
        private ItemIdDelegate itemIdRequested;

        private Dictionary<string, string> submitValues;
        private ListBox firstColumnItems, secondColumnItems;
        private System.Windows.Controls.Grid container;
        private TextBlock measuringBlock;

        public ListView()
            : this(ListViewStyle.Default)
        {

        }

        public ListView(ListViewStyle style)
        {
            InitializeComponent();
            Style = style;
            
            Sections = new SectionCollection();
            ValidationErrors = new ValidationErrorCollection();
            submitValues = new Dictionary<string, string>();

            measuringBlock = new TextBlock() { TextWrapping = TextWrapping.Wrap };

            container = new System.Windows.Controls.Grid()
            {
                ColumnDefinitions = { new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) } }
            };
            Children.Add(container);
            
            firstColumnItems = new ListBox()
            {
                BorderThickness = new Thickness(0),
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                ItemContainerStyle = (Style)Resources["ListBoxItemStyle"],
                ItemTemplateSelector = new CellTemplateSelector(this),
                IsEnabled = false
            };

            firstColumnItems.SizeChanged += (o, e) =>
            {
                ReloadSections();
            };

            KeyboardNavigation.SetTabNavigation(firstColumnItems, KeyboardNavigationMode.Continue);
            ScrollViewer.SetHorizontalScrollBarVisibility(firstColumnItems, ScrollBarVisibility.Disabled);
            container.Children.Add(firstColumnItems);

            Loaded += (o, e) =>
            {
                if (!firstColumnItems.IsEnabled)
                {
                    firstColumnItems.IsEnabled = true;

                    var scroller = firstColumnItems.FindChild<ScrollViewer>(null);
                    if (scroller != null)
                    {
                        scroller.ScrollChanged += (obj, args) =>
                        {
                            var handler = Scrolled;
                            if (handler != null)
                            {
                                handler(Pair ?? this, EventArgs.Empty);
                            }
                        };
                    }

                    ReloadSections();

                    // the dispatcher call may look weird, but without it, animated ScrollToCell calls that happen on Render put the list in the wrong position
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var scrollPosition = Metadata.Get<object[]>("wpfInitialScrollPosition");
                        if (scrollPosition != null)
                        {
                            Metadata.Remove("wpfInitialScrollPosition");
                            ScrollToCell((int)scrollPosition[0], (int)scrollPosition[1], (bool)scrollPosition[2]);
                        }
                    }));
                }
            };
        }

        public IDictionary<string, string> GetSubmissionValues()
        {
            foreach (var cell in firstColumnItems.GetChildren<IGridCell>())
            {
                SetSubmitValue(cell);
            }

            if (secondColumnItems != null)
            {
                foreach (var cell in secondColumnItems.GetChildren<IGridCell>())
                {
                    SetSubmitValue(cell);
                }
            }

            return new Dictionary<string, string>(submitValues);
        }

        public IEnumerable<ICell> GetVisibleCells()
        {
            var cells = firstColumnItems.GetChildren<ICell>().Select(c => (c.Pair as ICell) ?? c);
            if (secondColumnItems != null)
            {
                cells = cells.Concat(secondColumnItems.GetChildren<ICell>().Select(c => (c.Pair as ICell) ?? c));
            }
            return cells;
        }
        
        public void ReloadSections()
        {
            if (!firstColumnItems.IsEnabled)
                return;

            if (ColumnMode == UI.ColumnMode.TwoColumns)
            {
                if (secondColumnItems == null)
                {
                    secondColumnItems = new ListBox()
                    {
                        HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                        ItemContainerStyle = (Style)Resources["ListBoxItemStyle"],
                        ItemTemplateSelector = new CellTemplateSelector(this),
                        BorderThickness = new Thickness(0),
                        Background = firstColumnItems.Background,
                        Padding = new Thickness(8, 0, 10, 0)
                    };
                    System.Windows.Controls.Grid.SetColumn(secondColumnItems, 1);
                    ScrollViewer.SetHorizontalScrollBarVisibility(secondColumnItems, ScrollBarVisibility.Disabled);
                    ScrollViewer.SetCanContentScroll(secondColumnItems, false);
                    KeyboardNavigation.SetTabNavigation(secondColumnItems, KeyboardNavigationMode.Continue);

                    secondColumnItems.SelectionChanged += (o, e) =>
                    {
                        if (e.AddedItems.Count > 0)
                        {
                            firstColumnItems.UnselectAll();
                        }
                    };

                    firstColumnItems.SelectionChanged += (o, e) =>
                    {
                        if (e.AddedItems.Count > 0 && secondColumnItems != null)
                        {
                            secondColumnItems.UnselectAll();
                        }
                    };
                }

                if (!container.Children.Contains(secondColumnItems))
                {
                    container.ColumnDefinitions.Insert(1, new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    container.Children.Add(secondColumnItems);

                    ScrollViewer.SetVerticalScrollBarVisibility(firstColumnItems, ScrollBarVisibility.Hidden);
                    ScrollViewer.SetCanContentScroll(firstColumnItems, false);

                    var scroller = firstColumnItems.FindChild<ScrollViewer>(null);
                    if (scroller != null)
                    {
                        scroller.ScrollChanged -= SyncScrollBars;
                        scroller.ScrollChanged += SyncScrollBars;
                    }

                    secondColumnItems.ApplyTemplate();
                    scroller = secondColumnItems.FindChild<ScrollViewer>(null);
                    if (scroller != null)
                    {
                        scroller.ScrollChanged -= SyncScrollBars;
                        scroller.ScrollChanged += SyncScrollBars;
                    }
                }

                List<object> firstItems = new List<object>();
                for (int i = 0; i < Sections.Count; i += 2)
                {
                    Section section = Sections[i];
                    firstItems.Add(new HeaderFooter() { Element = WpfFactory.GetNativeObject<UIElement>(section.Header, "Header", true) });

                    for (int j = 0; j < section.ItemCount; j++)
                    {
                        var sc = new SectionCell() { CellIndex = j, SectionIndex = i };
                        if (section.CellRequested != null)
                        {
                            sc.SectionCellRequested = section.CellRequested;
                        }
                        else
                        {
                            sc.CellRequested = CellRequested;
                        }

                        firstItems.Add(sc);
                    }

                    firstItems.Add(new HeaderFooter() { Element = WpfFactory.GetNativeObject<UIElement>(section.Footer, "Footer", true) });
                }

                firstColumnItems.ItemsSource = firstItems;
                firstColumnItems.Padding = new Thickness(10, 0, 8 + SystemParameters.ScrollWidth, 0);

                List<object> secondItems = new List<object>();
                for (int i = 1; i < Sections.Count; i += 2)
                {
                    Section section = Sections[i];
                    secondItems.Add(new HeaderFooter() { Element = WpfFactory.GetNativeObject<UIElement>(section.Header, "Header", true) });

                    for (int j = 0; j < section.ItemCount; j++)
                    {
                        var sc = new SectionCell() { CellIndex = j, SectionIndex = i };
                        if (section.CellRequested != null)
                        {
                            sc.SectionCellRequested = section.CellRequested;
                        }
                        else
                        {
                            sc.CellRequested = CellRequested;
                        }

                        secondItems.Add(sc);
                    }

                    secondItems.Add(new HeaderFooter() { Element = WpfFactory.GetNativeObject<UIElement>(section.Footer, "Footer", true) });
                }

                secondColumnItems.ItemsSource = secondItems;
            }
            else
            {
                if (secondColumnItems != null && container.Children.Contains(secondColumnItems))
                {
                    container.Children.Remove(secondColumnItems);
                    container.ColumnDefinitions.RemoveAt(1);

                    ScrollViewer.SetVerticalScrollBarVisibility(firstColumnItems, ScrollBarVisibility.Auto);
                    ScrollViewer.SetCanContentScroll(firstColumnItems, true);

                    var scroller = firstColumnItems.FindChild<ScrollViewer>(null);
                    if (scroller != null)
                    {
                        scroller.ScrollChanged -= SyncScrollBars;
                    }

                    scroller = secondColumnItems.FindChild<ScrollViewer>(null);
                    if (scroller != null)
                    {
                        scroller.ScrollChanged -= SyncScrollBars;
                    }

                    secondColumnItems = null;
                }

                var items = new List<object>();
                for (int i = 0; i < Sections.Count; i++)
                {
                    Section section = Sections[i];
                    items.Add(new HeaderFooter() { Element = WpfFactory.GetNativeObject<UIElement>(section.Header, "Header", true) });

                    for (int j = 0; j < section.ItemCount; j++)
                    {
                        var sc = new SectionCell() { CellIndex = j, SectionIndex = i };
                        if (section.CellRequested != null)
                        {
                            sc.SectionCellRequested = section.CellRequested;
                        }
                        else
                        {
                            sc.CellRequested = CellRequested;
                        }

                        items.Add(sc);
                    }

                    items.Add(new HeaderFooter() { Element = WpfFactory.GetNativeObject<UIElement>(section.Footer, "Footer", true) });
                }

                firstColumnItems.ItemsSource = items;
                firstColumnItems.Padding = new Thickness(0);
            }
        }

        public void ScrollToCell(int section, int index, bool animated)
        {
            var listBox = secondColumnItems != null && section % 2 == 1 ? secondColumnItems : firstColumnItems;
            if (listBox.ItemsSource == null)
            {
                Metadata["wpfInitialScrollPosition"] = new object[] { section, index, animated };
                return;
            }

            var cell = listBox.ItemsSource.OfType<SectionCell>().FirstOrDefault(sc => sc.SectionIndex == section && sc.CellIndex == index);
            if (cell == null)
            {
                return;
            }

            if (animated)
            {
                ScrollIntoView(listBox, cell);
            }
            else
            {
                listBox.ScrollIntoView(cell);
            }
        }

        public void ScrollToEnd(bool animated)
        {
            var listBox = firstColumnItems;
            if (secondColumnItems != null)
            {
                var scroller1 = firstColumnItems.FindChild<ScrollViewer>(null);
                var scroller2 = secondColumnItems.FindChild<ScrollViewer>(null);

                listBox = scroller1.ExtentHeight < scroller2.ExtentHeight ? secondColumnItems : firstColumnItems;
            }

            if (!listBox.ItemsSource.Any())
            {
                return;
            }

            if (animated)
            {
                ScrollIntoView(listBox, listBox.ItemsSource.LastOrDefault());
            }
            else
            {
                listBox.ScrollIntoView(listBox.ItemsSource.LastOrDefault());
            }
        }

        public void ScrollToHome(bool animated)
        {
            if (!firstColumnItems.ItemsSource.Any())
            {
                return;
            }

            if (animated)
            {
                ScrollIntoView(firstColumnItems, firstColumnItems.ItemsSource.FirstOrDefault());
            }
            else
            {
                firstColumnItems.ScrollIntoView(firstColumnItems.ItemsSource.FirstOrDefault());
            }
        }

        public void Submit(Link link)
        {
            if (link.Parameters == null)
            {
                link.Parameters = new Dictionary<string, string>();
            }

            foreach (var cell in firstColumnItems.GetChildren<IGridCell>())
            {
                SetSubmitValue(cell);
            }

            if (secondColumnItems != null)
            {
                foreach (var cell in secondColumnItems.GetChildren<IGridCell>())
                {
                    SetSubmitValue(cell);
                }
            }

            var args = new SubmissionEventArgs(link, ValidationErrors);

            var handler = Submitting;
            if (handler != null)
            {
                handler(Pair ?? this, args);
            }

            if (args.Cancel)
                return;

            foreach (string id in submitValues.Keys)
            {
                link.Parameters[id] = submitValues[id];
            }

            iApp.Navigate(link, this);
        }

        public void Submit(string url)
        {
            Submit(new Link(url));
        }

        public override void SetBackground(UI.Color color)
        {
            base.SetBackground(color);
            firstColumnItems.Background = color.IsDefaultColor ? null : color.GetBrush();

            if (secondColumnItems != null)
            {
                secondColumnItems.Background = color.IsDefaultColor ? null : color.GetBrush();
            }
        }

        protected override void OnRender()
        {
            submitValues.Clear();
            ReloadSections();
        }

        private void ScrollIntoView(ListBox listBox, object item)
        {
            var scroller = listBox.FindChild<ScrollViewer>(null);
            double toValue = 0;

            if (scroller.CanContentScroll)
            {
                int index = listBox.ItemsSource.IndexOf(item);
                if (index >= scroller.VerticalOffset && index < scroller.VerticalOffset + scroller.ViewportHeight)
                {
                    return;
                }

                toValue = index;
                if (index >= scroller.VerticalOffset + scroller.ViewportHeight)
                {
                    toValue -= scroller.ViewportHeight - 1;
                }
            }
            else
            {
                var element = listBox.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
                var point = element.TranslatePoint(new System.Windows.Point(), scroller);
                if (point.Y >= 0 && point.Y <= scroller.ViewportHeight - element.RenderSize.Height)
                {
                    return;
                }

                toValue = scroller.VerticalOffset + point.Y;
                if (point.Y > scroller.ViewportHeight - element.RenderSize.Height)
                {
                    toValue -= (scroller.ViewportHeight - element.RenderSize.Height);
                }
            }

            var verticalAnimation = new DoubleAnimation();
            verticalAnimation.From = scroller.VerticalOffset;
            verticalAnimation.To = toValue;
            verticalAnimation.DecelerationRatio = .2;
            verticalAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(333));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(verticalAnimation);
            Storyboard.SetTarget(verticalAnimation, scroller);
            Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ListView.VerticalOffsetProperty));

            storyboard.Completed += (o, e) =>
            {
                // doing this accounts for an issue that may crop up when scrolling
                // downward through a virtualized list with variable-height items
                listBox.ScrollIntoView(item);
            };

            storyboard.Begin();
        }

        private void SetSubmitValue(IGridCell cell)
        {
            foreach (var control in cell.Children.OfType<IControl>().Where(c => c.ShouldSubmit()))
            {
                string[] errors;
                if (!control.Validate(out errors))
                {
                    ValidationErrors[control.SubmitKey] = errors;
                }
                else
                {
                    ValidationErrors.Remove(control.SubmitKey);
                }

                submitValues[control.SubmitKey] = control.StringValue;
            }
        }

        private void SyncScrollBars(object sender, ScrollChangedEventArgs e)
        {
            var scroller1 = firstColumnItems.FindChild<ScrollViewer>(null);
            var scroller2 = secondColumnItems.FindChild<ScrollViewer>(null);

            var content1 = scroller1 == null ? null : scroller1.Content as FrameworkElement;
            var content2 = scroller2 == null ? null : scroller2.Content as FrameworkElement;

            if (content1 != null && content2 != null)
            {
                var height = Math.Max(content1.ActualHeight, content2.ActualHeight);
                if (content1.ActualHeight < height)
                {
                    content1.Height = height;
                }

                if (content2.ActualHeight < height)
                {
                    content2.Height = height;
                }

                scroller1.ScrollToVerticalOffset(e.VerticalOffset);
                scroller2.ScrollToVerticalOffset(e.VerticalOffset);
            }
            else
            {
                if (scroller1 != null)
                {
                    scroller1.ScrollToVerticalOffset(e.VerticalOffset);
                }

                if (scroller2 != null)
                {
                    scroller2.ScrollToVerticalOffset(e.VerticalOffset);
                }
            }
        }

        private class CellTemplateSelector : DataTemplateSelector
        {
            private static readonly CellRetriever Converter = new CellRetriever();

            private ListView view;

            public CellTemplateSelector(ListView view)
            {
                this.view = view;
            }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                var contentControl = new FrameworkElementFactory(typeof(ContentControl));
                contentControl.SetBinding(ContentControl.ContentProperty, new Binding(".") { Converter = Converter, ConverterParameter = view });

                return new DataTemplate() { VisualTree = contentControl };
            }
        }

        private class CellRetriever : System.Windows.Data.IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value is HeaderFooter)
                {
                    return ((HeaderFooter)value).Element;
                }
                if (value is SectionCell)
                {
                    SectionCell sc = (SectionCell)value;

                    ICell cell = null;
                    if (sc.SectionCellRequested != null)
                    {
                        cell = sc.SectionCellRequested(sc.CellIndex, null);
                    }
                    else if (sc.CellRequested != null)
                    {
                        cell = sc.CellRequested(sc.SectionIndex, sc.CellIndex, null);
                    }
                    sc.Cell = cell;

                    CustomItemContainer container = cell as CustomItemContainer;
                    if (container != null)
                    {
                        var custom = container.CustomItem as FrameworkElement;
                        System.Windows.Controls.Grid.SetColumn(custom, sc.SectionIndex % 2);
                        return custom;
                    }

                    var element = WpfFactory.GetNativeObject<FrameworkElement>(cell, "cell", true);
                    if (element != null)
                    {
                        System.Windows.Controls.Grid.SetColumn(element, sc.SectionIndex % 2);
                        element.Unloaded += (o, e) =>
                        {
                            var gridCell = o as IGridCell;
                            var view = parameter as ListView;
                            if (view != null && gridCell != null)
                            {
                                view.SetSubmitValue(gridCell);
                            }
                        };
                    }

                    return element;
                }

                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }

    internal struct HeaderFooter
    {
        public object Element;
    }

    internal class SectionCell
    {
        public int CellIndex;

        public int SectionIndex;

        public SectionCellDelegate SectionCellRequested;

        public CellDelegate CellRequested;

        public ICell Cell;
    }
}
