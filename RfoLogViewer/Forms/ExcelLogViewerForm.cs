using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using RfoLogViewer.Controls;
using RfoLogViewer.Data;
using RfoLogViewer.Models;
using RfoLogViewer.Properties;

namespace RfoLogViewer.Forms
{
    public sealed class ExcelLogViewerForm : Form
    {
        private static readonly IReadOnlyDictionary<string, string> LogTableColumnHeaders =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(LogEntry.LogId)] = "Log Id",
                [nameof(LogEntry.DateTime)] = "Timestamp",
                [nameof(LogEntry.Message)] = "Message",
                [nameof(LogEntry.Function)] = "Function",
                [nameof(LogEntry.Step)] = "Step",
                [nameof(LogEntry.Parameters)] = "Parameters",
                [nameof(LogEntry.LogType)] = "Type",
                [nameof(LogEntry.TaskId)] = "Task Id",
                [nameof(LogEntry.RootTaskId)] = "Root Task Id",
                [nameof(LogEntry.Machine)] = "Machine",
                [nameof(LogEntry.ProcessDesc)] = "Process",
                [nameof(LogEntry.LogStructId)] = "Log Struct Id",
                [nameof(LogEntry.RootLogKey)] = "Root Log Key",
                [nameof(LogEntry.PartitionKey)] = "Context"
            };

        private const string TimestampFormat = "yyyy-MM-dd'T'HH:mm:ss.fff";

        private readonly IList<LogEntry> _entries;
        private readonly TreeView _tree;
        private readonly LogDataGridView _grid;
        private readonly ToolStripLabel _lblStatus;
        private readonly SplitContainer _split;
        private readonly Dictionary<string, bool> _logTableColumnVisibility;
        private string _findText;
        private int _findLastRow = -1;
        private int _findLastColumn = -1;
        private bool _findHadMatch;

        public ExcelLogViewerForm(string filePath, IList<LogEntry> entries, TreeNode rootNode)
        {
            this._entries = entries ?? throw new ArgumentNullException(nameof(entries));

            this.Text = $"RFo Log Viewer - {Path.GetFileName(filePath)}";
            this.Icon = AppIcon.Get();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F);
            this.Width = 1400;
            this.Height = 900;

            this._logTableColumnVisibility = ColumnVisibilityStore.Load(
                Settings.Default.LogTableColumnVisibility,
                LogTableColumnHeaders.Keys);

            var findMenu = new ToolStripDropDownButton("Find");
            var findItem = new ToolStripMenuItem("Find...")
            {
                ShortcutKeys = Keys.Control | Keys.F,
                ShowShortcutKeys = true
            };
            findItem.Click += (_, __) => this.ShowFindDialog();
            var findNextItem = new ToolStripMenuItem("Find Next")
            {
                ShortcutKeys = Keys.F3,
                ShowShortcutKeys = true
            };
            findNextItem.Click += (_, __) => this.FindNext();
            var findPreviousItem = new ToolStripMenuItem("Find Previous")
            {
                ShortcutKeys = Keys.Shift | Keys.F3,
                ShowShortcutKeys = true
            };
            findPreviousItem.Click += (_, __) => this.FindPrevious();
            findMenu.DropDownItems.Add(findItem);
            findMenu.DropDownItems.Add(findNextItem);
            findMenu.DropDownItems.Add(findPreviousItem);

            var columnsMenu = this.CreateColumnVisibilityMenu();

            this._lblStatus = new ToolStripLabel { TextAlign = ContentAlignment.MiddleLeft };

            var toolStrip = new ToolStrip();
            toolStrip.Items.Add(findMenu);
            toolStrip.Items.Add(columnsMenu);
            toolStrip.Items.Add(this._lblStatus);

            this.KeyPreview = true;

            this._tree = new TreeView
            {
                Dock = DockStyle.Fill,
                HideSelection = false
            };
            this._tree.AfterSelect += this.Tree_AfterSelect;

            this._grid = new LogDataGridView { Dock = DockStyle.Fill };
            this._grid.AllowUserToResizeRows = false;

            this._split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 380
            };
            this._split.Panel1.Controls.Add(this._tree);
            this._split.Panel2.Controls.Add(this._grid);

            this.Controls.Add(this._split);
            this.Controls.Add(toolStrip);

            this._tree.Nodes.Add(rootNode);
            rootNode.Expand();
            this._lblStatus.Text = $"{entries.Count} log row(s) loaded. Select a tree node to display logs.";
        }

        private ToolStripDropDownButton CreateColumnVisibilityMenu()
        {
            var menu = new ToolStripDropDownButton("Log Columns");
            foreach (var column in LogTableColumnHeaders)
            {
                var item = new ToolStripMenuItem(column.Value)
                {
                    Tag = column.Key,
                    CheckOnClick = true,
                    Checked = this._logTableColumnVisibility[column.Key]
                };
                item.Click += (_, __) =>
                {
                    var key = (string)item.Tag;
                    this._logTableColumnVisibility[key] = item.Checked;
                    ColumnVisibilityStore.ApplyToGrid(this._grid, this._logTableColumnVisibility);
                };
                menu.DropDownItems.Add(item);
            }

            return menu;
        }

        private void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
            {
                return;
            }

            this.ClearFindState();
            var tag = e.Node.Tag as ExcelTreeNodeTag;
            if (tag == null)
            {
                this._grid.DataSource = null;
                return;
            }

            IList<LogEntry> entries;
            switch (tag.ItemType)
            {
                case ExcelTreeItemType.LogSession:
                    entries = this._entries
                        .Where(entry => entry.LogStructId.HasValue && tag.FilterLogStructIds.Contains(entry.LogStructId.Value))
                        .Where(entry => !string.Equals(entry.Function, "LOG_BEGIN", StringComparison.OrdinalIgnoreCase))
                        .Where(entry => !string.Equals(entry.Function, "LOG_END", StringComparison.OrdinalIgnoreCase))
                        .OrderBy(entry => entry.DateTime ?? DateTime.MinValue)
                        .ThenBy(entry => entry.LogId)
                        .ToList();
                    break;
                default:
                    this._grid.DataSource = null;
                    this._lblStatus.Text = e.Node.FullPath.Replace('\\', '/');
                    return;
            }

            this.BindLogTable(entries);
            this._lblStatus.Text = $"{e.Node.FullPath.Replace('\\', '/')} ({entries.Count} row(s))";
        }

        private void BindLogTable(IList<LogEntry> entries)
        {
            this._grid.DataSource = null;
            this._grid.Columns.Clear();
            this._grid.AutoGenerateColumns = false;

            this.AddColumn("Log Id", nameof(LogEntry.LogId), 80);
            this.AddColumn("Timestamp", nameof(LogEntry.DateTime), 190, TimestampFormat);
            this.AddColumn("Message", nameof(LogEntry.Message), 350);
            this.AddColumn("Function", nameof(LogEntry.Function), 160);
            this.AddColumn("Step", nameof(LogEntry.Step), 120);
            this.AddColumn("Parameters", nameof(LogEntry.Parameters), 250);
            this.AddColumn("Type", nameof(LogEntry.LogType), 50);
            this.AddColumn("Task Id", nameof(LogEntry.TaskId), 80);
            this.AddColumn("Root Task Id", nameof(LogEntry.RootTaskId), 90);
            this.AddColumn("Machine", nameof(LogEntry.Machine), 120);
            this.AddColumn("Process", nameof(LogEntry.ProcessDesc), 120);
            this.AddColumn("Log Struct Id", nameof(LogEntry.LogStructId), 90);
            this.AddColumn("Root Log Key", nameof(LogEntry.RootLogKey), 100);
            this.AddColumn("Context", nameof(LogEntry.PartitionKey), 80);

            ColumnVisibilityStore.ApplyToGrid(this._grid, this._logTableColumnVisibility);
            this._grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            this._grid.DataSource = entries;
            this.ApplyLogTypeColors();
        }

        private void AddColumn(string header, string propertyName, int width, string format = null)
        {
            var column = new DataGridViewTextBoxColumn
            {
                Name = propertyName,
                HeaderText = header,
                DataPropertyName = propertyName,
                Width = width,
                MinimumWidth = 40,
                Resizable = DataGridViewTriState.True
            };
            if (!string.IsNullOrEmpty(format))
            {
                column.DefaultCellStyle.Format = format;
            }
            this._grid.Columns.Add(column);
        }

        private void ApplyLogTypeColors()
        {
            foreach (DataGridViewRow row in this._grid.Rows)
            {
                if (row.DataBoundItem is LogEntry entry)
                {
                    switch (entry.LogType)
                    {
                        case "E":
                            row.DefaultCellStyle.ForeColor = Color.Red;
                            break;
                        case "P":
                            row.DefaultCellStyle.ForeColor = Color.Blue;
                            break;
                        case "W":
                            row.DefaultCellStyle.ForeColor = Color.FromArgb(255, 128, 64);
                            break;
                    }
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                this.ShowFindDialog();
                return true;
            }

            if (keyData == Keys.F3)
            {
                this.FindNext();
                return true;
            }

            if (keyData == (Keys.Shift | Keys.F3))
            {
                this.FindPrevious();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ClearFindState()
        {
            this._findText = null;
            this._findLastRow = -1;
            this._findLastColumn = -1;
            this._findHadMatch = false;
        }

        private void ShowFindDialog()
        {
            using (var dialog = new FindDialog(this._findText))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var text = dialog.SearchText;
                if (string.IsNullOrWhiteSpace(text))
                {
                    return;
                }

                this._findText = text;
                this._findLastRow = -1;
                this._findLastColumn = -1;
                this._findHadMatch = false;
                this.FindNextMatch(isNewSearch: true);
            }
        }

        private void FindNext()
        {
            if (string.IsNullOrEmpty(this._findText))
            {
                this.ShowFindDialog();
                return;
            }

            this.FindNextMatch(isNewSearch: false);
        }

        private void FindPrevious()
        {
            if (string.IsNullOrEmpty(this._findText))
            {
                this.ShowFindDialog();
                return;
            }

            this.FindPreviousMatch(isNewSearch: false);
        }

        private void FindNextMatch(bool isNewSearch)
        {
            if (string.IsNullOrEmpty(this._findText) || this._grid.Rows.Count == 0)
            {
                this.ShowFindNotFound();
                return;
            }

            var cells = this.BuildVisibleCellOrder();
            if (cells.Count == 0)
            {
                this.ShowFindNotFound();
                return;
            }

            var startIndex = this.GetFindStartIndex(cells, isNewSearch, forward: true);
            if (this.SearchCells(cells, startIndex, cells.Count, out var foundRow, out var foundColumn))
            {
                this.SelectFoundCell(foundRow, foundColumn);
                return;
            }

            if (this._findHadMatch && startIndex > 0 &&
                this.SearchCells(cells, 0, startIndex, out foundRow, out foundColumn))
            {
                this.SelectFoundCell(foundRow, foundColumn);
                return;
            }

            this.ShowFindNotFound();
            if (isNewSearch)
            {
                this._findHadMatch = false;
                this._findLastRow = -1;
                this._findLastColumn = -1;
            }
        }

        private void FindPreviousMatch(bool isNewSearch)
        {
            if (string.IsNullOrEmpty(this._findText) || this._grid.Rows.Count == 0)
            {
                this.ShowFindNotFound();
                return;
            }

            var cells = this.BuildVisibleCellOrder();
            if (cells.Count == 0)
            {
                this.ShowFindNotFound();
                return;
            }

            var startIndex = this.GetFindStartIndex(cells, isNewSearch, forward: false);
            if (this.SearchCellsBackward(cells, startIndex, 0, out var foundRow, out var foundColumn))
            {
                this.SelectFoundCell(foundRow, foundColumn);
                return;
            }

            if (this._findHadMatch && startIndex < cells.Count - 1 &&
                this.SearchCellsBackward(cells, cells.Count - 1, startIndex + 1, out foundRow, out foundColumn))
            {
                this.SelectFoundCell(foundRow, foundColumn);
                return;
            }

            this.ShowFindNotFound();
            if (isNewSearch)
            {
                this._findHadMatch = false;
                this._findLastRow = -1;
                this._findLastColumn = -1;
            }
        }

        private List<(int Row, int Column)> BuildVisibleCellOrder()
        {
            var cells = new List<(int, int)>();
            var columns = this._grid.Columns.Cast<DataGridViewColumn>()
                .Where(column => column.Visible)
                .OrderBy(column => column.DisplayIndex)
                .ToList();

            foreach (DataGridViewRow row in this._grid.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                foreach (var column in columns)
                {
                    cells.Add((row.Index, column.Index));
                }
            }

            return cells;
        }

        private int GetFindStartIndex(IReadOnlyList<(int Row, int Column)> cells, bool isNewSearch, bool forward)
        {
            if (isNewSearch)
            {
                var current = this._grid.CurrentCell;
                if (current == null)
                {
                    return forward ? 0 : cells.Count - 1;
                }

                for (var i = 0; i < cells.Count; i++)
                {
                    if (cells[i].Row == current.RowIndex && cells[i].Column == current.ColumnIndex)
                    {
                        return i;
                    }
                }

                return forward ? 0 : cells.Count - 1;
            }

            if (!this._findHadMatch)
            {
                return forward ? 0 : cells.Count - 1;
            }

            for (var i = 0; i < cells.Count; i++)
            {
                if (cells[i].Row == this._findLastRow && cells[i].Column == this._findLastColumn)
                {
                    return forward ? i + 1 : i - 1;
                }
            }

            return forward ? 0 : cells.Count - 1;
        }

        private bool SearchCells(
            IReadOnlyList<(int Row, int Column)> cells,
            int startIndex,
            int endIndex,
            out int foundRow,
            out int foundColumn)
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                var cell = cells[i];
                if (this.CellContainsFindText(cell.Row, cell.Column))
                {
                    foundRow = cell.Row;
                    foundColumn = cell.Column;
                    return true;
                }
            }

            foundRow = -1;
            foundColumn = -1;
            return false;
        }

        private bool SearchCellsBackward(
            IReadOnlyList<(int Row, int Column)> cells,
            int startIndex,
            int endIndex,
            out int foundRow,
            out int foundColumn)
        {
            for (var i = startIndex; i >= endIndex; i--)
            {
                var cell = cells[i];
                if (this.CellContainsFindText(cell.Row, cell.Column))
                {
                    foundRow = cell.Row;
                    foundColumn = cell.Column;
                    return true;
                }
            }

            foundRow = -1;
            foundColumn = -1;
            return false;
        }

        private bool CellContainsFindText(int rowIndex, int columnIndex)
        {
            var value = Convert.ToString(this._grid.Rows[rowIndex].Cells[columnIndex].FormattedValue) ?? string.Empty;
            return value.IndexOf(this._findText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void SelectFoundCell(int rowIndex, int columnIndex)
        {
            this._grid.ClearSelection();
            var cell = this._grid.Rows[rowIndex].Cells[columnIndex];
            cell.Selected = true;
            this._grid.CurrentCell = cell;
            this._grid.FirstDisplayedScrollingRowIndex = rowIndex;
            this._findLastRow = rowIndex;
            this._findLastColumn = columnIndex;
            this._findHadMatch = true;
        }

        private void ShowFindNotFound()
        {
            MessageBox.Show(this, "No matches found.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
