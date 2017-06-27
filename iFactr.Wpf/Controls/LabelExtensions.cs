using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using iFactr.Core.Controls;

namespace iFactr.Wpf
{
    /// <summary>
    /// This class extends the <see cref="Label"/> class to the WPF target.
    /// </summary>
    public static class LabelExtensions
    {
        /// <summary>
        /// Gets a label as a WPF control.
        /// </summary>
        /// <param name="label">A <see cref="iFactr.Core.Controls.Label"/> representing the label value.</param>
        /// <returns>A concrete <see cref="TextBlock"/> representing the abstract label.</returns>
        public static TextBlock GetControl(this iFactr.Core.Controls.Label label)
        {
            var labelControl = new TextBlock();

            if (label.Style != null)
            {
                if (label.Style.WordWrap)
                    labelControl.TextWrapping = TextWrapping.Wrap;
                if (label.Style.HeaderLevel > 0 || (label.Style.TextFormat & Core.Styles.LabelStyle.Format.Bold) == Core.Styles.LabelStyle.Format.Bold)
                    labelControl.FontWeight = FontWeights.Bold;
                if ((label.Style.TextFormat & Core.Styles.LabelStyle.Format.Italic) == Core.Styles.LabelStyle.Format.Italic)
                    labelControl.FontStyle = FontStyles.Italic;
                labelControl.FontSize = label.Style.FontSize;
                labelControl.FontFamily = new FontFamily(label.Style.FontFamily);

                switch (label.Style.HeaderLevel)
                {
                    case 6:
                    case 5:
                    case 4:
                        labelControl.FontSize = 11;
                        break;
                    case 2:
                        labelControl.FontSize = 15.25;
                        break;
                    case 1:
                        labelControl.FontSize = 18;
                        break;
                }
            }

            if (!string.IsNullOrEmpty(label.Name)) labelControl.Name = label.Name;
            if (!string.IsNullOrEmpty(label.Text)) labelControl.Text = label.Text;
            return labelControl;
        }
    }
}