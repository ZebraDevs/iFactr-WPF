using System.Windows;
using System.Windows.Documents;
using iFactr.Core;
using iFactr.Core.Controls;
using MonoCross.Navigation;

namespace iFactr.Wpf
{
    public class ConfirmLink : Hyperlink
    {
        #region DependencyProperty 'Confirmation'

        /// <summary>
        /// Gets or sets the confirmation text.
        /// </summary>
        public string Confirmation
        {
            get { return (string)GetValue(ConfirmationProperty); }
            set { SetValue(ConfirmationProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set the confirmation text.
        /// </summary>
        public static readonly DependencyProperty ConfirmationProperty = DependencyProperty.Register("Confirmation", typeof(string), typeof(ConfirmLink));
        #endregion

        protected override void OnClick()
        {
            iApp.Navigate(new Link(NavigateUri.OriginalString) { ConfirmationText = Confirmation }, this.GetParent<IMXView>());
        }
    }
}
