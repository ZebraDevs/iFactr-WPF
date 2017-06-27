using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using iFactr.Core;
using iFactr.Core.Layers;
using iFactr.UI;
using MonoCross.Navigation;

namespace iFactr.Wpf
{
    public class PopoverPane : Pane
    {
        public static Window PopoverWindow { get; private set; }

        public PopoverPane()
            : base("Popover")
        {
            if (PopoverWindow == null)
            {
                PopoverWindow = new Window()
                {
                    Content = this,
                    Title = iApp.Instance.Title,
                    Width = 800,
                    Height = 600,
                    Owner = Application.Current.MainWindow,
                    ResizeMode = ResizeMode.NoResize,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };

                PopoverWindow.Closing += (o, e) =>
                {
                    e.Cancel = true;
                    PopoverWindow.Hide();

                    if (Application.Current.MainWindow != null)
                    {
                        Application.Current.MainWindow.IsHitTestVisible = PopoverWindow.IsHitTestVisible;
                        Application.Current.MainWindow.Focusable = PopoverWindow.Focusable;
                        Application.Current.MainWindow.Activate();
                    }
                };
            }

            PopoverWindow.IsVisibleChanged += (o, e) =>
            {
                if (!PopoverWindow.IsVisible)
                {
                    ViewStack.Clear();
                    Content = null;
                }
            };
        }

        public override IMXView[] PopToRoot()
        {
            var views = Views.ToArray();

            if (PopoverWindow.IsVisible)
            {
                PopoverWindow.Close();
            }

            return views;
        }

        public override IMXView PopView()
        {
            if (Views.Count() < 2)
            {
                var views = CurrentView;

                if (PopoverWindow.IsVisible)
                {
                    PopoverWindow.Close();
                }

                return views;
            }
            else
            {
                return base.PopView();
            }
        }

        public override void PushView(IMXView view)
        {
            base.PushView(view);

            var entry = view as IHistoryEntry;
            if (entry != null)
            {
                if (entry.PopoverPresentationStyle == PopoverPresentationStyle.FullScreen)
                {
                    PopoverWindow.WindowState = WindowState.Maximized;
                }
                else
                {
                    PopoverWindow.WindowState = WindowState.Normal;

                    if (!PopoverWindow.RestoreBounds.IsEmpty)
                    {
                        PopoverWindow.Width = PopoverWindow.RestoreBounds.Width;
                        PopoverWindow.Height = PopoverWindow.RestoreBounds.Height;

                        // this may look weird, but the property won't work unless its value undergoes a change.
                        PopoverWindow.Left = PopoverWindow.RestoreBounds.Left + 1;
                        PopoverWindow.Left--;

                        PopoverWindow.Top = PopoverWindow.RestoreBounds.Top + 1;
                        PopoverWindow.Top--;
                    }
                }
            }

            if (!PopoverWindow.IsVisible)
            {
                if (Application.Current.MainWindow != null)
                {
                    Application.Current.MainWindow.IsHitTestVisible = false;
                    Application.Current.MainWindow.Focusable = false;
                }

                PopoverWindow.Show();
            }
        }
    }

    public class Pane : ContentControl, IHistoryStack
    {
        public string ID { get; private set; }

        public IEnumerable<IMXView> Views
        {
            get
            {
                foreach (var view in ViewStack)
                {
                    var pair = view as IPairable;
                    yield return pair == null ? view : (pair.Pair as IMXView) ?? view;
                }
            }
        }

        public IEnumerable<iLayer> History
        {
            get { return ViewStack.Select(v => v.GetModel() as iLayer); }
        }

        public iLayer CurrentLayer
        {
            get { return ViewStack.Count == 0 ? null : ViewStack.Last().GetModel() as iLayer; }
        }

        public IMXView CurrentView
        {
            get
            {
                var pairable = Content as IPairable;
                if (pairable != null)
                {
                    return (pairable.Pair as IMXView) ?? pairable as IMXView;
                }

                return Content as IMXView;
            }
        }

        protected List<IMXView> ViewStack;

        public Pane(string id)
        {
            ID = id;
            IsTabStop = false;
            Wpf.Clip.SetToBounds(this, true);

            ViewStack = new List<IMXView>();
        }

        public void PopToLayer(iLayer layer)
        {
            if (layer != null)
            {
                PopToView(layer.View);
            }
        }

        public iLayer Peek()
        {
            return History.Last();
        }

        public void PushCurrent()
        {
        }

        public void Clear(iLayer layer)
        {
            PopToRoot();
        }

        public void InsertView(int index, IMXView view)
        {
            Parameter.CheckIndex(ViewStack, "history", index);
            view = (IMXView)WpfFactory.GetNativeObject<UIElement>(view, "view", false);

            ViewStack.Insert(index, view);
            if (index == ViewStack.Count - 1)
            {
                Content = view;
            }
        }

        public virtual IMXView[] PopToRoot()
        {
            IMXView[] views = null;
            if (ViewStack.Count > 1)
            {
                views = ViewStack.GetRange(1, ViewStack.Count - 1).Select(v =>
                {
                    var p = v as IPairable;
                    return p == null ? v : (p.Pair as IMXView) ?? v;
                }).ToArray();

                ViewStack.RemoveRange(1, ViewStack.Count - 1);
            }

            if (ViewStack.Count > 0)
            {
                Content = ViewStack.Last();
            }

            return views;
        }

        public IMXView[] PopToView(IMXView view)
        {
            view = (IMXView)WpfFactory.GetNativeObject<UIElement>(view, "view", false);
            Parameter.CheckObjectExists(ViewStack, "history", view, "view");

            List<IMXView> views = null;
            if (ViewStack.Last() != view)
            {
                views = new List<IMXView>();
                while (ViewStack.Count > 1 && ViewStack[ViewStack.Count - 1] != view)
                {
                    var last = ViewStack.Last();
                    var pair = last as IPairable;
                    if (pair == null)
                    {
                        views.Add(last);
                    }
                    else
                    {
                        views.Add((pair.Pair as IMXView) ?? (IMXView)pair);
                    }

                    ViewStack.RemoveAt(ViewStack.Count - 1);
                }

                Content = ViewStack.Last();
            }

            return views == null ? null : views.ToArray();
        }

        public virtual IMXView PopView()
        {
            var parent = this.GetParent<ITabView>() as Page;

            IMXView view = null;
            if (ViewStack.Count > 1)
            {
                view = ViewStack.Last();
                var pair = view as IPairable;
                if (pair != null)
                {
                    view = (pair.Pair as IMXView) ?? view;
                }

                ViewStack.RemoveAt(ViewStack.Count - 1);
                Content = ViewStack.Last();
            }

            return view;
        }

        public virtual void PushView(IMXView view)
        {
            ViewStack.Add((IMXView)WpfFactory.GetNativeObject<UIElement>(view, "view", false));
            Content = ViewStack.Last();
        }

        public void ReplaceView(IMXView currentView, IMXView newView)
        {
            currentView = (IMXView)WpfFactory.GetNativeObject<UIElement>(currentView, "currentView", false);
            int index = Parameter.CheckObjectExists(ViewStack, "history", currentView, "currentView");

            ViewStack[index] = (IMXView)WpfFactory.GetNativeObject<UIElement>(newView, "newView", false);

            if (index == ViewStack.Count - 1)
            {
                Content = ViewStack.Last();
            }
        }
    }
}