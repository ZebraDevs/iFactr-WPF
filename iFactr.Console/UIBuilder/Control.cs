using iFactr.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.UI;

namespace iFactr.Console
{
    abstract class Control : IControl
    {

        public int ColumnIndex { get; set; }

        public int ColumnSpan { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; }

        public string ID { get; set; }

        public bool IsEnabled { get; set; }

        public Thickness Margin { get; set; }

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

        public object Parent { get; }
        public int RowIndex { get; set; }

        public int RowSpan { get; set; }

        public abstract string StringValue { get; }

        public string SubmitKey { get; set; }

        public VerticalAlignment VerticalAlignment { get; set; }

        public Visibility Visibility { get; set; }

        public event ValidationEventHandler Validating;

        public bool Equals(IElement other)
        {
            var control = other as Element;
            if (control != null)
            {
                return control.Equals(this);
            }

            return base.Equals(other);
        }

        public Size Measure(Size constraints)
        {
            return Size.Empty;
        }

        public virtual void NullifyEvents()
        {
            Validating = null;
        }

        public void SetLocation(Point location, Size size)
        {
        }

        public abstract bool Validate(out string[] errors);

        protected bool Validate(object value, out string[] errors)
        {
            var handler = Validating;
            if (handler != null)
            {
                var args = new ValidationEventArgs(SubmitKey, value, StringValue);
                handler(Pair ?? this, args);

                if (args.Errors.Count > 0)
                {
                    errors = new string[args.Errors.Count];
                    args.Errors.CopyTo(errors, 0);
                    return false;
                }
            }

            errors = null;
            return true;
        }

    }

}
