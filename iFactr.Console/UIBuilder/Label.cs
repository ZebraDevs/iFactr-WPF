using iFactr.UI;
using iFactr.UI.Controls;

namespace iFactr.Console
{
    internal class Label : Control, ILabel
    {
      
        public Font Font { get; set; }

        public Color ForegroundColor { get; set; }

        public Color HighlightColor { get; set; }

        public int Lines { get; set; }

        public override string StringValue { get { return Text;  } }

        public string Text { get; set; }


        public TextAlignment TextAlignment { get; set; }


        public override bool Validate(out string[] errors)
        {
            return Validate(Text, out errors);
        }
    }
}
