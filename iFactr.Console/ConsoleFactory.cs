using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using iFactr.Core;
using iFactr.Core.Controls;
using iFactr.Core.Forms;
using iFactr.Core.Layers;
using iFactr.Core.Native;
using iFactr.Core.Targets;
using iFactr.Core.Targets.Settings;
using MonoCross.Utilities.Encryption;
using MonoCross.Utilities.Storage;
using MonoCross.Utilities.Logging;
using MonoCross.Utilities.ImageComposition;
using MonoCross.Utilities;
using iFactr.UI;
using MonoCross.Navigation;

using Button = iFactr.Core.Controls.Button;
using Link = iFactr.UI.Link;
using iFactr.UI.Controls;

/// <summary>
/// This namespace contains the ConsoleFactory and all of the Console bindings for
/// Windows console-based target.
/// <para></para>
/// <para><img src="ConsoleFactory.cd"/></para>
/// </summary>
namespace iFactr.Console
{
    /// <summary>
    /// This class implements the abstract TargetFactory for Windows console-based
    /// targets.
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// <para><img src="ConsoleFactory.cd"/></para></remarks>
    public class ConsoleFactory : NativeFactory, ITargetFactory
    {
        private Dictionary<int, Link> navigationKeys = new Dictionary<int, Link>();
        private Stack<iLayer> navHistory = new Stack<iLayer>();
        private string LastAddress { get; set; }
        private int page;
        private string searchTerm;

        static ConsoleFactory()
        {
            Device.Initialize(ConsoleDevice.Instance);
        }

        private ConsoleFactory() { }

        /// <summary>
        /// Initializes the factory singleton.
        /// </summary>
        public static void Initialize()
        {
            if (!IsInitialized)
            {
                Initialize(new ConsoleFactory());
            }
        }
        /// <summary>
        /// Gets the factory instance.
        /// </summary>
        /// <value>The instance.</value>
        public static new ConsoleFactory Instance
        {
            get
            {
                Initialize();
                return (ConsoleFactory)MXContainer.Instance;
            }
        }

        /// <summary>
        /// Outputs a layer to the console platform.
        /// </summary>
        /// <param name="layer">The layer.</param>
        protected override bool OnOutputLayer(iLayer layer)
        {
            layer.NavContext.NavigatedUrl = LastAddress ?? string.Empty;
            page = 0;
            searchTerm = string.Empty;

            if (navHistory.Contains(layer))
            {
                navHistory.Pop().Unload();
                while (navHistory.Count > 1 && navHistory.Peek() != layer)
                {
                    navHistory.Pop();
                }
            }
            else
            {
                navHistory.Push(layer);
            }

            DisplayLayer(layer);

            return true;
        }

        private void DisplayLayer(iLayer layer)
        {
            LastAddress = layer.NavContext.NavigatedUrl;
            ResetNavigationKeys();

            System.Console.Title = TheApp.Title;
            System.Console.Clear();

            if (!string.IsNullOrEmpty(layer.Title))
                System.Console.WriteLine(layer.Title);

            if (layer is NavigationTabs)
            {
                OutputTabs((NavigationTabs)layer);
            }
            else
            {
                foreach (var item in layer.Items)
                {
                    if (item is iBlock)
                        OutputBlock((iBlock)item);
                    else if (item is iPanel)
                        OutputPanel((iPanel)item);
                    else if (item is iList)
                        OutputList((iList)item);
                    else if (item is iMenu)
                        OutputMenu((iMenu)item);
                    else if (item is Fieldset)
                        OutputForm(layer, (Fieldset)item);
                }
                OutputActions(layer.ActionButtons);
            }

            if (layer.BackButton != null && layer.BackButton.Action != iFactr.Core.Controls.Button.ActionType.None)
            {
                layer.BackButton.Address = TheApp.NavigateOnLoad;
            }

            System.Console.WriteLine();
            System.Console.WriteLine("0. {0} ...", GetResourceString("Back") ?? "Back");
            System.Console.WriteLine();

            string input = System.Console.ReadLine();
            Link action = new Link(TheApp.NavigateOnLoad);

            try
            {
                int key = Convert.ToInt16(input);

                if (key == 0 && navHistory.Count > 1)
                {
                    navHistory.Pop().Unload();
                    var nextLayer = navHistory.Peek();
                    DisplayLayer(nextLayer);
                }
                action = navigationKeys[key];
            }
            catch
            {
                action = new Link(input == string.Empty ? TheApp.NavigateOnLoad : input);
            }

            if (action.Address != null && action.Address.StartsWith("##"))
            {
                if (action.Address == "##PREV##")
                {
                    page--;
                }
                else if (action.Address == "##NEXT##")
                {
                    page++;
                }
                else if (action.Address == "##SEARCH##")
                {
                    System.Console.Write("{0}: ", GetResourceString("SearchHint") ?? "Search");
                    input = System.Console.ReadLine();
                    searchTerm = input;
                    page = 0;
                }
                else if (action.Address == "##CLEAR##")
                {
                    searchTerm = string.Empty;
                    page = 0;
                }
                DisplayLayer(layer);
                return;
            }

            if (!string.IsNullOrEmpty(action.ConfirmationText))
            {
                System.Console.Write(action.ConfirmationText + " (y/N) ");
                string confirm = System.Console.ReadLine();
                if (confirm.ToLower().Trim()[0] != 'y')
                {
                    DisplayLayer(layer);
                    return;
                }
            }

            System.Console.WriteLine();
            LastAddress = action.Address;

            if (action.RequestType == RequestType.ClearPaneHistory)
                ClearHistory();

            iApp.Navigate(action);
        }
        protected override void OnSetDefinitions()
        {
            Register<IPlatformDefaults>(typeof(PlatformDefaults));
            Register<IAlert>(typeof(Alert));
            Register<ITimer>(typeof(Timer));
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


        protected void OutputPanel(iPanel panel)
        {
            System.Console.WriteLine(panel.Header);
            System.Console.WriteLine(StripHtml(panel.Text));
        }

        protected void OutputBlock(iBlock block)
        {
            System.Console.WriteLine(block.Header);
            System.Console.WriteLine(StripHtml(block.Text));
        }

        protected void OutputTabs(NavigationTabs tabs)
        {
            System.Console.WriteLine();
            foreach (Tab tab in tabs.TabItems)
            {
                string output = tab.Text;
                if (tab.Link.Address != null)
                    output = navigationKeys.Count + ".  " + output;
                System.Console.WriteLine(output);
                navigationKeys.Add(navigationKeys.Count, tab.Link);
            }
        }

        protected void OutputList(iList list)
        {
            System.Console.WriteLine();
            if (!string.IsNullOrEmpty(list.Header))
                System.Console.WriteLine(list.Header);

            bool nav = false;

            if (list is SearchList)
            {
                System.Console.WriteLine(navigationKeys.Count + ".  {0}", GetResourceString("EnterTerm") ?? "Enter search term");
                navigationKeys.Add(navigationKeys.Count, new Link("##SEARCH##"));
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    System.Console.WriteLine(navigationKeys.Count + ".  {0}", GetResourceString("ClearTerm") ?? "Clear search");
                    navigationKeys.Add(navigationKeys.Count, new Link("##CLEAR##"));
                }
                nav = true;
                (list as SearchList).PerformSearch(LastAddress, searchTerm);
            }

            int pageLength = list.Items.Count;
            string pager = string.Empty;

            #region iList paging
            if (list is iPagedList)
            {
                var pagedList = list as iPagedList;
                pageLength = pagedList.PageSize;
            }

            if (pageLength < list.Items.Count)
            {
                nav = true;
                if (page > 0)
                {
                    System.Console.WriteLine(navigationKeys.Count + ".  {0}", GetResourceString("Previous") ?? "Previous page");
                    navigationKeys.Add(navigationKeys.Count, new Link("##PREV##"));
                }
                if (pageLength * (page + 1) < list.Items.Count)
                {
                    System.Console.WriteLine(navigationKeys.Count + ".  {0}", GetResourceString("Next") ?? "Next page");
                    navigationKeys.Add(navigationKeys.Count, new Link("##NEXT##"));
                }

                pager = string.Format(Environment.NewLine + (GetResourceString("Pager") ?? "Page {0} of {1}"), page + 1, Math.Ceiling((decimal)list.Items.Count / pageLength));
            }
            #endregion

            if (nav)
                System.Console.WriteLine();

            for (int i = pageLength * page; i < pageLength * (page + 1) && i < list.Items.Count; i++)
            {
                var listItem = list.Items[i];
                string output = listItem.Text;
                if (listItem.Subtext != null)
                    output = output + " - " + listItem.Subtext;
                if (listItem.Link != null)
                {
                    output = navigationKeys.Count + ".  " + output;
                    navigationKeys.Add(navigationKeys.Count, listItem.Link);
                }
                System.Console.WriteLine(output);
            }
            if (!string.IsNullOrEmpty(pager))
            {
                System.Console.WriteLine(pager);
            }
        }

        protected void OutputMenu(iMenu menu)
        {
            System.Console.WriteLine();
            if (!string.IsNullOrEmpty(menu.Header))
                System.Console.WriteLine(menu.Header);

            foreach (iItem menuItem in menu.Items)
            {
                string output = menuItem.Text;
                if (menuItem.Subtext != null)
                    output = output + " - " + menuItem.Subtext;
                if (menuItem.Link != null)
                    output = navigationKeys.Count + ".  " + output;
                System.Console.WriteLine(output);
                navigationKeys.Add(navigationKeys.Count, menuItem.Link);
            }
        }

        protected void OutputActions(List<iFactr.Core.Controls.Button> buttons)
        {
            if (buttons == null)
                return;
            if (buttons.Count > 0)
                System.Console.WriteLine();

            foreach (var action in buttons)
            {
                string output = action.Text;
                if (action.Address != null)
                    output = navigationKeys.Count + ".  " + output;
                System.Console.WriteLine(output);
                navigationKeys.Add(navigationKeys.Count, action);
            }
        }

        protected void OutputForm(iLayer layer, Fieldset fieldset)
        {
            System.Console.WriteLine();

            Link link = layer.ActionButtons.FirstOrDefault(b => b.Action == iFactr.Core.Controls.Button.ActionType.Submit);
            if (link == null) return;
            link = (Link)link.Clone();

            layer.FieldValuesRequested -= GetFieldValues;
            layer.FieldValuesRequested += GetFieldValues;

            var parameters = layer.GetFieldValues();
            if (link.Parameters == null)
                link.Parameters = parameters;
            else
            {
                foreach (var key in parameters.Keys)
                    link.Parameters[key] = parameters[key];
            }

            if (link.RequestType == RequestType.ClearPaneHistory)
                ClearHistory();

            iApp.Navigate(link);
        }

        private Dictionary<string, string> GetFieldValues(object sender)
        {
            iLayer layer = (iLayer)sender;
            var parameters = new Dictionary<string, string>(layer.ActionParameters);

            foreach (Fieldset fieldset in layer.Items.OfType<Fieldset>())
            {
                System.Console.WriteLine(fieldset.Header);

                foreach (Field field in fieldset.Fields)
                {
                    System.Console.WriteLine(field.Label);

                    if (field is SelectListField)
                    {
                        SelectListField listField = (SelectListField)field;
                        for (int i = 0; i < listField.Items.Count; i++)
                        {
                            System.Console.WriteLine((i + 1) + ".  " + listField.Items[i].Value);
                        }
                        var item = listField.Items[Convert.ToInt32(System.Console.ReadLine()) - 1];
                        parameters[listField.ID + ".Key"] = item.Key;
                        parameters[listField.ID] = item.Value;
                    }
                    else
                    {
                        parameters[field.ID] = System.Console.ReadLine();
                    }
                }
            }

            return parameters;
        }

        private void ClearHistory()
        {
            var tabs = navHistory.FirstOrDefault() as NavigationTabs;
            if (navHistory.Count > 0)
                navHistory.Pop().Unload();
            navHistory.Clear();

            if (tabs != null)
                navHistory.Push(tabs);
        }

        protected void ResetNavigationKeys()
        {
            navigationKeys.Clear();
            navigationKeys.Add(0, new Link(navHistory.Count > 1 ? navHistory.ElementAt(1).NavContext.NavigatedUrl : TheApp.NavigateOnLoad));
        }

        private string StripHtml(string text)
        {
            text = text.Replace("<br/>", "\n");
            text = text.Replace("</h3>", "\n");
            return Regex.Replace(text, @"<(.|\n)*?>", string.Empty);
        }

        #region ITargetFactory Members

        /// <summary>
        /// Gets the application path.
        /// </summary>
        /// <value>The application path.</value>
        public override string ApplicationPath { get { return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); } }

        /// <summary>
        /// Gets the data path.
        /// </summary>
        /// <value>The data path.</value>
        public override string DataPath { get { return Path.Combine(ApplicationPath, "Data"); } }

        public override ILog Logger
        {
            get { return Device.Log; }
        }

        public override ICompositor Compositor
        {
            get { return _compositor ?? (_compositor = new NullCompositor()); }
        }

        public override IEncryption Encryption
        {
            get { return Device.Encryption; }
        }

        public override IFile File
        {
            get { return Device.File; }
        }

        public override MobileTarget Target
        {
            get
            {
                return MobileTarget.Console;
            }
        }

        protected override double GetLineHeight(Font font)
        {
            return 1;
        }

        public override ISettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new BasicSettingsDictionary();
                }

                return _settings;
            }
        }

        #endregion


        protected override void OnShowLoadIndicator(string title) { }

        protected override void OnHideLoadIndicator() { }
    }
}