using System;
using iFactr.Core;
using iFactr.UI;
using MonoCross.Utilities;

namespace iFactr.Console
{
    public class Alert : IAlert
    {
        public event AlertResultEventHandler Dismissed;

        public Link CancelLink { get; set; }

        public Link OKLink { get; set; }

        public string Message { get; private set; }

        public string Title { get; private set; }

        public AlertButtons Buttons { get; private set; }

        public void Show()
        {
            if (!string.IsNullOrWhiteSpace(Title))
            {
                System.Console.WriteLine(Title);
            }
            if (Buttons == AlertButtons.OK)
            {
                if (!string.IsNullOrWhiteSpace(Message))
                {
                    System.Console.WriteLine(Message);
                }
            }
            else
            {
                System.Console.Write(Message + " (y/N) ");
                string confirm = (System.Console.ReadLine() ?? "n").ToLower().Trim();
                bool isConfirmed = ExpandInput(confirm).TryParseBoolean();
                var handler = Dismissed;
                if (handler != null)
                {
                    handler(this, new AlertResultEventArgs(isConfirmed ? AlertResult.OK : AlertResult.Cancel));
                }
                else if (isConfirmed)
                {
                    iApp.Navigate(OKLink);
                }
                else
                {
                    iApp.Navigate(CancelLink);
                }
            }
        }

        private string ExpandInput(string confirm)
        {
            switch (confirm)
            {
                case "y":
                    return "yes";
                case "n":
                    return "no";
                case "t":
                    return "true";
                case "f":
                    return "false";
            }
            return confirm;
        }

        public Alert(string message, string title, AlertButtons buttons)
        {
            Message = message;
            Title = title;
            Buttons = buttons;
        }
    }
}
