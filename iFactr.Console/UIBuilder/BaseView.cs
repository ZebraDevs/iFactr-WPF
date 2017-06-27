using iFactr.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactr.Console
{
    internal class BaseView : IView
    {
        public Color HeaderColor { get; set; }

        public double Height { get; }

        public MetadataCollection Metadata { get; } = new MetadataCollection();

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

        public PreferredOrientation PreferredOrientations { get; set; }

        public string Title { get; set; }

        public Color TitleColor { get; set; }

        public double Width { get; }

        public event EventHandler Rendering;

        public bool Equals(IView other)
        {
            var view = other as iFactr.UI.View;
            if (view != null)
            {
                return view.Equals(this);
            }

            return base.Equals(other);
        }


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


        public void Render()
        {
            Rendering?.Invoke(Pair ?? this, EventArgs.Empty);
        }

        public void SetBackground(Color color)
        {
        }

        public void SetBackground(string imagePath, ContentStretch stretch)
        {
        }

    }
}
