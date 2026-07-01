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
    public partial class ExcelLogViewerForm : Form
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
        private readonly Dictionary<string, bool> _logTableColumnVisibility;
        private bool _suppressLayoutSave;
        private bool _suppressSplitterSave;
        private int _savedSplitterDistance;
        private bool _splitterRestorePending = true;
        private bool _splitterSaveEnabled;
        private string _findText;
        private int _findLastRow = -1;
        private int _findLastColumn = -1;
        private bool _findHadMatch;

        public ExcelLogViewerForm()
        {
            this.InitializeComponent();
        }

        public ExcelLogViewerForm(string filePath, IList<LogEntry> entries, TreeNode rootNode) : this()
        {
            this._entries = entries ?? throw new ArgumentNullException(nameof(entries));

            this.Text = $"RFo Log Viewer - {Path.GetFileName(filePath)}";
            this.Icon = AppIcon.Get();
            this.LoadWindowSettings();

            this._logTableColumnVisibility = ColumnVisibilityStore.Load(
                Settings.Default.ExcelLogTableColumnVisibility,
                LogTableColumnHeaders.Keys);
            this.PopulateColumnVisibilityMenu();

            this._tree.Nodes.Add(rootNode);
            rootNode.Expand();
            this.SelectFirstLogSessionNode();
            this._lblStatus.Text = $"{entries.Count} log row(s) loaded. Select a tree node to display logs.";
            this.Text += $" - {entries.Count} log row(s)";
        }

        private void PopulateColumnVisibilityMenu()
        {
            this._logColumnsMenu.DropDownItems.Clear();
            foreach (var column in LogTableColumnHeaders)
            {
                var item = new ToolStripMenuItem(column.Value)
                {
                    Tag = column.Key,
                    CheckOnClick = true,
                    Checked = this._logTableColumnVisibility[column.Key]
                };
                item.Click += this.LogColumnMenuItem_Click;
                this._logColumnsMenu.DropDownItems.Add(item);
            }
        }

        private void LogColumnMenuItem_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            var key = (string)item.Tag;
            this._logTableColumnVisibility[key] = item.Checked;
            ColumnVisibilityStore.ApplyToGrid(this._grid, this._logTableColumnVisibility);
            this.SaveColumnVisibilitySettings();
        }

        private void FindItem_Click(object sender, EventArgs e) => this.ShowFindDialog();
        private void FindNextItem_Click(object sender, EventArgs e) => this.FindNext();
        private void FindPreviousItem_Click(object sender, EventArgs e) => this.FindPrevious();
        private void TreeCopyMenuItem_Click(object sender, EventArgs e) => this.CopySelectedTreeNodeLabel();
        private void Grid_ColumnLayoutChanged(object sender, EventArgs e) => this.ScheduleColumnLayoutSave();
        private void Split_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!this._splitterSaveEnabled)
            {
                return;
            }

            this.SaveSplitterLayout();
        }

        private void Split_Resize(object sender, EventArgs e)
        {
            if (this._splitterRestorePending)
            {
                this.TryRestoreSplitterDistance();
            }
        }

        private void LayoutSaveTimer_Tick(object sender, EventArgs e)
        {
            this._layoutSaveTimer.Stop();
            this.SaveColumnLayout();
        }

        private void ExcelLogViewerForm_Shown(object sender, EventArgs e)
        {
            this.TryRestoreSplitterDistance();
            this.BeginInvoke(new Action(() =>
            {
                this.TryRestoreSplitterDistance();
                this.BeginInvoke(new Action(this.FinishSplitterRestore));
            }));
        }
        private void ExcelLogViewerForm_ResizeEnd(object sender, EventArgs e)
        {
            if (this._splitterRestorePending)
            {
                this.TryRestoreSplitterDistance();
            }

            this.SaveWindowSettings();
        }
        private void ExcelLogViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveColumnLayout();
            this.SaveColumnVisibilitySettings();
            if (this._splitterSaveEnabled)
            {
                this.SaveSplitterLayout();
            }

            this.SaveWindowSettings();
        }

        private void FinishSplitterRestore()
        {
            this.TryRestoreSplitterDistance();
            this._splitterRestorePending = false;
            this._splitterSaveEnabled = true;
        }

        private void Tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this._tree.SelectedNode = e.Node;
            }
        }

        private void TreeContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var menu = (ContextMenuStrip)sender;
            var canCopy = this._tree.SelectedNode?.Tag is ExcelTreeNodeTag;
            menu.Items[0].Enabled = canCopy;
            if (!canCopy)
            {
                e.Cancel = true;
            }
        }

        private void CopySelectedTreeNodeLabel()
        {
            var selectedNode = this._tree.SelectedNode;
            if (selectedNode?.Tag is ExcelTreeNodeTag)
            {
                Clipboard.SetText(selectedNode.Text);
            }
        }

        private void SelectFirstLogSessionNode()
        {
            var node = this.FindFirstLogSessionNode(this._tree.Nodes);
            if (node != null)
            {
                this._tree.SelectedNode = node;
            }
        }

        private TreeNode FindFirstLogSessionNode(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag is ExcelTreeNodeTag tag && tag.ItemType == ExcelTreeItemType.LogSession)
                {
                    return node;
                }

                var child = this.FindFirstLogSessionNode(node.Nodes);
                if (child != null)
                {
                    return child;
                }
            }

            return null;
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
            this.RestoreColumnLayout(Settings.Default.ExcelLogTableColumnLayout);
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

        private void LoadWindowSettings()
        {
            var settings = Settings.Default;
            this.Width = settings.ExcelLogViewerFormWidth > 0 ? settings.ExcelLogViewerFormWidth : 1400;
            this.Height = settings.ExcelLogViewerFormHeight > 0 ? settings.ExcelLogViewerFormHeight : 900;
            this._savedSplitterDistance = settings.ExcelLogViewerSplitterDistance > 0
                ? settings.ExcelLogViewerSplitterDistance
                : 380;
        }

        private void TryRestoreSplitterDistance()
        {
            if (!this._splitterRestorePending || this._savedSplitterDistance <= 0)
            {
                return;
            }

            var split = this._split;
            var distance = SplitContainerLayoutStore.GetClampedDistance(split, this._savedSplitterDistance);
            if (distance < 0)
            {
                return;
            }

            this._suppressSplitterSave = true;
            try
            {
                if (split.SplitterDistance != distance)
                {
                    split.SplitterDistance = distance;
                }
            }
            finally
            {
                this._suppressSplitterSave = false;
            }
        }

        private void SaveWindowSettings()
        {
            if (this.WindowState != FormWindowState.Normal)
            {
                return;
            }

            var settings = Settings.Default;
            settings.ExcelLogViewerFormWidth = this.Width;
            settings.ExcelLogViewerFormHeight = this.Height;
            settings.Save();
        }

        private void SaveSplitterLayout()
        {
            if (!this._splitterSaveEnabled || this._suppressSplitterSave)
            {
                return;
            }

            Settings.Default.ExcelLogViewerSplitterDistance = this._split.SplitterDistance;
            Settings.Default.Save();
        }

        private void ScheduleDeferredLayoutSave()
        {
            this._layoutSaveTimer.Stop();
            this._layoutSaveTimer.Start();
        }

        private void RestoreColumnLayout(string layout)
        {
            if (string.IsNullOrWhiteSpace(layout))
            {
                return;
            }

            this._suppressLayoutSave = true;
            try
            {
                DataGridViewLayoutStore.Apply(this._grid, layout);
            }
            finally
            {
                this._suppressLayoutSave = false;
            }
        }

        private void ScheduleColumnLayoutSave()
        {
            if (this._suppressLayoutSave || this._grid.Columns.Count == 0)
            {
                return;
            }

            this.ScheduleDeferredLayoutSave();
        }

        private void SaveColumnLayout()
        {
            if (this._suppressLayoutSave || this._grid.Columns.Count == 0)
            {
                return;
            }

            Settings.Default.ExcelLogTableColumnLayout = DataGridViewLayoutStore.Serialize(this._grid);
            Settings.Default.Save();
        }

        private void SaveColumnVisibilitySettings()
        {
            Settings.Default.ExcelLogTableColumnVisibility = ColumnVisibilityStore.Save(this._logTableColumnVisibility);
            Settings.Default.Save();
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

            if (keyData == (Keys.Control | Keys.C) && this._tree.Focused)
            {
                if (this._tree.SelectedNode?.Tag is ExcelTreeNodeTag)
                {
                    this.CopySelectedTreeNodeLabel();
                    return true;
                }
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
