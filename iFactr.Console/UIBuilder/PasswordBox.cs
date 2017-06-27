using iFactr.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFactr.UI;

namespace iFactr.Console
{
    internal class PasswordBox : Control, IPasswordBox
    {
        public Color BackgroundColor { get; set; }

        public string Expression { get; set; }
 
        public Font Font { get; set; }

        public Color ForegroundColor { get; set; }

        public bool IsFocused { get; set; }

        public KeyboardReturnType KeyboardReturnType { get; set; }

        public KeyboardType KeyboardType { get; set; }

        public string Password { get; set; }

        public string Placeholder { get; set; }

        public Color PlaceholderColor { get; set; }
        public override string StringValue { get { return Password; } }


        public event EventHandler GotFocus;
        public event EventHandler LostFocus;
        public event ValueChangedEventHandler<string> PasswordChanged;
        public event EventHandler<EventHandledEventArgs> ReturnKeyPressed;


        public void Focus()
        {
        }

        public override void NullifyEvents()
        {
            base.NullifyEvents();
            GotFocus = null;
            LostFocus = null;
            ReturnKeyPressed = null;
            PasswordChanged = null;

        }


        public override bool Validate(out string[] errors)
        {
            return Validate(Password, out errors);
        }
    }
}
