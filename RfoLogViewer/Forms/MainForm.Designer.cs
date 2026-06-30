namespace RfoLogViewer.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._toolStrip = new System.Windows.Forms.ToolStrip();
            this.dataMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.readExcelItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshSelectedItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectContextItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeWindowItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.findItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findPreviousItem = new System.Windows.Forms.ToolStripMenuItem();
            this._logStructColumnsMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this._logTableColumnsMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this._lblStatus = new System.Windows.Forms.ToolStripLabel();
            this._split = new System.Windows.Forms.SplitContainer();
            this._tree = new System.Windows.Forms.TreeView();
            this.treeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.treeCopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeContextMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this._treeDeleteLogStructMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._grid = new RfoLogViewer.Controls.LogDataGridView();
            this.gridContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._viewQueryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._layoutSaveTimer = new System.Windows.Forms.Timer(this.components);
            this._toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._split)).BeginInit();
            this._split.Panel1.SuspendLayout();
            this._split.Panel2.SuspendLayout();
            this._split.SuspendLayout();
            this.treeContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            this.gridContextMenu.SuspendLayout();
            this.SuspendLayout();
            //
            // _toolStrip
            //
            this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataMenu,
            this.findMenu,
            this._logStructColumnsMenu,
            this._logTableColumnsMenu,
            this._lblStatus});
            this._toolStrip.Location = new System.Drawing.Point(0, 0);
            this._toolStrip.Name = "_toolStrip";
            this._toolStrip.Size = new System.Drawing.Size(1400, 25);
            this._toolStrip.TabIndex = 0;
            this._toolStrip.Text = "toolStrip";
            //
            // dataMenu
            //
            this.dataMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readExcelItem,
            this.refreshItem,
            this.refreshSelectedItem,
            this.selectContextItem,
            this.closeWindowItem});
            this.dataMenu.Name = "dataMenu";
            this.dataMenu.Size = new System.Drawing.Size(43, 22);
            this.dataMenu.Text = "Data";
            //
            // readExcelItem
            //
            this.readExcelItem.Name = "readExcelItem";
            this.readExcelItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.readExcelItem.ShowShortcutKeys = true;
            this.readExcelItem.Size = new System.Drawing.Size(220, 22);
            this.readExcelItem.Text = "Read Excel log file...";
            this.readExcelItem.Click += new System.EventHandler(this.ReadExcelItem_Click);
            //
            // refreshItem
            //
            this.refreshItem.Name = "refreshItem";
            this.refreshItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshItem.ShowShortcutKeys = true;
            this.refreshItem.Size = new System.Drawing.Size(220, 22);
            this.refreshItem.Text = "Refresh";
            this.refreshItem.Click += new System.EventHandler(this.RefreshItem_Click);
            //
            // refreshSelectedItem
            //
            this.refreshSelectedItem.Name = "refreshSelectedItem";
            this.refreshSelectedItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.refreshSelectedItem.ShowShortcutKeys = true;
            this.refreshSelectedItem.Size = new System.Drawing.Size(220, 22);
            this.refreshSelectedItem.Text = "Refresh selected";
            this.refreshSelectedItem.Click += new System.EventHandler(this.RefreshSelectedItem_Click);
            //
            // selectContextItem
            //
            this.selectContextItem.Name = "selectContextItem";
            this.selectContextItem.Size = new System.Drawing.Size(220, 22);
            this.selectContextItem.Text = "Select Context...";
            this.selectContextItem.Click += new System.EventHandler(this.SelectContextItem_Click);
            //
            // closeWindowItem
            //
            this.closeWindowItem.Name = "closeWindowItem";
            this.closeWindowItem.Size = new System.Drawing.Size(220, 22);
            this.closeWindowItem.Text = "Close window";
            this.closeWindowItem.Click += new System.EventHandler(this.CloseWindowItem_Click);
            //
            // findMenu
            //
            this.findMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findItem,
            this.findNextItem,
            this.findPreviousItem});
            this.findMenu.Name = "findMenu";
            this.findMenu.Size = new System.Drawing.Size(42, 22);
            this.findMenu.Text = "Find";
            //
            // findItem
            //
            this.findItem.Name = "findItem";
            this.findItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findItem.ShowShortcutKeys = true;
            this.findItem.Size = new System.Drawing.Size(180, 22);
            this.findItem.Text = "Find...";
            this.findItem.Click += new System.EventHandler(this.FindItem_Click);
            //
            // findNextItem
            //
            this.findNextItem.Name = "findNextItem";
            this.findNextItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findNextItem.ShowShortcutKeys = true;
            this.findNextItem.Size = new System.Drawing.Size(180, 22);
            this.findNextItem.Text = "Find Next";
            this.findNextItem.Click += new System.EventHandler(this.FindNextItem_Click);
            //
            // findPreviousItem
            //
            this.findPreviousItem.Name = "findPreviousItem";
            this.findPreviousItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.findPreviousItem.ShowShortcutKeys = true;
            this.findPreviousItem.Size = new System.Drawing.Size(180, 22);
            this.findPreviousItem.Text = "Find Previous";
            this.findPreviousItem.Click += new System.EventHandler(this.FindPreviousItem_Click);
            //
            // _logStructColumnsMenu
            //
            this._logStructColumnsMenu.Name = "_logStructColumnsMenu";
            this._logStructColumnsMenu.Size = new System.Drawing.Size(120, 22);
            this._logStructColumnsMenu.Text = "Log Struct Columns";
            //
            // _logTableColumnsMenu
            //
            this._logTableColumnsMenu.Name = "_logTableColumnsMenu";
            this._logTableColumnsMenu.Size = new System.Drawing.Size(82, 22);
            this._logTableColumnsMenu.Text = "Log Columns";
            //
            // _lblStatus
            //
            this._lblStatus.Name = "_lblStatus";
            this._lblStatus.Size = new System.Drawing.Size(100, 22);
            this._lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // _split
            //
            this._split.Dock = System.Windows.Forms.DockStyle.Fill;
            this._split.Location = new System.Drawing.Point(0, 25);
            this._split.Name = "_split";
            //
            // _split.Panel1
            //
            this._split.Panel1.Controls.Add(this._tree);
            //
            // _split.Panel2
            //
            this._split.Panel2.Controls.Add(this._grid);
            this._split.Size = new System.Drawing.Size(1400, 875);
            this._split.SplitterDistance = 380;
            this._split.TabIndex = 1;
            this._split.Resize += new System.EventHandler(this.Split_Resize);
            this._split.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.Split_SplitterMoved);
            //
            // _tree
            //
            this._tree.ContextMenuStrip = this.treeContextMenu;
            this._tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tree.HideSelection = false;
            this._tree.Location = new System.Drawing.Point(0, 0);
            this._tree.Name = "_tree";
            this._tree.Size = new System.Drawing.Size(380, 875);
            this._tree.TabIndex = 0;
            this._tree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.Tree_BeforeExpand);
            this._tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.Tree_AfterSelect);
            this._tree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.Tree_NodeMouseClick);
            //
            // treeContextMenu
            //
            this.treeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.treeCopyMenuItem,
            this.treeContextMenuSeparator,
            this._treeDeleteLogStructMenuItem});
            this.treeContextMenu.Name = "treeContextMenu";
            this.treeContextMenu.Size = new System.Drawing.Size(181, 54);
            this.treeContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.TreeContextMenu_Opening);
            //
            // treeCopyMenuItem
            //
            this.treeCopyMenuItem.Name = "treeCopyMenuItem";
            this.treeCopyMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.treeCopyMenuItem.ShowShortcutKeys = true;
            this.treeCopyMenuItem.Size = new System.Drawing.Size(180, 22);
            this.treeCopyMenuItem.Text = "Copy";
            this.treeCopyMenuItem.Click += new System.EventHandler(this.TreeCopyMenuItem_Click);
            //
            // treeContextMenuSeparator
            //
            this.treeContextMenuSeparator.Name = "treeContextMenuSeparator";
            this.treeContextMenuSeparator.Size = new System.Drawing.Size(177, 6);
            //
            // _treeDeleteLogStructMenuItem
            //
            this._treeDeleteLogStructMenuItem.Name = "_treeDeleteLogStructMenuItem";
            this._treeDeleteLogStructMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this._treeDeleteLogStructMenuItem.ShowShortcutKeys = true;
            this._treeDeleteLogStructMenuItem.Size = new System.Drawing.Size(180, 22);
            this._treeDeleteLogStructMenuItem.Text = "Delete log struct";
            this._treeDeleteLogStructMenuItem.Click += new System.EventHandler(this.TreeDeleteLogStructMenuItem_Click);
            //
            // _grid
            //
            this._grid.AllowUserToResizeRows = false;
            this._grid.ContextMenuStrip = this.gridContextMenu;
            this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grid.Location = new System.Drawing.Point(0, 0);
            this._grid.Name = "_grid";
            this._grid.Size = new System.Drawing.Size(1016, 875);
            this._grid.TabIndex = 0;
            this._grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Grid_CellDoubleClick);
            this._grid.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.Grid_CellMouseDown);
            this._grid.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.Grid_ColumnLayoutChanged);
            this._grid.ColumnDisplayIndexChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.Grid_ColumnLayoutChanged);
            //
            // gridContextMenu
            //
            this.gridContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._viewQueryMenuItem});
            this.gridContextMenu.Name = "gridContextMenu";
            this.gridContextMenu.Size = new System.Drawing.Size(141, 26);
            this.gridContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.GridContextMenu_Opening);
            //
            // _viewQueryMenuItem
            //
            this._viewQueryMenuItem.Name = "_viewQueryMenuItem";
            this._viewQueryMenuItem.Size = new System.Drawing.Size(140, 22);
            this._viewQueryMenuItem.Text = "View query...";
            this._viewQueryMenuItem.Click += new System.EventHandler(this.ViewQueryMenuItem_Click);
            //
            // _layoutSaveTimer
            //
            this._layoutSaveTimer.Interval = 400;
            this._layoutSaveTimer.Tick += new System.EventHandler(this.LayoutSaveTimer_Tick);
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 900);
            this.Controls.Add(this._split);
            this.Controls.Add(this._toolStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RFo Log Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this._toolStrip.ResumeLayout(false);
            this._toolStrip.PerformLayout();
            this._split.Panel1.ResumeLayout(false);
            this._split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._split)).EndInit();
            this._split.ResumeLayout(false);
            this.treeContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            this.gridContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip _toolStrip;
        private System.Windows.Forms.ToolStripDropDownButton dataMenu;
        private System.Windows.Forms.ToolStripMenuItem readExcelItem;
        private System.Windows.Forms.ToolStripMenuItem refreshItem;
        private System.Windows.Forms.ToolStripMenuItem refreshSelectedItem;
        private System.Windows.Forms.ToolStripMenuItem selectContextItem;
        private System.Windows.Forms.ToolStripMenuItem closeWindowItem;
        private System.Windows.Forms.ToolStripDropDownButton findMenu;
        private System.Windows.Forms.ToolStripMenuItem findItem;
        private System.Windows.Forms.ToolStripMenuItem findNextItem;
        private System.Windows.Forms.ToolStripMenuItem findPreviousItem;
        private System.Windows.Forms.ToolStripDropDownButton _logStructColumnsMenu;
        private System.Windows.Forms.ToolStripDropDownButton _logTableColumnsMenu;
        private System.Windows.Forms.ToolStripLabel _lblStatus;
        private System.Windows.Forms.SplitContainer _split;
        private System.Windows.Forms.TreeView _tree;
        private System.Windows.Forms.ContextMenuStrip treeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem treeCopyMenuItem;
        private System.Windows.Forms.ToolStripSeparator treeContextMenuSeparator;
        private System.Windows.Forms.ToolStripMenuItem _treeDeleteLogStructMenuItem;
        private RfoLogViewer.Controls.LogDataGridView _grid;
        private System.Windows.Forms.ContextMenuStrip gridContextMenu;
        private System.Windows.Forms.ToolStripMenuItem _viewQueryMenuItem;
        private System.Windows.Forms.Timer _layoutSaveTimer;
    }
}
