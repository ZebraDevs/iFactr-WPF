using iFactr.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.Core;
using iFactr.UI.Controls;
using System.Collections;

namespace iFactr.Console
{
    internal class GridView : BaseView, IGridView
    {
        public Link BackLink { get; set; }

        public IEnumerable<IElement> Children { get; } = new List<IElement>();

        public ColumnCollection Columns { get; } = new ColumnCollection();

    
        public bool HorizontalScrollingEnabled { get; set; }

        public IMenu Menu { get; set; }


        public Pane OutputPane { get; set; }

        public Thickness Padding { get; set; }

        public PopoverPresentationStyle PopoverPresentationStyle { get; set; }


        public RowCollection Rows { get; } = new RowCollection();

        public ShouldNavigateDelegate ShouldNavigate { get; set; }

        public IHistoryStack Stack { get; }

        public string StackID { get; set; }

        public ValidationErrorCollection ValidationErrors { get; } = new ValidationErrorCollection();

        public bool VerticalScrollingEnabled { get; set; }

        public event EventHandler Activated;
        public event EventHandler Deactivated;
        public event SubmissionEventHandler Submitting;

        public void AddChild(IElement element)
        {
            ((IList)Children).Add(element);
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

        public void RemoveChild(IElement element)
        {
            ((IList)Children).Remove(element);
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

        public void Submit(string url)
        {
            Submit(new Link(url));
        }
    }
}
