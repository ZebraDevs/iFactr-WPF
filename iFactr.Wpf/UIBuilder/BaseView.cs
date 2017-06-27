using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using iFactr.Core;
using iFactr.UI;
using Thickness = System.Windows.Thickness;

namespace iFactr.Wpf
{
    public abstract class BaseView : DockPanel, IView, INotifyPropertyChanged
    {
        public event EventHandler Activated;
        public event EventHandler Deactivated;
        public event EventHandler Rendering;

        public event PropertyChangedEventHandler PropertyChanged;

        #region IMXView members
        private object _model;

        /// <summary>
        /// The type of the model displayed by this view
        /// </summary>
        public Type ModelType
        {
            get { return _model == null ? null : _model.GetType(); }
        }

        public object GetModel()
        {
            return _model;
        }

        /// <summary>
        /// Sets the model for the view. An InvalidCastException may be thrown if a model of the wrong type is set
        /// </summary>
        /// <param name="model"></param>
        public void SetModel(object model)
        {
            _model = model;
        }
        #endregion

        public Link BackLink
        {
            get { return backLink; }
            set
            {
                if (value != backLink)
                {
                    backLink = value;
                    if (backLink == null)
                    {
                        backButton.ToolTip = iApp.Factory.GetResourceString("Back");
                    }
                    else
                    {
                        backButton.ToolTip = backLink.Text;
                    }

                    var stack = Stack;
                    if (stack != null)
                    {
                        backButton.Visibility = stack.DisplayBackButton(backLink) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                    }

                    OnPropertyChanged("BackLink");
                }
            }
        }
        private Link backLink;

        public UI.Color HeaderColor
        {
            get { return HeaderBar.Background.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? new UI.Color(248, 248, 248) : value;
                if (value != HeaderColor)
                {
                    HeaderBar.Background = value.GetBrush();
                    OnPropertyChanged("HeaderColor");
                }
            }
        }

        public MetadataCollection Metadata
        {
            get { return metadata ?? (metadata = new MetadataCollection()); }
        }
        private MetadataCollection metadata;

        public UI.Pane OutputPane
        {
            get { return outputPane; }
            set
            {
                if (value != outputPane)
                {
                    outputPane = value;
                    OnPropertyChanged("OutputPane");
                }
            }
        }
        private UI.Pane outputPane;

        public PopoverPresentationStyle PopoverPresentationStyle
        {
            get { return popoverPresentationStyle; }
            set
            {
                if (value != popoverPresentationStyle)
                {
                    popoverPresentationStyle = value;
                    OnPropertyChanged("PopoverPresentationStyle");
                }
            }
        }
        private PopoverPresentationStyle popoverPresentationStyle;

        public PreferredOrientation PreferredOrientations
        {
            get { return preferredOrientations; }
            set
            {
                if (value != preferredOrientations)
                {
                    preferredOrientations = value;
                    OnPropertyChanged("PreferredOrientations");
                }
            }
        }
        private PreferredOrientation preferredOrientations;

        public ShouldNavigateDelegate ShouldNavigate
        {
            get { return shouldNavigate; }
            set
            {
                if (value != shouldNavigate)
                {
                    shouldNavigate = value;
                    OnPropertyChanged("ShouldNavigate");
                }
            }
        }
        private ShouldNavigateDelegate shouldNavigate;

        public IHistoryStack Stack
        {
            get { return PaneManager.Instance.FirstOrDefault(s => s.Views.Contains(this)); }
        }

        public string StackID
        {
            get { return stackID; }
            set
            {
                if (value != stackID)
                {
                    stackID = value;
                    OnPropertyChanged("StackID");
                }
            }
        }
        private string stackID;

        public string Title
        {
            get { return TitleBlock.Text; }
            set
            {
                value = value ?? string.Empty;
                if (value != TitleBlock.Text)
                {
                    TitleBlock.Text = value;
                    OnPropertyChanged("Title");
                    InvalidateArrange();
                }
            }
        }

        public UI.Color TitleColor
        {
            get { return TitleBlock.Foreground.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? Color.Black : value;
                if (value != TitleColor)
                {
                    TitleBlock.Foreground = backButton.Foreground = value.GetBrush();
                    OnPropertyChanged("TitleColor");
                }
            }
        }

        public new double Height
        {
            get { return ActualHeight; }
        }

        public new double Width
        {
            get { return ActualWidth; }
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

        public System.Windows.Controls.Grid HeaderBar { get; private set; }

        public TextBlock TitleBlock { get; private set; }

        private System.Windows.Controls.Button backButton;

        public BaseView()
        {
            LastChildFill = true;

            TitleBlock = new TextBlock()
            {
                FontSize = 15,
                FontWeight = FontWeights.Regular,
                Foreground = System.Windows.Media.Brushes.Black,
                TextAlignment = System.Windows.TextAlignment.Center,
                TextTrimming = System.Windows.TextTrimming.CharacterEllipsis,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center
            };
            System.Windows.Controls.Grid.SetColumnSpan(TitleBlock, 3);

            backButton = new ButtonBase()
            {
                IsCancel = true,
                Background = new Color().GetBrush(),
                BorderThickness = new Thickness(0),
                Content = "\uE601",
                FontFamily = new System.Windows.Media.FontFamily(new Uri("pack://application:,,,/iFactr.Wpf;component/"), "./Resources/#WPF-Symbol"),
                FontSize = 20,
                Margin = new Thickness(10, 2, 2, 2),
                Padding = new Thickness(6, 0, 6, 0),
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch
            };

            backButton.Click += (o, e) =>
            {
                var stack = Stack;
                if (stack != null)
                {
                    stack.HandleBackLink(backLink, OutputPane);
                }
            };

            backButton.MouseEnter += (o, e) =>
            {
                backButton.BorderThickness = new Thickness(1);
                backButton.Margin = new Thickness(8, 0, 0, 0);
            };

            backButton.MouseLeave += (o, e) =>
            {
                if (!backButton.IsKeyboardFocused)
                {
                    backButton.BorderThickness = new Thickness(0);
                    backButton.Margin = new Thickness(10, 2, 2, 2);
                }
            };

            backButton.GotKeyboardFocus += (o, e) =>
            {
                backButton.BorderThickness = new Thickness(1);
                backButton.Margin = new Thickness(8, 0, 0, 0);
            };

            backButton.LostKeyboardFocus += (o, e) =>
            {
                if (!backButton.IsMouseOver)
                {
                    backButton.BorderThickness = new Thickness(0);
                    backButton.Margin = new Thickness(10, 2, 2, 2);
                }
            };

            Children.Add((HeaderBar = new System.Windows.Controls.Grid()
            {
                MinHeight = 40,
                Children = { backButton, TitleBlock },
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) }
                }
            }));
            SetDock(HeaderBar, Dock.Top);

            var line = new Border()
            {
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Height = 1,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch
            };
            Children.Add(line);
            SetDock(line, Dock.Top);

            Loaded += (o, e) =>
            {
                var stack = Stack;
                if (stack != null)
                {
                    backButton.Visibility = stack.DisplayBackButton(backLink) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                }

                var handler = Activated;
                if (handler != null)
                {
                    handler(Pair ?? this, EventArgs.Empty);
                }
            };

            Unloaded += (o, e) =>
            {
                var handler = Deactivated;
                if (handler != null)
                {
                    handler(Pair ?? this, EventArgs.Empty);
                }
            };

            SizeChanged += (o, e) =>
            {
                if (e.PreviousSize.Width != e.NewSize.Width)
                {
                    OnPropertyChanged("Width");
                }

                if (e.PreviousSize.Height != e.NewSize.Height)
                {
                    OnPropertyChanged("Height");
                }
            };
        }

        public void Render()
        {
            var handler = Rendering;
            if (handler != null)
            {
                handler(Pair ?? this, EventArgs.Empty);
            }

            OnRender();
        }

        public async virtual void SetBackground(string imagePath, ContentStretch stretch)
        {
            var brush = await imagePath.GetImageBrush();
            if (brush == null)
            {
                Background = null;
            }
            else
            {
                brush.Stretch = (System.Windows.Media.Stretch)stretch;
                Background = brush;
            }
        }

        public virtual void SetBackground(UI.Color color)
        {
            Background = color.IsDefaultColor ? null : color.GetBrush();
        }

        public virtual bool Equals(IView other)
        {
            var view = other as iFactr.UI.View;
            if (view != null)
            {
                return view.Equals(this);
            }

            return base.Equals(other);
        }

        protected virtual void OnRender() { }

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size arrangeSize)
        {
            var size = base.ArrangeOverride(arrangeSize);

            var headerWidth = HeaderBar.ColumnDefinitions.Sum(c => c.ActualWidth);

            TitleBlock.Width = double.NaN;
            TitleBlock.Measure(arrangeSize);
            var desiredWidth = TitleBlock.DesiredSize.Width;
            var titleX = headerWidth / 2 - desiredWidth / 2;

            var index = HeaderBar.ColumnDefinitions.IndexOf(c => c.Width.IsStar);
            if (index >= 0)
            {
                var star = HeaderBar.ColumnDefinitions[index];
                var rightWidth = HeaderBar.ColumnDefinitions.Skip(index + 1).Sum(c => c.ActualWidth);
                var titleWidth = Math.Max(0, Math.Min(headerWidth - titleX - rightWidth, desiredWidth));
                while (desiredWidth - titleWidth > 0.1)
                {
                    titleX = headerWidth / 2 - titleWidth / 2;
                    desiredWidth = titleWidth;
                    titleWidth = Math.Max(0, Math.Min(headerWidth - titleX - rightWidth, desiredWidth));
                }

                TitleBlock.Width = titleWidth;
            }

            return size;
        }
    }
}
