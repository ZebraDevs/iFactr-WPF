using System.Windows;
using System.Windows.Controls;
using iFactr.Core;
using iFactr.UI;

namespace iFactr.Wpf
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
            var button = MessageBoxButton.OK;
            if (Buttons == AlertButtons.OKCancel)
            {
                button = MessageBoxButton.OKCancel;
            }
            else if (Buttons == AlertButtons.YesNo)
            {
                button = MessageBoxButton.YesNo;
            }

            var result = MessageBox.Show(Message ?? string.Empty, Title ?? string.Empty, button);

            var handler = Dismissed;
            if (handler != null)
            {
                var ar = AlertResult.Cancel;
                if (result == MessageBoxResult.OK)
                {
                    ar = AlertResult.OK;
                }
                else if (result == MessageBoxResult.Yes)
                {
                    ar = AlertResult.Yes;
                }
                else if (result == MessageBoxResult.No)
                {
                    ar = AlertResult.No;
                }

                handler(this, new AlertResultEventArgs(ar));
            }
            else if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
            {
                iApp.Navigate(OKLink);
            }
            else
            {
                iApp.Navigate(CancelLink);
            }
        }

        public Alert(string message, string title, AlertButtons buttons)
        {
            Message = message;
            Title = title;
            Buttons = buttons;
        }
    }
}