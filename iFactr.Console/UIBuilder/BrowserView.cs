using iFactr.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.Core;

namespace iFactr.Console
{
    internal class BrowserView : BaseView, IBrowserView
    {
        public Link BackLink { get; set; }

        public bool CanGoBack { get; }

        public bool CanGoForward { get; }

        public bool EnableDefaultControls { get; set; }

        public IMenu Menu { get; set; }

        public Pane OutputPane { get; set; }

        public PopoverPresentationStyle PopoverPresentationStyle { get; set; }

        public ShouldNavigateDelegate ShouldNavigate { get; set; }

        public IHistoryStack Stack { get; }

        public string StackID { get; set; }

        public event EventHandler Activated;
        public event EventHandler Deactivated;
        public event EventHandler<LoadFinishedEventArgs> LoadFinished;

        public void GoBack()
        { }

        public void GoForward()
        {
        }

        public void LaunchExternal(string url)
        {
        }

        public void Load(string url)
        {
        }

        public void LoadFromString(string html)
        {
        }

        public void Refresh()
        {
        }
    }
}
