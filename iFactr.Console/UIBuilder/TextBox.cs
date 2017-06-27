using iFactr.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.UI;

namespace iFactr.Console
{
    internal class TextBox : Control, ITextBox
    {
        public Color BackgroundColor { get; set; }

        public string Expression { get; set; }

        public Font Font { get; set; }

        public Color ForegroundColor { get; set; }

        public bool IsFocused { get; }

        public KeyboardReturnType KeyboardReturnType { get; set; }

        public KeyboardType KeyboardType { get; set; }
        
        public string Placeholder { get; set; }

        public Color PlaceholderColor { get; set; }

        public override string StringValue { get { return Text; } }


        public string Text { get; set; }

        public TextAlignment TextAlignment { get; set; }

        public TextCompletion TextCompletion { get; set; }

        public event EventHandler GotFocus;
        public event EventHandler LostFocus;
        public event EventHandler<EventHandledEventArgs> ReturnKeyPressed;
        public event ValueChangedEventHandler<string> TextChanged;

        public void Focus()
        {
        }

        public override void NullifyEvents()
        {
            base.NullifyEvents();

            GotFocus = null;
            LostFocus = null;
            ReturnKeyPressed = null;
            TextChanged = null;
        }

        public override bool Validate(out string[] errors)
        {
            return Validate(Text, out errors);
        }
    }
}
