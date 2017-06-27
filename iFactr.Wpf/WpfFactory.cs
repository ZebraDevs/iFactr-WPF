using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using iFactr.Core;
using iFactr.Core.Layers;
using iFactr.Core.Native;
using iFactr.Core.Targets;
using iFactr.Core.Targets.Settings;
using MonoCross.Utilities;
using MonoCross.Utilities.ImageComposition;
using iFactr.Integrations;
using iFactr.UI;
using iFactr.UI.Controls;
using iFactr.UI.Instructions;
using MonoCross.Navigation;

namespace iFactr.Wpf
{
    /// <summary>
    /// This class represents the binding factory for the Windows target.
    /// <para></para>
    /// </summary>
    /// <remarks>
    /// <para><img src="WpfFactory.cd"/></para></remarks>
    public class WpfFactory : NativeFactory
    {
        #region Properties, constructors, and singletons
        private bool _windowInited;
        private LoadSpinner loadSpinner;

        public MainPage MainWindow { get; private set; }

        public ITabView TabView { get; private set; }

        public void ShowPopover(object content)
        {
        }

        public void HidePopover()
        {
        }

        public override Instructor Instructor
        {
            get { return instructor ?? (instructor = new WindowsInstructor()); }
            set { instructor = value; }
        }
        private Instructor instructor;

        public override Core.Styles.Style Style
        {
            get
            {
                return style ?? (style = new Core.Styles.Style
                {
                    SubTextColor = new UI.Color(134, 134, 134),
                    SecondarySubTextColor = new UI.Color(134, 134, 134)
                });
            }
            set
            {
                style = value;
            }
        }
        private Core.Styles.Style style;

        public override MonoCross.Utilities.Threading.IThread Thread
        {
            get
            {
                return Device.Thread;
            }
        }

        public bool IsBusy { get; set; }

        internal BitmapImage Chevron;
        public string iItemButtonImagePath
        {
            get
            {
                return Chevron == null ? null : Chevron.UriSource.OriginalString;
            }
            set
            {
                Chevron = LoadBitmap(value);
            }
        }

        static WpfFactory()
        {
            Device.Initialize(new WpfDevice(null));
            Initialize(new WpfFactory());
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                Application.Current.Activated += (o, e) =>
                {
                    Application.Current.MainWindow.InvalidateVisual();
                };

                Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseBack, NavigationBackCommandBinding_Executed));
                bool.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("smallFormFactor"), out Instance._smallFormFactor);
            }
            iApp.VanityImagePath = "iPad-Detail.png";
        }

        /// <summary>
        /// Private default constructor to use with singleton
        /// </summary>
        private WpfFactory()
        {
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.MinWidth = 800;
                Application.Current.MainWindow.MinHeight = 600;

                MainWindow = new MainPage();
                MainWindow.AddHandler(Hyperlink.RequestNavigateEvent, new RoutedEventHandler(OnNavigationRequest));
                WpfDevice.Thread.Dispatcher = MainWindow.Dispatcher;
            }

            DefaultLoadIndicatorDelay = 1000;
        }

        /// <summary>
        /// Initializes the factory singleton.
        /// </summary>
        public static void Initialize()
        {
        }

        /// <summary>
        /// Gets the factory instance.
        /// </summary>
        /// <value>The instance.</value>
        public static new WpfFactory Instance
        {
            get { return (WpfFactory)MXContainer.Instance; }
        }
        #endregion

        protected override void OnSetDefinitions()
        {
            Register<IPlatformDefaults>(typeof(PlatformDefaults));
            Register<IGeoLocation>(typeof(GeoLocation));
            Register<ICompass>(typeof(Compass));
            Register<IAccelerometer>(typeof(Accelerometer));
            Register<IAlert>(typeof(Alert));
            Register<ITimer>(typeof(Timer));
            Register<IView>(typeof(VanityView));
            Register<IBrowserView>(typeof(BrowserView));
            Register<ICanvasView>(typeof(CanvasView));
            Register<IGridView>(typeof(GridView));
            Register<IListView>(typeof(ListView));
            Register<ITabView>(typeof(TabView));
            Register<ITabItem>(typeof(TabItem));
            Register<ISearchBox>(typeof(SearchBox));
            Register<IMenu>(typeof(Menu));
            Register<IMenuButton>(typeof(MenuButton));
            Register<IToolbar>(typeof(Toolbar));
            Register<IToolbarButton>(typeof(ToolbarButton));
            Register<IToolbarSeparator>(typeof(ToolbarSeparator));
            Register<ISectionHeader>(typeof(SectionHeader));
            Register<ISectionFooter>(typeof(SectionFooter));
            Register<IGridCell>(typeof(GridCell));
            Register<IRichContentCell>(typeof(RichContentCell));
            //Register<IGrid>(typeof(Grid));
            Register<IButton>(typeof(Button));
            Register<IDatePicker>(typeof(DatePicker));
            Register<IImage>(typeof(Image));
            Register<ILabel>(typeof(Label));
            Register<IPasswordBox>(typeof(PasswordBox));
            Register<ISelectList>(typeof(SelectList));
            Register<ISlider>(typeof(Slider));
            Register<ISwitch>(typeof(Switch));
            Register<ITextArea>(typeof(TextArea));
            Register<ITextBox>(typeof(TextBox));
            Register<ITimePicker>(typeof(TimePicker));
        }

        protected override void OnOutputView(IMXView view)
        {
            if (!_windowInited)
            {
                InitializeWindow();
            }

            var pairable = view as IPairable;
            if (!(view is UIElement) && (pairable == null || !(pairable.Pair is UIElement)))
            {
                iApp.Log.Debug("Cannot output a view whose native component is not a UIElement.");
                return;
            }

            var tabView = GetNativeObject<UIElement>(view, "view", false) as ITabView;
            if (tabView != null)
            {
                MainWindow.Content = TabView = tabView;
            }
            else
            {
                PaneManager.Instance.DisplayView(view);
            }
        }

        protected override void OnBeginBlockingUserInput()
        {
            if (PopoverPane.PopoverWindow != null)
            {
                PopoverPane.PopoverWindow.IsHitTestVisible = false;
                PopoverPane.PopoverWindow.Focusable = false;
            }
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.IsHitTestVisible = false;
                Application.Current.MainWindow.Focusable = false;
            }
        }

        protected override void OnStopBlockingUserInput()
        {
            if (PopoverPane.PopoverWindow != null)
            {
                PopoverPane.PopoverWindow.IsHitTestVisible = true;
                PopoverPane.PopoverWindow.Focusable = true;
            }
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.IsHitTestVisible = true;
                Application.Current.MainWindow.Focusable = true;
            }
        }

        protected override void OnShowImmediateLoadIndicator()
        {
            Mouse.OverrideCursor = Cursors.Wait;
        }

        protected override void OnShowLoadIndicator(string title)
        {
            if (loadSpinner == null)
            {
                loadSpinner = new LoadSpinner();
            }
            loadSpinner.Show(title);
        }

        protected override void OnHideLoadIndicator()
        {
            if (loadSpinner != null)
            {
                loadSpinner.Hide();
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        protected override object OnGetCustomItem(ICustomItem item, iLayer layer, IListView view, object recycledCell)
        {
            var cell = GetNativeObject<FrameworkElement>(Converter.ConvertToCell(item, layer.LayerStyle, view, null), "item", true);
            if (CustomItemRequested != null)
            {
                return CustomItemRequested(item, layer);
            }
            return cell;
        }

        protected override double GetLineHeight(Font font)
        {
            var family = new FontFamily(font.Name);
            return family.LineSpacing * font.Size * family.Baseline;
        }

        /// <summary>
        /// Outputs a layer to the WPF target.
        /// </summary>
        /// <param name="layer">The layer.</param>
        protected override bool OnOutputLayer(iLayer layer)
        {
            if (!_windowInited)
            {
                InitializeWindow();
            }

            if (layer == null)
                return false;
				
            if (layer is LoginLayer)
            {
                new LoginControl(layer as LoginLayer).Show();
                return true;
            }

            if (layer is Browser)
            {
                var uri = ((Browser)layer).Url;
                MainWindow.IsEnabled = true;
                IsBusy = false;
                const string imageScheme = "image://";
                const string audioPlaybackScheme = "audio://";
                const string videoPlaybackScheme = "video://";
                const string voiceScheme = "voicerecording://";
                const string videoRecordingScheme = "videorecording://";

                if (uri.StartsWith(imageScheme))
                {
                    new ImagePicker(layer.NavContext, HttpUtility.ParseQueryString(uri.Substring(imageScheme.Length)));
                    return true;
                }
                else if (uri.StartsWith(audioPlaybackScheme))
                {
                    //AudioPlaybackExtensions.Launch(url);
                }
                else if (uri.StartsWith(voiceScheme))
                {
                    //AudioRecordingExtensions.Launch(url);
                }
                else if (uri.StartsWith(videoPlaybackScheme))
                {
                    //VideoPlaybackExtensions.Launch(url);
                }
                else if (uri.StartsWith(videoRecordingScheme))
                {
                    //VideoRecordingExtensions.Launch(url);
                }
                else if (uri.StartsWith("tel"))
                {
                    BrowserView.Launch(uri);
                    return true;
                }
            }
            return base.OnOutputLayer(layer);
        }

        public void OnNavigationRequest(object sender, RoutedEventArgs e)
        {
            var source = e.OriginalSource as Hyperlink;
            if (source != null)
            {
                iApp.Navigate(source.NavigateUri.ToString());
            }
        }

        /// <summary>
        /// Allows implementation of ICustomItem in container
        /// </summary>
        public Func<ICustomItem, iLayer, FrameworkElement> CustomItemRequested { get; set; }

        #region Future WpfFactory abstract methods
#if SILVERLIGHT
        public override MobileTarget Target { get { return MobileTarget.Silverlight; } }

        protected void Exit()
        {
            System.Windows.Browser.HtmlPage.Window.Eval("window.close();");
        }

        protected void LaunchBrowser(string location)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(location, UriKind.RelativeOrAbsolute), "_blank");
        }

        protected void InitializeWindow(string title)
        {
            if (Application.Current.IsRunningOutOfBrowser)
            {
                Application.Current.MainWindow.Title = title;
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
            else
            {
                System.Windows.Browser.HtmlPage.Window.Eval(string.Format("document.title=\"{0}\"", title));
            }
            WaitSpinner = new RadBusyIndicator
            {
                BusyContent = GetResourceString("Loading"),
                DisplayAfter = TimeSpan.FromSeconds(1.75),
                IsTabStop = false,
            };
            MainWindow.LayoutRoot.Children.Add(WaitSpinner);
        }

        public void SetDock(UIElement element, System.Windows.Controls.Dock dock)
        {
            switch (dock)
            {
                case System.Windows.Controls.Dock.Bottom:
                    RadDockPanel.SetDock(element, Telerik.Windows.Controls.Dock.Bottom);
                    break;
                case System.Windows.Controls.Dock.Left:
                    RadDockPanel.SetDock(element, Telerik.Windows.Controls.Dock.Left);
                    break;
                case System.Windows.Controls.Dock.Right:
                    RadDockPanel.SetDock(element, Telerik.Windows.Controls.Dock.Right);
                    break;
                case System.Windows.Controls.Dock.Top:
                    RadDockPanel.SetDock(element, Telerik.Windows.Controls.Dock.Top);
                    break;
            }
        }

        public object ParseXaml(string xamlText)
        {
            return XamlReader.Load(xamlText);
        }

        public async static Task<BitmapImage> LoadBitmapAsync(string uri)
        {
            if (uri == null) return null;

            Stream memStream = null;
            BitmapImage bitmap = null;
            if (uri.StartsWith("data:image"))
            {
                string ex;
                memStream = new MemoryStream(ImageUtility.DecodeImageFromDataUri(uri, out ex));
            }
            else if (uri.StartsWith("http") || uri.StartsWith("ftp"))
            {
                memStream = new MemoryStream();
                var httPeter = new HttpClient();
                try
                {
                    var stream = await httPeter.GetStreamAsync(uri);
                    await stream.CopyToAsync(memStream);
                }
                catch (Exception e)
                {
                    iApp.Log.Info(e);
                    memStream = null;
                }
            }
            else
            {
                byte[] bytes = iApp.File.Read(uri, EncryptionMode.NoEncryption);
                if (bytes == null) iApp.Log.Info("Not found: " + uri);
                else memStream = new MemoryStream(bytes);
            }

            if (memStream != null)
            {
                memStream.Position = 0;
                bitmap = new BitmapImage();
                bitmap.SetSource(memStream);
            }

            return bitmap;
        }

        public override string TempPath
        {
            get { return "Temp"; }
        }
#else
        public override MobileTarget Target { get { return MobileTarget.Windows; } }

        private void Exit()
        {
            Application.Current.MainWindow.Close();
        }

        private void InitializeWindow()
        {
            _windowInited = true;
            MainWindow.IsEnabled = true;
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            Application.Current.MainWindow.Title = App.Title;

            //PaneManager.Instance.Clear();

            if (PaneManager.Instance.FromNavContext(UI.Pane.Master, 0) == null)
            {
                var pane = new Pane("0");
                PaneManager.Instance.AddStack(pane, new iApp.AppNavigationContext() { ActiveTab = 0 });
                MainWindow.LayoutRoot.Children.Add(pane);
            }

            PaneManager.Instance.AddStack(new PopoverPane(), new iApp.AppNavigationContext() { ActivePane = UI.Pane.Popover });

            if (TheApp.FormFactor == FormFactor.SplitView)
            {
                MainWindow.LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star), MinWidth = 200 });
                MainWindow.LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                MainWindow.LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2.5, GridUnitType.Star), MinWidth = 200 });

                var splitter = new GridSplitter()
                {
                    IsEnabled = true,
                    IsTabStop = false,
                    Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0)),
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                    ResizeDirection = GridResizeDirection.Columns,
                    ResizeBehavior = GridResizeBehavior.PreviousAndNext,
                    ShowsPreview = true,
                    Width = 3
                };
                splitter.MouseEnter += (o, e) => { Mouse.OverrideCursor = Cursors.SizeWE; };
                splitter.MouseLeave += (o, e) => { Mouse.OverrideCursor = null; };
                System.Windows.Controls.Grid.SetColumn(splitter, 1);
                MainWindow.LayoutRoot.Children.Add(splitter);

                var detail = new Pane("Detail");
                System.Windows.Controls.Grid.SetColumn(detail, 2);
                PaneManager.Instance.AddStack(detail, new iApp.AppNavigationContext() { ActivePane = UI.Pane.Detail });
                MainWindow.LayoutRoot.Children.Add(detail);
            }
        }

        public object ParseXaml(string xamlText)
        {
            try
            {
                return XamlReader.Parse(xamlText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static BitmapImage LoadBitmap(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) return null;
            var task = LoadBitmapAsync(uri);
            task.Wait();
            return task.Result;
        }

        public async static Task<BitmapImage> LoadBitmapAsync(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) return null;

            Stream memStream = null;
            if (uri.StartsWith("data:image"))
            {
                string ex;
                var data = ImageUtility.DecodeImageFromDataUri(uri, out ex);
                if (data != null)
                {
                    memStream = new MemoryStream(data);
                }
            }
            else if (uri.StartsWith("http") || uri.StartsWith("ftp"))
            {
                memStream = new MemoryStream();
                var httPeter = new HttpClient();
                try
                {
                    var stream = await httPeter.GetStreamAsync(uri);
                    await stream.CopyToAsync(memStream);
                }
                catch (Exception e)
                {
                    iApp.Log.Error("Image load failed: " + uri, e);
                    memStream = null;
                }
            }
            else
            {
                byte[] bytes = iApp.File.Read(uri, EncryptionMode.NoEncryption);
                if (bytes == null) iApp.Log.Info("Image load failed: " + uri);
                else memStream = new MemoryStream(bytes);
            }

            if (memStream == null) return null;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.None;
            memStream.Position = 0;
            bitmap.StreamSource = memStream;

            try
            {
                bitmap.EndInit();
                //bitmap.Freeze();
            }
            catch (Exception e)
            {
                iApp.Log.Error(e);
                bitmap = null;
            }

            return bitmap;
        }
#endif
        #endregion
        #region Navigation helpers
        private static void NavigationBackCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PaneManager.Instance.FromNavContext(PaneManager.Instance.TopmostPane).HandleBackLink(null, PaneManager.Instance.TopmostPane.OutputOnPane);
        }
        #endregion
        #region Image helpers
        /// <summary>
        /// Stores an image to filesystem, overwriting if a file already exists.
        /// </summary>
        /// <returns>
        /// The image ID.
        /// </returns>
        /// <param name='imageData'>
        /// The image data.
        /// </param>
        /// <param name="extension">
        /// The image file's extension.
        /// </param>
        /// <param name='imageId'>
        /// An optional image identifier. If null, a GUID will be assigned as the image ID.
        /// </param>
        internal string StoreImage(Stream imageData, string extension, string imageId = null)
        {
            if (null == imageId)
            {
                imageId = Guid.NewGuid().ToString();
            }

            iApp.File.Save(Path.Combine(TempImagePath, imageId + "." + extension), imageData, EncryptionMode.NoEncryption);
            return imageId;
        }


        /// <summary>
        /// Retrieves a full file path for an image ID.
        /// </summary>
        /// <returns>
        /// The full path to the image file for the specified ID.
        /// </returns>
        /// <param name='imageId'>
        /// An image identifier. Returns null if no image is found for the given ID.
        /// </param>
        public override string RetrieveImage(string imageId)
        {
            var info = iApp.File.GetFileNames(TempImagePath);
            return (from file in info where file.Contains(imageId) select file).FirstOrDefault();
        }
        #endregion
        #region ITargetFactory Members


#if SILVERLIGHT
        public override ICompositor Compositor
        {
            get { return _compositor ?? (_compositor = new NullCompositor()); }
        }

        public override bool LargeFormFactor
        {
            get { return true; }
        }
#else
        public override ICompositor Compositor
        {
            get { return _compositor ?? (_compositor = new GdiPlusCompositor()); }
        }

        public override ISettings Settings
        {
            get { return _settings ?? (_settings = new AppConfigSettings()); }
        }

        private bool _smallFormFactor;
        public override bool LargeFormFactor
        {
            get { return !_smallFormFactor; }
        }

        public override string DeviceId
        {
            get
            {
                try
                {
                    var scope = new System.Management.ManagementScope("\\\\" + Environment.MachineName + "\\root\\cimv2");
                    scope.Connect();
                    var wmiClass = new System.Management.ManagementObject(scope, new System.Management.ManagementPath("Win32_BaseBoard.Tag=\"Base Board\""), new System.Management.ObjectGetOptions());

                    var propData = wmiClass.Properties.Cast<System.Management.PropertyData>().FirstOrDefault(prop => prop.Name == "SerialNumber");
                    if (propData != null)
                    {
                        return Convert.ToString(propData.Value);
                    }
                }
                catch (Exception ex)
                {
                    iApp.Log.Warn("Failed to get unique device id", ex);
                }
                return null;
            }
        }
#endif

        internal string TempImagePath { get { return Path.Combine(TempPath, "Images"); } }
        #endregion

        internal static T GetNativeObject<T>(object obj, string objName, bool allowNull)
            where T : class
        {
            if (allowNull && obj == null)
                return null;

            obj = TargetFactory.GetNativeObject(obj, objName, typeof(T), typeof(ISealedElement));
            var seal = obj as ISealedElement;
            if (seal != null)
            {
                return seal.Element as T;
            }
            return obj as T;
        }
    }
}