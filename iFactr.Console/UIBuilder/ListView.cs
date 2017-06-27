using iFactr.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.Core;
using iFactr.UI.Controls;

namespace iFactr.Console
{
    class ListView : BaseView, IListView
    {
        private List<ICell> Cells = new List<ICell>();
        public Link BackLink { get; set; }

        public CellDelegate CellRequested { get; set; }

        public ColumnMode ColumnMode { get; set; }


        public ItemIdDelegate ItemIdRequested { get; set; }

        public IMenu Menu { get; set; }


        public Pane OutputPane { get; set; }


        public PopoverPresentationStyle PopoverPresentationStyle { get; set; }

        public ISearchBox SearchBox { get; set; }

        public SectionCollection Sections { get; } = new SectionCollection();

        public Color SeparatorColor { get; set; }

        public ShouldNavigateDelegate ShouldNavigate { get; set; }

        public IHistoryStack Stack { get; }

        public string StackID { get; set; }

        public ListViewStyle Style { get; }

        public ValidationErrorCollection ValidationErrors { get; } = new ValidationErrorCollection();

        public event EventHandler Activated;
        public event EventHandler Deactivated;
        public event SubmissionEventHandler Submitting;

        public ListView()
        {
            Rendering += (o, e) => { ReloadSections(); };
        }

        public IDictionary<string, string> GetSubmissionValues()
        {

            var dict = new Dictionary<string, string>();

            foreach (var control in Cells.OfType<IGridCell>().SelectMany(c => c.Children.OfType<IControl>().Where(c2 => c2.ShouldSubmit())))
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

                dict[control.SubmitKey] = control.StringValue;
            }
            return dict;
        }



        public IEnumerable<ICell> GetVisibleCells()
        {
            return Cells;
        }

        public void ReloadSections()
        {
            Cells.Clear();
            for (int section = 0; section < Sections.Count; section++)
            {
                for (int index = 0; index < Sections[section].ItemCount; index++)
                {
                    Cells.Add(CellRequested(section, index, null));
                }
            }
        }


        public void ScrollToCell(int section, int index, bool animated)
        { }

        public void ScrollToEnd(bool animated)
        { }

        public void ScrollToHome(bool animated)
        { }

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
