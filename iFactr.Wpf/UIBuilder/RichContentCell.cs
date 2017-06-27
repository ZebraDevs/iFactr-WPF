using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using iFactr.Core;
using iFactr.UI;

namespace iFactr.Wpf
{
    public class RichContentCell : System.Windows.Controls.Border, IRichContentCell, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Color ForegroundColor
        {
            get { return foregroundColor; }
            set
            {
                value = value.IsDefaultColor ? Color.Black : value;
                if (value != foregroundColor)
                {
                    foregroundColor = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("ForegroundColor"));
                    }
                }
            }
        }
        private Color foregroundColor;

        public Color BackgroundColor
        {
            get { return Background.GetColor(); }
            set
            {
                if (value != BackgroundColor)
                {
                    Background = value.IsDefaultColor ? null : value.GetBrush();

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("BackgroundColor"));
                    }
                }
            }
        }

        public MetadataCollection Metadata
        {
            get { return metadata ?? (metadata = new MetadataCollection()); }
        }
        private MetadataCollection metadata;

        public string Text
        {
            get { return text; }
            set
            {
                if (value != text)
                {
                    text = value;

                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Text"));
                    }
                }
            }
        }
        private string text;

        List<Core.Controls.PanelItem> Core.Layers.IHtmlText.Items { get; set; }

        public IPairable Pair
        {
            get { return pair; }
            set
            {
                if (pair == null && value != null)
                {
                    pair = value;
                    pair.Pair = this;
                }
            }
        }
        private IPairable pair;

        public RichContentCell()
        {
            base.Loaded += (o, e) =>
            {
                var control = Parent as ContentControl;
                if (control != null)
                {
                    control.IsTabStop = false;
                }

                Load();
            };
        }

        public void Load()
        {
            Child = ConvertHtml(Text);
        }

        public void Measure()
        {
            var element = this.GetParent<IListView>() as FrameworkElement;

            base.Height = double.NaN;
            base.Measure(new System.Windows.Size(element == null ? 0 : element.ActualWidth, double.PositiveInfinity));
            base.Height = DesiredSize.Height;
        }

        public bool Equals(ICell other)
        {
            var cell = other as iFactr.UI.Cell;
            if (cell != null)
            {
                return cell.Equals(this);
            }

            return base.Equals(other);
        }

        private UIElement ConvertHtml(string html)
        {
            var xamlBuilder = new StringBuilder(html);
            foreach (var pair in HtmlFlowDocumentMapper)
            {
                xamlBuilder.Replace(pair.Key, pair.Value);
            }

            xamlBuilder.Insert(0, string.Format("<Paragraph FontFamily=\"Segoe UI\" FontSize=\"12\" TextAlignment=\"Left\" Foreground=\"{0}\">", foregroundColor.HexCode));
            xamlBuilder.Append("</Paragraph>");

            string viewer;
            string args = string.Empty;

            xamlBuilder.Insert(0, "<FlowDocument>");
            xamlBuilder.Append("</FlowDocument>");
            viewer = "FlowDocumentScrollViewer";
            args = @"Margin=""0""";

            var xaml = string.Format(ViewFormat, viewer, xamlBuilder.ToString(), args);
            const string imageLink = "</InlineImage></ConfirmLink>";
            while (xaml.Contains(imageLink))
            {
                var linkIndex = xaml.IndexOf(imageLink, System.StringComparison.Ordinal);
                var part1 = xaml.Remove(linkIndex);
                var part2 = xaml.Substring(linkIndex);

                var tagName = "ConfirmLink";
                linkIndex = part1.LastIndexOf(tagName, System.StringComparison.Ordinal);
                part1 = part1.Remove(linkIndex, tagName.Length).Insert(linkIndex, "InlineImage");

                tagName = "><InlineImage";
                linkIndex = part1.LastIndexOf(tagName, System.StringComparison.Ordinal);
                part1 = part1.Remove(linkIndex, tagName.Length);

                tagName = "</ConfirmLink>";
                linkIndex = part2.IndexOf(tagName, System.StringComparison.Ordinal);
                part2 = part2.Remove(linkIndex, tagName.Length);

                xaml = part1 + part2;
            }

            return (UIElement)(WpfFactory.Instance.ParseXaml(xaml) ?? ConvertHtml("Invalid markup detected."));
        }

        private const string ViewFormat = @"<{0} xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" Background=""#00000000"" FontSize=""12"" HorizontalScrollBarVisibility=""Hidden"" VerticalScrollBarVisibility=""Hidden"" IsTabStop=""False"" {2}>{1}</{0}>";

        private static readonly Dictionary<string, string> HtmlFlowDocumentMapper = new Dictionary<string, string>
        {
            { "&", "&amp;" },
            { "&amp;amp;", "&amp;" },
            { "<p>", "<Paragraph TextAlignment=\"Left\">" },
            { "</p>", "</Paragraph>" },
            { "<a href", "<ConfirmLink NavigateUri" },
            { "</a>", "</ConfirmLink>" },
            { "<b>", "<Bold>" },
            { "</b>", "</Bold>" },
            { "<u>", "<Underline>" },
            { "</u>", "</Underline>" },
            { "<h3>", "<Bold>" },
            { "</h3>", "</Bold><LineBreak/>" },
            { "<h1>", "<Run FontWeight=\"Bold\" FontSize=\"15\">" },    
            { "</h1>", "</Run><LineBreak/>" },
            { "<h4>", "<Run FontWeight=\"Bold\" FontSize=\"11\">" },
            { "</h4>", "</Run><LineBreak/>" },
            { "<i>", "<Italic>" },
            { "</i>", "</Italic>" },
            { "<br/>", "<LineBreak/>" },
            { "<hr/>", "<Line Stretch=\"Fill\" Stroke=\"DarkGray\" X2=\"1\"/>" },
            { "<img ", "<InlineImage Margin=\"5,0\" Padding=\"0\" " },
            { "title=\"", "ToolTip=\"" },
            { " src=\"", "><![CDATA[" },
            { " target=\"_blank\"", string.Empty },
            { "\"   />", "]]></InlineImage>" },
            { "onclick=\"if(!confirm('", "Confirmation=\"" },
            { "'))return false;)\"", "\"" },
            { "align=\"left\" style=\"padding:5px", "WrapDirection=\"Both\" HorizontalAnchor=\"ContentLeft" },
            { "align=\"right\" style=\"padding:5px", "WrapDirection=\"Both\" HorizontalAnchor=\"ContentRight" },
            { ";max-height:none", "\" Height=\"Auto" },
            { ";max-width:100%", "\" Width=\"Auto" },
            { ";max-height:", "\" Height=\"" },
            { ";max-width:", "\" Width=\"" },
            { "style=\"padding:5px", "WrapDirection=\"None\" HorizontalAnchor=\"ContentLeft" },
        };
    }
}
