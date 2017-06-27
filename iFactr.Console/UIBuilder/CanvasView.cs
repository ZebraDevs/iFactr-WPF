using iFactr.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.Core;

namespace iFactr.Console
{
    internal class CanvasView : BaseView, ICanvasView
    {
        public Link BackLink { get; set; }

        public Pane OutputPane { get; set; }

   
        public PopoverPresentationStyle PopoverPresentationStyle { get; set; }

        public ShouldNavigateDelegate ShouldNavigate { get; set; }

        public IHistoryStack Stack { get; }

        public string StackID { get; set; }

        public Color StrokeColor { get; set; }

        public double StrokeThickness { get; set; }

        public IToolbar Toolbar { get; set; }

      
        public event EventHandler Activated;
        public event EventHandler Deactivated;
        public event SaveEventHandler DrawingSaved;

        public void Clear()
        { }

       

        public void Load(string fileName)
        { }

        public void Save(string fileName)
        { }

        public void Save(bool compositeBackground)
        { }

        public void Save(string fileName, bool compositeBackground)
        { }

 
    }
}
