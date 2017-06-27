using iFactr.Core;
using iFactr.Core.Layers;
using System.Windows;
using System.Windows.Input;
using MonoCross.Utilities;
using MonoCross.Navigation;

namespace iFactr.Wpf
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : IMXView<LoginLayer>
    {
        private static Window loginWindow;
        private LoginLayer _layer;

        public LoginControl(LoginLayer layer)
        {
            InitializeComponent();
            Layer = layer;
            this.SkinLayer(Layer.LayerStyle);

            TxtUser.GotFocus += (o, e) => TxtUser.SelectAll();

            if (layer.UsernameLabel != null)
                LblUser.Text = layer.UsernameLabel;
            if (layer.PasswordLabel != null)
                LblPassword.Text = layer.PasswordLabel;
            if (layer.DefaultUsername != null)
                TxtUser.Text = layer.DefaultUsername;

            if (!Layer.LayerStyle.ErrorTextColor.IsDefaultColor)
            {
                LblError.Foreground = Layer.LayerStyle.ErrorTextColor.GetBrush();
            }

            if (Layer.BrandImage != null)
            {
                var brandImage = WpfFactory.LoadBitmap(Layer.BrandImage.Location);
                if (brandImage != null)
                {
                    BrandImage.Source = brandImage;
                }
            }

            KeyUp += (o, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    btnLogin_Click(o, e);
                }
            };
        }

        public void Show()
        {
            WpfFactory.Instance.StopBlockingUserInput();

            if (loginWindow == null)
            {
                loginWindow = new Window()
                {
                    ResizeMode = ResizeMode.NoResize,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    WindowState = WindowState.Maximized
                };

                loginWindow.Closing += (o, e) =>
                {
                    e.Cancel = true;
                    btnCancel_Click(null, null);
                };

                loginWindow.IsVisibleChanged += (o, e) =>
                {
                    if (!loginWindow.IsVisible)
                    {
                        (PopoverPane.PopoverWindow.Visibility == Visibility.Visible ? PopoverPane.PopoverWindow : Application.Current.MainWindow).Focus();
                    }
                };
            }

            if (loginWindow.Visibility != Visibility.Visible)
            {
                loginWindow.Owner = PopoverPane.PopoverWindow.Visibility == Visibility.Visible ? PopoverPane.PopoverWindow : Application.Current.MainWindow;
                loginWindow.Content = this;
                loginWindow.MinWidth = Application.Current.MainWindow.Width;
                loginWindow.MinHeight = Application.Current.MainWindow.Height;
                loginWindow.ShowDialog();
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
#if !SILVERLIGHT
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Last));
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
#endif
            var loginLayer = Layer;
            var user = TxtUser.Text;
            var pass = TxtPassword.Password;
            iApp.Factory.ActivateLoadTimer(loginLayer.LoginLink.LoadIndicatorTitle ?? iApp.Factory.GetResourceString("AuthLabel"), 0);
            iApp.Thread.Start(ob =>
            {
                if (loginLayer.LogIn(user, pass))
                {
                    iApp.Navigate(loginLayer.LoginLink, loginLayer.View);
                    iApp.Thread.ExecuteOnMainThread(() => loginWindow.Hide());
                }
                else
                {
                    iApp.Factory.StopBlockingUserInput();
                    iApp.Thread.ExecuteOnMainThread(() => LblError.Text = loginLayer.ErrorText);
                }
            }, this);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(iApp.Factory.GetResourceString("LoginCancel") ?? "Canceling your login will close this application, and any unsaved work will be lost. Do you want to quit?", string.Empty, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Application.Current.MainWindow.Close();
            }
        }

        public Core.Layers.LoginLayer Layer
        {
            get { return _layer; }
            private set
            {
                _layer = value;
                var prop = Device.Reflector.GetProperty(_layer.GetType(), "View");
                prop.SetValue(_layer, this);
                var field = Device.Reflector.GetField(typeof(PaneManager), "NavigatedURIs");
                var uris = (WeakKeyDictionary<IMXView, string>)field.GetValue(PaneManager.Instance);
                uris[this] = _layer.NavContext.NavigatedUrl;
            }
        }

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            //((PasswordBox)sender).SelectAll();
        }

        System.Type IMXView.ModelType
        {
            get
            {
                return Layer == null ? typeof(LoginLayer) : Layer.GetType();
            }
        }

        object IMXView.GetModel()
        {
            return Layer;
        }

        void IMXView.Render()
        {
            Show();
        }

        void IMXView.SetModel(object model)
        {
            Layer = model as LoginLayer;
        }

        LoginLayer IMXView<LoginLayer>.Model
        {
            get { return Layer; }
            set { Layer = value; }
        }
    }
}