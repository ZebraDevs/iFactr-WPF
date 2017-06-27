using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFactr.UI;
using iFactr.UI.Controls;
using iFactr.UI.Instructions;

namespace iFactr.Wpf
{
    public class WindowsInstructor : UniversalInstructor
    {
        protected override void OnLayout(ILayoutInstruction element)
        {
            var headeredCell = element as HeaderedControlCell;
            if (headeredCell != null)
            {
                OnLayoutHeaderedCell(headeredCell);
                return;
            }

            base.OnLayout(element);
        }

        private void OnLayoutHeaderedCell(HeaderedControlCell cell)
        {
            var grid = ((IPairable)cell).Pair as IGridCell;
            if (grid == null)
                return;

            grid.Columns.Clear();
            grid.Rows.Clear();

            grid.Columns.Add(Column.OneStar);
            grid.Rows.Add(Row.AutoSized);

            cell.Header.Font = Font.PreferredHeaderFont;
            cell.Header.Lines = 1;
            cell.Header.VerticalAlignment = VerticalAlignment.Center;
            cell.Header.HorizontalAlignment = HorizontalAlignment.Stretch;
            cell.Header.RowIndex = 0;

            foreach (var boolSwitch in grid.Children.OfType<ISwitch>())
            {
                grid.Columns.Insert(0, Column.AutoSized);

                boolSwitch.VerticalAlignment = VerticalAlignment.Center;
                boolSwitch.Margin = new Thickness(0, 0, Thickness.LargeHorizontalSpacing, 0);
                boolSwitch.RowIndex = 0;
                boolSwitch.ColumnIndex = grid.Columns.Count - 2;
            }

            cell.Header.ColumnIndex = grid.Columns.Count - 1;

            foreach (var control in grid.Children.Where(c => c != cell.Header && !(c is ISwitch)))
            {
                grid.Rows.Add(Row.AutoSized);

                control.RowIndex = grid.Rows.Count - 1;
                control.ColumnIndex = 0;
                control.ColumnSpan = grid.Columns.Count;
                control.Margin = new Thickness(0, Thickness.SmallVerticalSpacing, 0, 0);

                if (control is IButton || control is IDatePicker || control is ITimePicker)
                {
                    control.HorizontalAlignment = HorizontalAlignment.Stretch;
                }

                if (control is ITextArea)
                {
                    control.VerticalAlignment = VerticalAlignment.Stretch;
                    grid.Rows[control.RowIndex] = Row.OneStar;
                }
                else
                {
                    control.VerticalAlignment = VerticalAlignment.Top;
                }
            }
        }
    }
}
