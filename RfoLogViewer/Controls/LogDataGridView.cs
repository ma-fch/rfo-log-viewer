using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RfoLogViewer.Controls
{
    /// <summary>
    /// Read-only grid with Ctrl+C copy support for cell, row, or multi-row selection.
    /// </summary>
    public partial class LogDataGridView : DataGridView
    {
        public LogDataGridView()
        {
            this.InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C))
            {
                this.CopySelectionToClipboard();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void CopySelectionToClipboard()
        {
            if (this.SelectedCells.Count == 0)
            {
                return;
            }

            if (this.SelectedCells.Count == 1)
            {
                Clipboard.SetText(GetCellText(this.SelectedCells[0]));
                return;
            }

            var selectedRows = this.SelectedCells
                .Cast<DataGridViewCell>()
                .Select(c => c.RowIndex)
                .Distinct()
                .OrderBy(i => i)
                .ToList();

            if (selectedRows.Count == 1 && this.SelectedCells.Count == this.Columns.Count)
            {
                Clipboard.SetText(this.BuildRowText(selectedRows[0]));
                return;
            }

            var builder = new StringBuilder();
            foreach (var rowIndex in selectedRows)
            {
                if (builder.Length > 0)
                {
                    builder.AppendLine();
                }
                builder.Append(this.BuildRowText(rowIndex));
            }
            Clipboard.SetText(builder.ToString());
        }

        private string BuildRowText(int rowIndex)
        {
            var values = new string[this.Columns.Count];
            for (var col = 0; col < this.Columns.Count; col++)
            {
                values[col] = GetCellText(this.Rows[rowIndex].Cells[col]);
            }
            return string.Join("\t", values);
        }

        private static string GetCellText(DataGridViewCell cell)
        {
            return Convert.ToString(cell.FormattedValue) ?? string.Empty;
        }
    }
}
