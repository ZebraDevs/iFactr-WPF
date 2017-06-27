using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using iFactr.Core;
using iFactr.UI;
using iFactr.UI.Controls;
using Thickness = System.Windows.Thickness;

namespace iFactr.Wpf
{
    public class GridView : BaseView, IGridView
    {
        public event SubmissionEventHandler Submitting;

        public bool HorizontalScrollingEnabled
        {
            get { return scrollViewer.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled; }
            set
            {
                if (value != HorizontalScrollingEnabled)
                {
                    scrollViewer.HorizontalScrollBarVisibility = value ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
                    OnPropertyChanged("HorizontalScrollingEnabled");
                }
            }
        }

        public bool VerticalScrollingEnabled
        {
            get { return scrollViewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled; }
            set
            {
                if (value != VerticalScrollingEnabled)
                {
                    scrollViewer.VerticalScrollBarVisibility = value ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
                    OnPropertyChanged("VerticalScrollingEnabled");
                }
            }
        }

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

        public ColumnCollection Columns { get; private set; }

        public new IEnumerable<IElement> Children
        {
            get
            {
                var controls = GetControls(canvas).Select(c => (c.Pair as IElement) ?? c);
                foreach (var control in controls)
                {
                    yield return control;
                }
            }
        }

        public UI.Thickness Padding
        {
            get { return padding; }
            set
            {
                if (value != padding)
                {
                    padding = value;
                    OnPropertyChanged("Padding");
                }
            }
        }
        private UI.Thickness padding;

        public RowCollection Rows { get; private set; }

        public ValidationErrorCollection ValidationErrors { get; private set; }

        private ScrollViewer scrollViewer;
        private Canvas canvas;

        public GridView()
        {
            Columns = new ColumnCollection();
            Rows = new RowCollection();
            ValidationErrors = new ValidationErrorCollection();

            scrollViewer = new ScrollViewer()
            {
                Content = (canvas = new Canvas() { Margin = new Thickness(1, 0, 2, 0) })
            };
            base.Children.Add(scrollViewer);
        }

        public void AddChild(IElement control)
        {
            var element = WpfFactory.GetNativeObject<FrameworkElement>(control, "element", false);
            if (element != null)
            {
                if (element.Parent is Panel)
                {
                    ((Panel)element.Parent).Children.Remove(element);
                }

                canvas.Children.Add(element);
                OnPropertyChanged("Children");
            }
        }

        public void RemoveChild(IElement control)
        {
            var element = WpfFactory.GetNativeObject<FrameworkElement>(control, "element", true);
            if (element != null)
            {
                canvas.Children.Remove(element);
                OnPropertyChanged("Children");
            }
        }

        public IDictionary<string, string> GetSubmissionValues()
        {
            var submitValues = new Dictionary<string, string>();
            foreach (var control in Children.OfType<IControl>().Where(c => c.ShouldSubmit()))
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

            return submitValues;
        }

        public void Submit(string url)
        {
            Submit(new Link(url));
        }

        public void Submit(Link link)
        {
            if (link.Parameters == null)
            {
                link.Parameters = new Dictionary<string, string>();
            }

            var submitValues = GetSubmissionValues();
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

        protected override void OnRender()
        {
            InvalidateMeasure();
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            var minSize = new UI.Size(constraint.Width - (canvas.Margin.Left + canvas.Margin.Right),
                constraint.Height - Math.Max(HeaderBar.ActualHeight, 40) - (canvas.Margin.Top + canvas.Margin.Bottom));

            var maxSize = new UI.Size(HorizontalScrollingEnabled ? double.PositiveInfinity : minSize.Width,
                VerticalScrollingEnabled ? double.PositiveInfinity : minSize.Height);

            var size = this.PerformLayout(minSize, maxSize);
            canvas.Width = size.Width;
            canvas.Height = size.Height;

            return base.MeasureOverride(constraint);
        }

        private static IList<IElement> GetControls(DependencyObject parent)
        {
            var children = new List<IElement>();

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var control = VisualTreeHelper.GetChild(parent, i);
                if (control is IElement)
                {
                    children.Add((IElement)control);
                }
                else if (control is FrameworkElement)
                {
                    var element = (FrameworkElement)control;
                    if (element.Tag is IElement)
                    {
                        children.Add((IElement)element.Tag);
                    }
                }
            }

            return children;
        }
    }
}
