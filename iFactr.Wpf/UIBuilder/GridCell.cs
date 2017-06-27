using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using iFactr.Core;
using iFactr.UI;
using iFactr.UI.Controls;
using Thickness = System.Windows.Thickness;

namespace iFactr.Wpf
{
    public class GridCell : System.Windows.Controls.Grid, IGridCell, INotifyPropertyChanged
    {
        private const byte HoverAlpha = 38;
        private const byte HoverBorderAlpha = 200;
        private const byte SelectAlpha = 90;
        private const byte SelectBorderAlpha = 255;

        private static readonly UI.Color DefaultHighlightColor = new UI.Color(145, 200, 255);

        public event PropertyChangedEventHandler PropertyChanged;

        [EventDelegate("selected")]
        public event EventHandler Selected
        {
            add
            {
                selected += value;
                SetHighlighterEnabled();
            }
            remove
            {
                selected -= value;
                SetHighlighterEnabled();
            }
        }
        private event EventHandler selected;

        [EventDelegate("accessorySelected")]
        public event EventHandler AccessorySelected
        {
            add
            {
                accessorySelected += value;
                SetAccessoryButton();
            }
            remove
            {
                accessorySelected -= value;
                SetAccessoryButton();
            }
        }
        private event EventHandler accessorySelected;

        public Link AccessoryLink
        {
            get { return accessoryLink; }
            set
            {
                if (value != accessoryLink)
                {
                    accessoryLink = value;
                    SetAccessoryButton();

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("AccessoryLink"));
                    }
                }
            }
        }
        private Link accessoryLink;

        public UI.Color BackgroundColor
        {
            get { return Background.GetColor(); }
            set
            {
                if (value != BackgroundColor)
                {
                    Background = value.IsDefaultColor ? null : value.GetBrush();

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("BackgroundColor"));
                    }
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

        public MetadataCollection Metadata
        {
            get { return metadata ?? (metadata = new MetadataCollection()); }
        }
        private MetadataCollection metadata;

        public Link NavigationLink
        {
            get { return navigationLink; }
            set
            {
                if (value != navigationLink)
                {
                    navigationLink = value;
                    SetHighlighterEnabled();

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("NavigationLink"));
                    }
                }
            }
        }
        private Link navigationLink;

        public UI.Thickness Padding
        {
            get { return padding; }
            set
            {
                if (value != padding)
                {
                    padding = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Padding"));
                    }
                }
            }
        }
        private UI.Thickness padding;

        public RowCollection Rows { get; private set; }

        public UI.Color SelectionColor
        {
            get { return highlighter.BorderBrush.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? DefaultHighlightColor : value;
                value.A = isSelected ? SelectBorderAlpha : HoverBorderAlpha;
                if (value != SelectionColor)
                {
                    highlighter.BorderBrush = value.GetBrush();
                    highlighter.Background = new UI.Color(isSelected ? SelectAlpha : HoverAlpha, value.R, value.G, value.B).GetBrush();

                    if (accessoryButton != null)
                    {
                        accessoryButton.BorderBrush = highlighter.BorderBrush;
                    }

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("SelectionColor"));
                    }
                }
            }
        }

        public SelectionStyle SelectionStyle
        {
            get { return selectionStyle; }
            set
            {
                if (value != selectionStyle)
                {
                    selectionStyle = value;

                    bool shouldHighlight = (value & UI.SelectionStyle.HighlightOnly) != 0;
                    SetHighlighterOpacity(shouldHighlight && highlighter.IsEnabled && IsMouseOver ? 1 : 0);

                    if (accessoryButton != null)
                    {
                        accessoryButton.BorderThickness = new Thickness(shouldHighlight && IsMouseOver ? 1 : 0);
                        accessoryButton.Background = shouldHighlight && !highlighter.IsEnabled && IsMouseOver ? highlighter.Background : Brushes.Transparent;
                    }

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("SelectionStyle"));
                    }
                }
            }
        }
        private SelectionStyle selectionStyle;

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

        private System.Windows.Controls.Button accessoryButton;
        private Border highlighter;
        private Canvas canvas;
        private bool isSelected, isDown;

        public GridCell()
        {
            Columns = new ColumnCollection();
            Rows = new RowCollection();
            
            base.Children.Add((highlighter = new Border()
            {
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(1),
                Opacity = 0,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                IsEnabled = false,
            }));
            
            base.Children.Add((canvas = new Canvas() { Margin = new Thickness(1, 0, 2, 0) }));

            Loaded += (o, e) => AttachToItem(this.GetParent<ListBoxItem>());
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

                if (highlighter.Opacity > 0)
                {
                    var label = element as ILabel;
                    if (label != null)
                    {
                        var textBlock = element as TextBlock;
                        if (textBlock != null)
                        {
                            textBlock.Foreground = (label.HighlightColor.IsDefaultColor ? label.ForegroundColor : label.HighlightColor).GetBrush();
                        }
                    }
                }

                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs("Children"));
                }
            }
        }

        public void NullifyEvents()
        {
            selected = null;
            accessorySelected = null;

            SetHighlighterEnabled();
            SetAccessoryButton();
        }

        public void RemoveChild(IElement control)
        {
            var element = WpfFactory.GetNativeObject<FrameworkElement>(control, "element", true);
            if (element != null)
            {
                canvas.Children.Remove(element);

                var label = element as ILabel;
                if (label != null)
                {
                    var textBlock = element as TextBlock;
                    if (textBlock != null)
                    {
                        textBlock.Foreground = label.ForegroundColor.GetBrush();
                    }
                }

                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs("Children"));
                }
            }
        }

        public void Select()
        {
            isSelected = true;

            var selectionColor = SelectionColor;
            selectionColor.A = SelectAlpha;
            highlighter.Background = selectionColor.GetBrush();

            selectionColor.A = SelectBorderAlpha;
            highlighter.BorderBrush = selectionColor.GetBrush();

            if (highlighter.IsEnabled && (selectionStyle & UI.SelectionStyle.HighlightOnly) != 0)
            {
                SetHighlighterOpacity(1);
                if (accessoryButton != null)
                {
                    accessoryButton.BorderThickness = new Thickness(1);
                    if (!highlighter.IsEnabled)
                    {
                        accessoryButton.Background = highlighter.Background;
                    }
                }
            }

            var parent = this.GetParent<ListBoxItem>();
            if (parent != null)
            {
                parent.IsSelected = true;
            }

            var handler = selected;
            if (handler != null)
            {
                handler(Pair ?? this, EventArgs.Empty);
            }
            else
            {
                iApp.Navigate(navigationLink, this.GetParent<MonoCross.Navigation.IMXView>());
            }
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

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if ((selectionStyle & UI.SelectionStyle.IndicatorOnly) != 0)
            {
                if (highlighter.IsEnabled)
                {
                    SetHighlighterOpacity(1);
                }

                if (accessoryButton != null)
                {
                    accessoryButton.BorderThickness = new Thickness(1);
                    if (!highlighter.IsEnabled)
                    {
                        accessoryButton.Background = highlighter.Background;
                    }
                }
            }
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            isDown = false;

            if (!(isSelected || IsFocused) || (selectionStyle & UI.SelectionStyle.HighlightOnly) == 0 || !highlighter.IsEnabled)
            {
                SetHighlighterOpacity(0);
                if (accessoryButton != null)
                {
                    accessoryButton.BorderThickness = new Thickness(0);
                    accessoryButton.Background = Brushes.Transparent;
                }
            }
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            isDown = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (isDown)
            {
                Select();
                isDown = false;
            }
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            AccessoryButton button = null;
            foreach (var child in base.Children.OfType<AccessoryButton>())
            {
                button = child;
                break;
            }

            var width = constraint.Width - (canvas.Margin.Left + canvas.Margin.Right) - (button == null ? 0 : button.Width);
            var size = this.PerformLayout(new UI.Size(width, MinHeight), new UI.Size(width, MaxHeight));

            return new System.Windows.Size(constraint.Width, size.Height);
        }

        protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if ((e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Space) && IsFocused)
            {
                Select();
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (IsFocused && (selectionStyle & UI.SelectionStyle.HighlightOnly) != 0)
            {
                SetHighlighterOpacity(1);
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (!isSelected && !IsMouseOver)
            {
                SetHighlighterOpacity(0);
            }
        }

        private void AttachToItem(ListBoxItem item)
        {
            if (item == null)
            {
                return;
            }

            item.Unselected += (o, e) =>
            {
                isSelected = false;
                var selectionColor = SelectionColor;
                selectionColor.A = HoverAlpha;
                highlighter.Background = selectionColor.GetBrush();

                selectionColor.A = HoverBorderAlpha;
                highlighter.BorderBrush = selectionColor.GetBrush();

                if (!(IsMouseOver || IsKeyboardFocused) || (selectionStyle & UI.SelectionStyle.HighlightOnly) == 0)
                {
                    SetHighlighterOpacity(0);
                    if (accessoryButton != null)
                    {
                        accessoryButton.BorderThickness = new Thickness(0);
                        accessoryButton.Background = Brushes.Transparent;
                    }
                }
            };

            var control = Parent as ContentControl;
            if (control != null)
            {
                control.IsTabStop = false;
                KeyboardNavigation.SetTabNavigation(control, KeyboardNavigationMode.Continue);
            }
        }

        private void SetHighlighterEnabled()
        {
            highlighter.IsEnabled = selected != null || (navigationLink != null && navigationLink.Address != null);
            Focusable = navigationLink != null && navigationLink.Address != null;
        }

        private void SetHighlighterOpacity(double opacity)
        {
            highlighter.Opacity = opacity;
            if (opacity == 0)
            {
                foreach (var label in Children.OfType<ILabel>())
                {
                    var textBlock = WpfFactory.GetNativeObject<TextBlock>(label, "child", true);
                    if (textBlock != null)
                    {
                        textBlock.Foreground = label.ForegroundColor.GetBrush();
                    }
                }
            }
            else
            {
                foreach (var label in Children.OfType<ILabel>())
                {
                    var textBlock = WpfFactory.GetNativeObject<TextBlock>(label, "child", true);
                    if (textBlock != null)
                    {
                        textBlock.Foreground = (label.HighlightColor.IsDefaultColor ? label.ForegroundColor : label.HighlightColor).GetBrush();
                    }
                }
            }
        }

        private void SetAccessoryButton()
        {
            if (accessorySelected != null || (accessoryLink != null && accessoryLink.Address != null))
            {
                if (accessoryButton == null)
                {
                    accessoryButton = new AccessoryButton()
                    {
                        BorderBrush = highlighter.BorderBrush,
                        BorderThickness = new Thickness(0)
                    };
                    System.Windows.Controls.Grid.SetColumn(accessoryButton, 1);

                    accessoryButton.MouseEnter += (o, e) =>
                    {
                        accessoryButton.BorderThickness = new Thickness(1);
                        accessoryButton.Background = Brushes.Transparent;
                    };

                    accessoryButton.MouseLeave += (o, e) =>
                    {
                        if (!((isSelected && (selectionStyle & UI.SelectionStyle.HighlightOnly) != 0 && highlighter.IsEnabled) ||
                            (IsMouseOver && (selectionStyle & UI.SelectionStyle.IndicatorOnly) != 0)))
                        {
                            accessoryButton.BorderThickness = new Thickness(0);
                        }
                        else if (!highlighter.IsEnabled)
                        {
                            accessoryButton.Background = highlighter.Background;
                        }
                    };

                    accessoryButton.Click += (o, e) =>
                    {
                        var handler = accessorySelected;
                        if (handler != null)
                        {
                            handler(Pair ?? this, EventArgs.Empty);
                        }
                        else
                        {
                            iApp.Navigate(accessoryLink, this.GetParent<MonoCross.Navigation.IMXView>());
                        }
                    };
                }

                if (!base.Children.Contains(accessoryButton))
                {
                    base.Children.Add(accessoryButton);
                }
            }
            else if (accessorySelected == null && (accessoryLink == null || accessoryLink.Address == null))
            {
                base.Children.Remove(accessoryButton);
            }
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
