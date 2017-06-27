using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using iFactr.UI;
using Thickness = System.Windows.Thickness;

namespace iFactr.Wpf
{
    [StackBehavior(StackBehaviorOptions.ForceRoot | StackBehaviorOptions.HistoryShy)]
    public class VanityView : DockPanel, IView, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler Rendering;

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

        public UI.Color HeaderColor
        {
            get { return headerBar.Background.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? new UI.Color(248, 248, 248) : value;
                if (value != HeaderColor)
                {
                    headerBar.Background = value.GetBrush();

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("HeaderColor"));
                    }
                }
            }
        }

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
            get { return titleBlock.Text; }
            set
            {
                value = value ?? string.Empty;
                if (value != titleBlock.Text)
                {
                    titleBlock.Text = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Title"));
                    }
                }
            }
        }

        public UI.Color TitleColor
        {
            get { return titleBlock.Foreground.GetColor(); }
            set
            {
                value = value.IsDefaultColor ? Color.Black : value;
                if (value != TitleColor)
                {
                    titleBlock.Foreground = value.GetBrush();

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("TitleColor"));
                    }
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

        private System.Windows.Controls.Grid headerBar;
        private TextBlock titleBlock;

        public VanityView()
        {
            LastChildFill = false;

            titleBlock = new TextBlock()
            {
                FontSize = 15,
                FontWeight = FontWeights.Regular,
                TextAlignment = System.Windows.TextAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center
            };

            Children.Add((headerBar = new System.Windows.Controls.Grid()
            {
                MinHeight = 40,
                Children = { titleBlock }
            }));
            SetDock(headerBar, Dock.Top);

            var line = new Border()
            {
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Height = 1,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch
            };
            Children.Add(line);
            SetDock(line, Dock.Top);

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

        public void SetBackground(UI.Color color)
        {
            Background = color.IsDefaultColor ? null : color.GetBrush();
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
