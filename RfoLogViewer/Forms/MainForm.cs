using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RfoLogViewer.Controls;
using RfoLogViewer.Data;
using RfoLogViewer.Models;
using RfoLogViewer.Properties;
using RfoLogViewer.Services;

namespace RfoLogViewer.Forms
{
	public sealed class MainForm : Form
	{
		private static readonly IReadOnlyDictionary<string, string> LogStructColumnHeaders =
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				["log_struct_id"] = "Log Struct Id",
				["start_timestamp"] = "Start Timestamp",
				["end_timestamp"] = "End Timestamp",
				["session_id"] = "Session Id",
				["depth"] = "Depth",
				["log_key"] = "Log Key",
				["root_log_key"] = "Root Log Key",
				["task_id"] = "Task Id",
				["root_task_id"] = "Root Task Id",
				["process_desc"] = "Process",
				["machine"] = "Machine",
				["nb_errors"] = "Errors",
				["nb_warnings"] = "Warnings",
				["nb_infos"] = "Infos",
				["total_errors"] = "Total Errors",
				["total_warnings"] = "Total Warnings",
				["context_id"] = "Context Id"
			};

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
		private const string LayoutLogTable = "LogTable";
		private const string LayoutLogStruct = "LogStruct";

		private readonly OracleLogRepository _repository;
		private readonly long _userId;
		private long _contextId;
		private readonly TreeView _tree;
		private readonly LogDataGridView _grid;
		private readonly ToolStrip _toolStrip;
		private readonly ToolStripDropDownButton _logStructColumnsMenu;
		private readonly ToolStripDropDownButton _logTableColumnsMenu;
		private readonly ToolStripLabel _lblStatus;
		private readonly SplitContainer _split;
		private readonly Timer _layoutSaveTimer;
		private readonly ToolStripMenuItem _viewQueryMenuItem;
		private bool _loadingTreeNode;
		private bool _suppressLayoutSave;
		private bool _isLogTableView;
		private string _currentColumnLayoutKey;
		private int _pendingSplitterDistance = -1;
		private bool _suppressWindowSettingsSave;
		private Dictionary<string, bool> _logStructColumnVisibility;
		private Dictionary<string, bool> _logTableColumnVisibility;
		private string _findText;
		private int _findLastRow = -1;
		private int _findLastColumn = -1;
		private bool _findHadMatch;

		public MainForm(OracleLogRepository repository, long userId, long contextId)
		{
			this._repository = repository;
			this._userId = userId;
			this._contextId = contextId;

			this.InitializeTitle();
			this.Icon = AppIcon.Get();
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Font = new Font("Segoe UI", 9F);
			this.LoadWindowSettings();
			this.LoadColumnVisibilitySettings();

			ToolStripDropDownButton dataMenu = new ToolStripDropDownButton("Data");
			ToolStripMenuItem readExcelItem = new ToolStripMenuItem("Read Excel log file...") {
				ShortcutKeys = Keys.Control | Keys.O,
				ShowShortcutKeys = true
			};
			readExcelItem.Click += (_, __) => this.OpenExcelLogFile();
			dataMenu.DropDownItems.Add(readExcelItem);

			ToolStripMenuItem refreshItem = new ToolStripMenuItem("Refresh")
			{
				ShortcutKeys = Keys.F5,
				ShowShortcutKeys = true
			};
			refreshItem.Click += (_, __) => this.RefreshCurrentView(preserveTree: true);
			ToolStripMenuItem refreshSelectedItem = new ToolStripMenuItem("Refresh selected")
			{
				ShortcutKeys = Keys.F7,
				ShowShortcutKeys = true
			};
			refreshSelectedItem.Click += (_, __) => this.RefreshSelectedView();
			ToolStripMenuItem selectContextItem = new ToolStripMenuItem("Select Context...");
			selectContextItem.Click += (_, __) => this.ShowSelectContextDialog();
			ToolStripMenuItem closeWindowItem = new ToolStripMenuItem("Close window");
			closeWindowItem.Click += (_, __) => this.Close();
			dataMenu.DropDownItems.Add(refreshItem);
			dataMenu.DropDownItems.Add(refreshSelectedItem);
			dataMenu.DropDownItems.Add(selectContextItem);
			dataMenu.DropDownItems.Add(closeWindowItem);

			this._lblStatus = new ToolStripLabel { TextAlign = ContentAlignment.MiddleLeft };

			ToolStripDropDownButton findMenu = new ToolStripDropDownButton("Find");
			ToolStripMenuItem findItem = new ToolStripMenuItem("Find...")
			{
				ShortcutKeys = Keys.Control | Keys.F,
				ShowShortcutKeys = true
			};
			findItem.Click += (_, __) => this.ShowFindDialog();
			ToolStripMenuItem findNextItem = new ToolStripMenuItem("Find Next")
			{
				ShortcutKeys = Keys.F3,
				ShowShortcutKeys = true
			};
			findNextItem.Click += (_, __) => this.FindNext();
			ToolStripMenuItem findPreviousItem = new ToolStripMenuItem("Find Previous")
			{
				ShortcutKeys = Keys.Shift | Keys.F3,
				ShowShortcutKeys = true
			};
			findPreviousItem.Click += (_, __) => this.FindPrevious();
			findMenu.DropDownItems.Add(findItem);
			findMenu.DropDownItems.Add(findNextItem);
			findMenu.DropDownItems.Add(findPreviousItem);

			this._logStructColumnsMenu = this.CreateColumnVisibilityMenu(
				"Log Struct Columns",
				LogStructColumnHeaders,
				this._logStructColumnVisibility,
				this.LogStructColumnMenuItem_Click);
			this._logTableColumnsMenu = this.CreateColumnVisibilityMenu(
				"Log Columns",
				LogTableColumnHeaders,
				this._logTableColumnVisibility,
				this.LogTableColumnMenuItem_Click);

			this._toolStrip = new ToolStrip();
			this._toolStrip.Items.Add(dataMenu);
			this._toolStrip.Items.Add(findMenu);
			this._toolStrip.Items.Add(this._logStructColumnsMenu);
			this._toolStrip.Items.Add(this._logTableColumnsMenu);
			//this._toolStrip.Items.Add(this._lblStatus);

			this.KeyPreview = true;

			this._tree = new TreeView
			{
				Dock = DockStyle.Fill,
				HideSelection = false,
				ImageList = LogTreeImageList.Get()
			};
			this._tree.BeforeExpand += this.Tree_BeforeExpand;
			this._tree.AfterSelect += this.Tree_AfterSelect;

			this._grid = new LogDataGridView { Dock = DockStyle.Fill };
			this._grid.ColumnWidthChanged += (_, __) => this.ScheduleColumnLayoutSave();
			this._grid.ColumnDisplayIndexChanged += (_, __) => this.ScheduleColumnLayoutSave();
			this._grid.CellDoubleClick += this.Grid_CellDoubleClick;
			this._grid.CellMouseDown += this.Grid_CellMouseDown;
			this._grid.AllowUserToResizeRows = false;

			this._viewQueryMenuItem = new ToolStripMenuItem("View query...");
			this._viewQueryMenuItem.Click += (_, __) => this.ViewSelectedLogQuery();
			var gridContextMenu = new ContextMenuStrip();
			gridContextMenu.Opening += this.GridContextMenu_Opening;
			gridContextMenu.Items.Add(this._viewQueryMenuItem);
			this._grid.ContextMenuStrip = gridContextMenu;

			this._split = new SplitContainer
			{
				Dock = DockStyle.Fill
			};
			this._split.SplitterMoved += (_, __) => this.SaveWindowSettings();
			this._split.Panel1.Controls.Add(this._tree);
			this._split.Panel2.Controls.Add(this._grid);

			this._layoutSaveTimer = new Timer { Interval = 400 };
			this._layoutSaveTimer.Tick += (_, __) =>
			{
				this._layoutSaveTimer.Stop();
				this.SaveColumnLayout();
			};

			this.Controls.Add(this._split);
			this.Controls.Add(this._toolStrip);

			this.ResizeEnd += (_, __) => this.SaveWindowSettings();
			this.FormClosing += (_, __) =>
			{
				this.SaveColumnLayout();
				this.SaveColumnVisibilitySettings();
				this.SaveWindowSettings();
			};
			this.Load += (_, __) =>
			{
				this.InitializeTree();
				this.ApplyPendingSplitterDistance();
			};
			this.Shown += (_, __) => this.ApplyPendingSplitterDistance();
		}

		private void InitializeTitle()
		{
			this.Text = $"RFo Log Viewer - {this._repository.GetCurrentConnectionString()} - contextId={this._repository.GetCurrentContextId()}";
		}

		private void OpenExcelLogFile()
		{
			using (var dialog = new OpenFileDialog
			{
				Title = "Read Excel log file",
				Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls|All files (*.*)|*.*",
				CheckFileExists = true
			})
			{
				if (dialog.ShowDialog(this) != DialogResult.OK)
				{
					return;
				}

				try
				{
					this.Cursor = Cursors.WaitCursor;
					var entries = ExcelLogReader.Load(dialog.FileName);
					if (entries.Count == 0)
					{
						MessageBox.Show(this, "No log rows found in the selected file.", "Read Excel log file",
							MessageBoxButtons.OK, MessageBoxIcon.Information);
						return;
					}

					var rootNode = ExcelLogTreeBuilder.Build(entries);
					var viewer = new ExcelLogViewerForm(dialog.FileName, entries, rootNode);
					viewer.Show();
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, ex.Message, "Read Excel log file", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				finally
				{
					this.Cursor = Cursors.Default;
				}
			}
		}

		private void InitializeTree()
		{
			this._tree.Nodes.Clear();
			var rootTag = new TreeNodeTag
			{
				ItemType = LogTreeItemType.Root,
				ChildrenLoaded = true
			};
			var root = this.CreateNode("Log", rootTag);
			LogNodeStatusHelper.ApplyToNode(root, LogNodeStatus.Normal);
			this._tree.Nodes.Add(root);
			this.AddPeriodNodes(root);
			this.UpdateStatusFromChildren(root);
			root.Expand();
			this._lblStatus.Text = $"Context {this._contextId}. Select a tree node to display logs.";
		}

		private void ShowSelectContextDialog()
		{
			IList<ContextEntry> contexts;
			long currentContextId;
			try
			{
				this.Cursor = Cursors.WaitCursor;
				contexts = this._repository.GetAccessibleContexts();
				currentContextId = this._repository.GetCurrentContextId();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Select Context", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}

			if (contexts.Count == 0)
			{
				MessageBox.Show(this, "No accessible contexts found.", "Select Context", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			try
			{
				using (var dialog = new ContextSelectionForm(contexts, currentContextId))
				{
					if (dialog.ShowDialog(this) != DialogResult.OK || !dialog.SelectedContextId.HasValue)
					{
						return;
					}

					var newContextId = dialog.SelectedContextId.Value;
					this.Cursor = Cursors.WaitCursor;
					this._repository.OpenContext(newContextId, this._userId);
					this._contextId = newContextId;
					ConnectionForm.SaveSettings(
						Settings.Default.LastLogin,
						Settings.Default.SavePassword ? Settings.Default.LastPassword : string.Empty,
						Settings.Default.LastDataSource,
						this._contextId,
						Settings.Default.SavePassword);
					this.ReloadAfterContextChange();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Select Context", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void ReloadAfterContextChange()
		{
			this.InitializeTitle();
			this.ClearFindState();
			this._suppressLayoutSave = true;
			try
			{
				this._grid.DataSource = null;
				this._grid.Columns.Clear();
				this._isLogTableView = false;
				this._currentColumnLayoutKey = null;
				this.InitializeTree();
			}
			finally
			{
				this._suppressLayoutSave = false;
			}
		}

		private void AddPeriodNodes(TreeNode root)
		{
			var today = DateTime.Today;
			this.AddPeriodNode(root, "Today", today, today.AddDays(1));
			this.AddPeriodNode(root, "Yesterday", today.AddDays(-1), today);
			this.AddPeriodNode(root, "Two days ago", today.AddDays(-2), today.AddDays(-1));
			this.AddPeriodNode(root, "Three days ago", today.AddDays(-3), today.AddDays(-2));
			this.AddPeriodNode(root, "More than 3 days ago", new DateTime(1990, 1, 1), today.AddDays(-3));
		}

		private void AddPeriodNode(TreeNode parent, string label, DateTime begin, DateTime end)
		{
			var tag = new TreeNodeTag
			{
				ItemType = LogTreeItemType.Period,
				PeriodBegin = begin,
				PeriodEnd = end,
				Status = this._repository.GetPeriodStatus(begin, end)
			};
			var node = this.CreateNode(label, tag);
			LogNodeStatusHelper.ApplyToNode(node, tag.Status);
			node.Nodes.Add(this.CreatePlaceholderNode());
			parent.Nodes.Add(node);
		}

		private void Tree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (this._loadingTreeNode || e.Node == null)
			{
				return;
			}

			var tag = e.Node.Tag as TreeNodeTag;
			if (tag == null || tag.ChildrenLoaded || tag.ItemType == LogTreeItemType.Root)
			{
				return;
			}

			try
			{
				this._loadingTreeNode = true;
				this.Cursor = Cursors.WaitCursor;
				e.Node.Nodes.Clear();
				this.LoadChildren(e.Node, tag);
				tag.ChildrenLoaded = true;
				this.UpdateStatusFromChildren(e.Node);
				if (e.Node.Parent != null)
				{
					this.UpdateStatusFromChildren(e.Node.Parent);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Error loading tree", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				this.Cursor = Cursors.Default;
				this._loadingTreeNode = false;
			}
		}

		private void LoadChildren(TreeNode node, TreeNodeTag tag)
		{
			switch (tag.ItemType)
			{
				case LogTreeItemType.Period:
					this.LoadPeriodChildren(node, tag);
					break;
				case LogTreeItemType.RootLogKey:
					this.LoadRootLogSessions(node, tag);
					break;
				case LogTreeItemType.LogSession:
					this.LoadNestedLogSessions(node, tag);
					break;
			}
		}

		private void LoadPeriodChildren(TreeNode node, TreeNodeTag tag)
		{
			foreach (var item in this._repository.GetRootLogKeys(tag.PeriodBegin, tag.PeriodEnd))
			{
				var childTag = new TreeNodeTag
				{
					ItemType = LogTreeItemType.RootLogKey,
					PeriodBegin = tag.PeriodBegin,
					PeriodEnd = tag.PeriodEnd,
					RootLogKey = item.RootLogKey,
					Status = item.Status
				};
				var child = this.CreateNode(item.Label, childTag);
				LogNodeStatusHelper.ApplyToNode(child, item.Status);
				child.Nodes.Add(this.CreatePlaceholderNode());
				node.Nodes.Add(child);
			}

			var orphanStatus = this._repository.GetOrphanStatus(tag.PeriodBegin, tag.PeriodEnd);
			var orphanTag = new TreeNodeTag
			{
				ItemType = LogTreeItemType.Orphan,
				PeriodBegin = tag.PeriodBegin,
				PeriodEnd = tag.PeriodEnd,
				ChildrenLoaded = true,
				Status = orphanStatus
			};
			var orphanNode = this.CreateNode("Orphan logs (without session)", orphanTag);
			LogNodeStatusHelper.ApplyToNode(orphanNode, orphanStatus);
			node.Nodes.Add(orphanNode);
		}

		private void LoadRootLogSessions(TreeNode node, TreeNodeTag tag)
		{
			foreach (var session in this._repository.GetRootLogSessions(tag.PeriodBegin, tag.PeriodEnd, tag.RootLogKey))
			{
				node.Nodes.Add(this.CreateLogSessionNode(session));
			}
			this.UpdateStatusFromChildren(node);
		}

		private void LoadNestedLogSessions(TreeNode node, TreeNodeTag tag)
		{
			if (!tag.LogStructId.HasValue)
			{
				return;
			}

			foreach (var session in this._repository.GetChildLogSessions(tag.LogStructId.Value, tag.RootDurationSeconds))
			{
				node.Nodes.Add(this.CreateLogSessionNode(session));
			}
			this.UpdateStatusFromChildren(node);
		}

		private TreeNode CreateLogSessionNode(LogSessionNodeInfo session)
		{
			var childTag = new TreeNodeTag
			{
				ItemType = LogTreeItemType.LogSession,
				LogStructId = session.LogStructId,
				ParentLogStructId = session.ParentLogStructId,
				RootDurationSeconds = session.RootDurationSeconds,
				PictureIndex = session.PictureIndex,
				Status = session.Status
			};
			var child = this.CreateNode(session.Label, childTag);
			LogNodeStatusHelper.ApplyToNode(child, session.Status, session.PictureIndex);
			if (session.HasChildren)
			{
				child.Nodes.Add(this.CreatePlaceholderNode());
			}
			return child;
		}

		private void Tree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (this._loadingTreeNode || e.Node == null)
			{
				return;
			}
			this.LoadGridForNode(e.Node);
		}

		private void LoadGridForNode(TreeNode node)
		{
			this.ClearFindState();

			var tag = node.Tag as TreeNodeTag;
			if (tag == null)
			{
				return;
			}

			try
			{
				this.Cursor = Cursors.WaitCursor;
				switch (tag.ItemType)
				{
					case LogTreeItemType.Period:
						this.BindLogStruct(
							"parent_log_struct_id IS NULL " +
							"AND start_timestamp >= TO_DATE('" + tag.PeriodBegin.ToString("yyyyMMdd") + "','YYYYMMDD') " +
							"AND start_timestamp < TO_DATE('" + tag.PeriodEnd.ToString("yyyyMMdd") + "','YYYYMMDD')");
						break;
					case LogTreeItemType.RootLogKey:
						this.BindLogStruct(
							"root_log_key = '" + this.EscapeSqlLiteral(tag.RootLogKey) + "' " +
							"AND start_timestamp >= TO_DATE('" + tag.PeriodBegin.ToString("yyyyMMdd") + "','YYYYMMDD') " +
							"AND start_timestamp < TO_DATE('" + tag.PeriodEnd.ToString("yyyyMMdd") + "','YYYYMMDD')");
						break;
					case LogTreeItemType.Orphan:
						this.BindLogTable(
							"root_log_key IS NULL " +
							"AND datetime >= TO_DATE('" + tag.PeriodBegin.ToString("yyyyMMdd") + "','YYYYMMDD') " +
							"AND datetime < TO_DATE('" + tag.PeriodEnd.ToString("yyyyMMdd") + "','YYYYMMDD') " +
							"AND log_type <> 'P'");
						break;
					case LogTreeItemType.LogSession:
						this.BindLogTable(
							"log_struct_id IN (" +
							"SELECT log_struct_id FROM log_struct " +
							"START WITH log_struct_id = " + tag.LogStructId +
							" CONNECT BY PRIOR log_struct_id = parent_log_struct_id) " +
							"AND log_type <> 'P'");
						break;
					default:
						this._grid.DataSource = null;
						break;
				}
				this._lblStatus.Text = node.FullPath.Replace('\\', '/');
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Error loading data", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void BindLogTable(string whereClause)
		{
			this._isLogTableView = true;
			var entries = this._repository.LoadLogTable(whereClause);
			this._grid.DataSource = null;
			this._grid.Columns.Clear();
			this._grid.AutoGenerateColumns = false;
			this._currentColumnLayoutKey = LayoutLogTable;

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

			this.RestoreColumnLayout(Settings.Default.LogTableColumnLayout);
			ColumnVisibilityStore.ApplyToGrid(this._grid, this._logTableColumnVisibility);

			this._grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
			this._grid.DataSource = entries;
			this.ApplyLogTypeColors();
		}

		private void BindLogStruct(string whereClause)
		{
			this._isLogTableView = false;
			var table = this._repository.LoadLogStruct(whereClause);
			this._grid.DataSource = null;
			this._grid.Columns.Clear();
			this._grid.AutoGenerateColumns = true;
			this._currentColumnLayoutKey = LayoutLogStruct;
			this._grid.DataSource = table;
			this._grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
			this.ApplyLogStructColumnHeaders();
			foreach (DataGridViewColumn column in this._grid.Columns)
			{
				column.Resizable = DataGridViewTriState.True;
				if (column.ValueType == typeof(DateTime) || column.ValueType == typeof(DateTime?))
				{
					column.DefaultCellStyle.Format = TimestampFormat;
					if (column.Width < 190)
					{
						column.Width = 190;
					}
				}
				else if (column.Width < 80)
				{
					column.Width = 80;
				}
			}

			this.RestoreColumnLayout(Settings.Default.LogStructColumnLayout);
			ColumnVisibilityStore.ApplyToGrid(this._grid, this._logStructColumnVisibility);
		}

		private void ApplyLogStructColumnHeaders()
		{
			foreach (DataGridViewColumn column in this._grid.Columns)
			{
				var key = column.DataPropertyName ?? column.Name;
				if (LogStructColumnHeaders.TryGetValue(key, out var header))
				{
					column.HeaderText = header;
				}
			}
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

		private void LoadColumnVisibilitySettings()
		{
			var settings = Settings.Default;
			this._logStructColumnVisibility = ColumnVisibilityStore.Load(
				settings.LogStructColumnVisibility,
				LogStructColumnHeaders.Keys);
			this._logTableColumnVisibility = ColumnVisibilityStore.Load(
				settings.LogTableColumnVisibility,
				LogTableColumnHeaders.Keys);
		}

		private void SaveColumnVisibilitySettings()
		{
			var settings = Settings.Default;
			settings.LogStructColumnVisibility = ColumnVisibilityStore.Save(this._logStructColumnVisibility);
			settings.LogTableColumnVisibility = ColumnVisibilityStore.Save(this._logTableColumnVisibility);
			settings.Save();
		}

		private ToolStripDropDownButton CreateColumnVisibilityMenu(
			string title,
			IReadOnlyDictionary<string, string> columns,
			IReadOnlyDictionary<string, bool> visibility,
			EventHandler itemClickHandler)
		{
			var menu = new ToolStripDropDownButton(title);
			foreach (var column in columns)
			{
				var item = new ToolStripMenuItem(column.Value)
				{
					Tag = column.Key,
					CheckOnClick = true,
					Checked = visibility[column.Key]
				};
				item.Click += itemClickHandler;
				menu.DropDownItems.Add(item);
			}

			return menu;
		}

		private void LogStructColumnMenuItem_Click(object sender, EventArgs e)
		{
			var item = (ToolStripMenuItem)sender;
			var key = (string)item.Tag;
			this._logStructColumnVisibility[key] = item.Checked;
			if (!this._isLogTableView)
			{
				ColumnVisibilityStore.ApplyToGrid(this._grid, this._logStructColumnVisibility);
			}
			this.SaveColumnVisibilitySettings();
		}

		private void LogTableColumnMenuItem_Click(object sender, EventArgs e)
		{
			var item = (ToolStripMenuItem)sender;
			var key = (string)item.Tag;
			this._logTableColumnVisibility[key] = item.Checked;
			if (this._isLogTableView)
			{
				ColumnVisibilityStore.ApplyToGrid(this._grid, this._logTableColumnVisibility);
			}
			this.SaveColumnVisibilitySettings();
		}

		private void LoadWindowSettings()
		{
			var settings = Settings.Default;
			this.Width = settings.MainFormWidth > 0 ? settings.MainFormWidth : 1400;
			this.Height = settings.MainFormHeight > 0 ? settings.MainFormHeight : 900;
			this._pendingSplitterDistance = settings.SplitterDistance > 0 ? settings.SplitterDistance : 380;
		}

		private void ApplyPendingSplitterDistance()
		{
			if (this._pendingSplitterDistance < 0)
			{
				return;
			}

			var split = this._split;
			var available = split.ClientSize.Width - split.SplitterWidth;
			if (available < split.Panel1MinSize + split.Panel2MinSize)
			{
				return;
			}

			var distance = this._pendingSplitterDistance;
			this._suppressWindowSettingsSave = true;
			try
			{
				var max = available - split.Panel2MinSize;
				split.SplitterDistance = Math.Max(split.Panel1MinSize, Math.Min(distance, max));
				this._pendingSplitterDistance = -1;
			}
			finally
			{
				this._suppressWindowSettingsSave = false;
			}
		}

		private void SaveWindowSettings()
		{
			if (this._suppressWindowSettingsSave)
			{
				return;
			}

			var settings = Settings.Default;
			if (this.WindowState == FormWindowState.Normal)
			{
				settings.MainFormWidth = this.Width;
				settings.MainFormHeight = this.Height;
			}
			settings.SplitterDistance = this._split.SplitterDistance;
			settings.Save();
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
			if (this._suppressLayoutSave || string.IsNullOrEmpty(this._currentColumnLayoutKey))
			{
				return;
			}

			this._layoutSaveTimer.Stop();
			this._layoutSaveTimer.Start();
		}

		private void SaveColumnLayout()
		{
			if (this._suppressLayoutSave || string.IsNullOrEmpty(this._currentColumnLayoutKey) || this._grid.Columns.Count == 0)
			{
				return;
			}

			var layout = DataGridViewLayoutStore.Serialize(this._grid);
			var settings = Settings.Default;
			if (this._currentColumnLayoutKey == LayoutLogTable)
			{
				settings.LogTableColumnLayout = layout;
			}
			else if (this._currentColumnLayoutKey == LayoutLogStruct)
			{
				settings.LogStructColumnLayout = layout;
			}
			settings.Save();
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

		private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (!this._isLogTableView || e.RowIndex < 0)
			{
				return;
			}

			var entry = this.GetLogEntryAtRow(e.RowIndex);
			if (!LogQueryHelper.HasViewableQuery(entry))
			{
				return;
			}

			this.ViewLogQuery(entry);
		}

		private void Grid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right || e.RowIndex < 0)
			{
				return;
			}

			if (!this._grid.Rows[e.RowIndex].Selected)
			{
				this._grid.ClearSelection();
				this._grid.Rows[e.RowIndex].Selected = true;
				if (e.ColumnIndex >= 0)
				{
					this._grid.CurrentCell = this._grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
				}
			}
		}

		private void GridContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this._viewQueryMenuItem.Enabled = this.CanViewSelectedLogQuery();
			if (!this._isLogTableView)
			{
				e.Cancel = true;
			}
		}

		private bool CanViewSelectedLogQuery()
		{
			return this._isLogTableView && LogQueryHelper.HasViewableQuery(this.GetSelectedLogEntry());
		}

		private void ViewSelectedLogQuery()
		{
			var entry = this.GetSelectedLogEntry();
			if (!LogQueryHelper.HasViewableQuery(entry))
			{
				return;
			}

			this.ViewLogQuery(entry);
		}

		private void ViewLogQuery(LogEntry entry)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				var query = this._repository.GetLogQuery(entry.LogId);
				LogQueryViewer.OpenQuery(this, entry.LogId, query);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "View query", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private LogEntry GetSelectedLogEntry()
		{
			if (!this._isLogTableView || this._grid.CurrentRow == null)
			{
				return null;
			}

			return this.GetLogEntryAtRow(this._grid.CurrentRow.Index);
		}

		private LogEntry GetLogEntryAtRow(int rowIndex)
		{
			if (!this._isLogTableView || rowIndex < 0 || rowIndex >= this._grid.Rows.Count)
			{
				return null;
			}

			return this._grid.Rows[rowIndex].DataBoundItem as LogEntry;
		}

		private void RefreshCurrentView(bool preserveTree)
		{
			this.ClearFindState();

			HashSet<string> expandedKeys = null;
			string selectedKey = null;
			if (preserveTree)
			{
				expandedKeys = this.CaptureExpandedNodeKeys();
				selectedKey = (this._tree.SelectedNode?.Tag as TreeNodeTag)?.NodeKey;
			}

			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.RefreshLoadedTreeNodes(this._tree.Nodes);
				this.RefreshTreeNodeColors(this._tree.Nodes);
				if (preserveTree && expandedKeys != null)
				{
					this.RestoreExpandedNodeKeys(this._tree.Nodes, expandedKeys);
					this.SelectNodeByKey(this._tree.Nodes, selectedKey);
				}

				if (this._tree.SelectedNode != null)
				{
					this.LoadGridForNode(this._tree.SelectedNode);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Refresh failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void RefreshSelectedView()
		{
			var selectedNode = this._tree.SelectedNode;
			if (selectedNode == null)
			{
				return;
			}

			this.ClearFindState();
			var selectedKey = (selectedNode.Tag as TreeNodeTag)?.NodeKey;
			var expandedKeys = this.CaptureExpandedNodeKeysInSubtree(selectedNode);

			try
			{
				this._loadingTreeNode = true;
				this.Cursor = Cursors.WaitCursor;
				this.RefreshTreeNodeAndDescendants(selectedNode);

				for (var ancestor = selectedNode.Parent; ancestor != null; ancestor = ancestor.Parent)
				{
					this.UpdateStatusFromChildren(ancestor);
				}

				this.RestoreExpandedNodeKeysInSubtree(selectedNode, expandedKeys);

				if (!string.IsNullOrEmpty(selectedKey))
				{
					this.SelectNodeByKey(this._tree.Nodes, selectedKey);
				}

				var nodeToLoad = this._tree.SelectedNode ?? selectedNode;
				this.LoadGridForNode(nodeToLoad);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Refresh selected failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				this.Cursor = Cursors.Default;
				this._loadingTreeNode = false;
			}
		}

		private void RefreshTreeNodeAndDescendants(TreeNode node)
		{
			var tag = node.Tag as TreeNodeTag;
			if (tag == null)
			{
				return;
			}

			this.RefreshTreeNodeData(node, tag);

			if (!tag.ChildrenLoaded)
			{
				return;
			}

			if (tag.ItemType == LogTreeItemType.Root)
			{
				foreach (TreeNode child in node.Nodes)
				{
					this.RefreshTreeNodeAndDescendants(child);
				}
				return;
			}

			node.Nodes.Clear();
			tag.ChildrenLoaded = false;
			this.LoadChildren(node, tag);
			tag.ChildrenLoaded = true;

			foreach (TreeNode child in node.Nodes)
			{
				this.RefreshTreeNodeAndDescendants(child);
			}
		}

		private void RefreshTreeNodeData(TreeNode node, TreeNodeTag tag)
		{
			switch (tag.ItemType)
			{
				case LogTreeItemType.LogSession when tag.LogStructId.HasValue:
					node.Text = this._repository.GetLogSessionLabel(tag.LogStructId.Value, tag.RootDurationSeconds);
					LogNodeStatusHelper.ApplyToNode(
						node,
						this._repository.GetLogSessionStatus(tag.LogStructId.Value),
						this._repository.GetLogSessionPictureIndex(tag.LogStructId.Value));
					break;
				case LogTreeItemType.Period:
					LogNodeStatusHelper.ApplyToNode(
						node,
						this._repository.GetPeriodStatus(tag.PeriodBegin, tag.PeriodEnd));
					break;
				case LogTreeItemType.Orphan:
					LogNodeStatusHelper.ApplyToNode(
						node,
						this._repository.GetOrphanStatus(tag.PeriodBegin, tag.PeriodEnd));
					break;
			}
		}

		private HashSet<string> CaptureExpandedNodeKeysInSubtree(TreeNode root)
		{
			var keys = new HashSet<string>(StringComparer.Ordinal);
			this.CaptureExpandedNodeKeysInSubtree(root, keys);
			return keys;
		}

		private void CaptureExpandedNodeKeysInSubtree(TreeNode node, ISet<string> keys)
		{
			if (node.Tag is TreeNodeTag tag && node.IsExpanded)
			{
				keys.Add(tag.NodeKey);
			}

			foreach (TreeNode child in node.Nodes)
			{
				this.CaptureExpandedNodeKeysInSubtree(child, keys);
			}
		}

		private void RestoreExpandedNodeKeysInSubtree(TreeNode node, ISet<string> keys)
		{
			if (node.Tag is TreeNodeTag tag && keys.Contains(tag.NodeKey))
			{
				node.Expand();
			}

			foreach (TreeNode child in node.Nodes)
			{
				this.RestoreExpandedNodeKeysInSubtree(child, keys);
			}
		}

		private void RefreshLoadedTreeNodes(TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				var tag = node.Tag as TreeNodeTag;
				if (tag == null)
				{
					continue;
				}

				if (tag.ItemType == LogTreeItemType.LogSession && tag.LogStructId.HasValue)
				{
					node.Text = this._repository.GetLogSessionLabel(tag.LogStructId.Value, tag.RootDurationSeconds);
					LogNodeStatusHelper.ApplyToNode(
						node,
						this._repository.GetLogSessionStatus(tag.LogStructId.Value),
						this._repository.GetLogSessionPictureIndex(tag.LogStructId.Value));
				}
				else if (tag.ItemType == LogTreeItemType.Period)
				{
					LogNodeStatusHelper.ApplyToNode(
						node,
						this._repository.GetPeriodStatus(tag.PeriodBegin, tag.PeriodEnd));
				}

				if (tag.ChildrenLoaded)
				{
					if (tag.ItemType == LogTreeItemType.Root)
					{
						this.RefreshLoadedTreeNodes(node.Nodes);
					}
					else
					{
						node.Nodes.Clear();
						tag.ChildrenLoaded = false;
						this.LoadChildren(node, tag);
						tag.ChildrenLoaded = true;
						this.RefreshLoadedTreeNodes(node.Nodes);
					}
				}
			}
		}

		private HashSet<string> CaptureExpandedNodeKeys()
		{
			var keys = new HashSet<string>(StringComparer.Ordinal);
			this.CaptureExpandedNodeKeys(this._tree.Nodes, keys);
			return keys;
		}

		private void CaptureExpandedNodeKeys(TreeNodeCollection nodes, ISet<string> keys)
		{
			foreach (TreeNode node in nodes)
			{
				var tag = node.Tag as TreeNodeTag;
				if (tag != null && node.IsExpanded)
				{
					keys.Add(tag.NodeKey);
				}
				this.CaptureExpandedNodeKeys(node.Nodes, keys);
			}
		}

		private void RestoreExpandedNodeKeys(TreeNodeCollection nodes, ISet<string> keys)
		{
			foreach (TreeNode node in nodes)
			{
				var tag = node.Tag as TreeNodeTag;
				if (tag != null && keys.Contains(tag.NodeKey))
				{
					node.Expand();
				}
				this.RestoreExpandedNodeKeys(node.Nodes, keys);
			}
		}

		private bool SelectNodeByKey(TreeNodeCollection nodes, string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return false;
			}

			foreach (TreeNode node in nodes)
			{
				var tag = node.Tag as TreeNodeTag;
				if (tag != null && tag.NodeKey == key)
				{
					this._tree.SelectedNode = node;
					return true;
				}
				if (this.SelectNodeByKey(node.Nodes, key))
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateStatusFromChildren(TreeNode node)
		{
			if (node == null)
			{
				return;
			}

			var tag = node.Tag as TreeNodeTag;
			if (tag != null && tag.ItemType == LogTreeItemType.LogSession)
			{
				// Each log_struct row has its own status icon, independent of child logs.
				this.UpdateStatusFromChildren(node.Parent);
				return;
			}

			var maxStatus = LogNodeStatus.Normal;
			foreach (TreeNode child in node.Nodes)
			{
				if (child.Tag is TreeNodeTag childTag)
				{
					maxStatus = LogNodeStatusHelper.Max(maxStatus, childTag.Status);
				}
			}

			LogNodeStatusHelper.ApplyToNode(node, maxStatus);
			this.UpdateStatusFromChildren(node.Parent);
		}

		private void RefreshTreeNodeColors(TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				this.RefreshTreeNodeColors(node.Nodes);
				var tag = node.Tag as TreeNodeTag;
				if (tag != null && tag.ItemType == LogTreeItemType.LogSession)
				{
					continue;
				}

				if (node.Nodes.Count > 0)
				{
					var maxStatus = LogNodeStatus.Normal;
					foreach (TreeNode child in node.Nodes)
					{
						if (child.Tag is TreeNodeTag childTag)
						{
							maxStatus = LogNodeStatusHelper.Max(maxStatus, childTag.Status);
						}
					}
					LogNodeStatusHelper.ApplyToNode(node, maxStatus);
				}
				else if (tag != null)
				{
					LogNodeStatusHelper.ApplyToNode(node, tag.Status);
				}
			}
		}

		private TreeNode CreateNode(string text, TreeNodeTag tag)
		{
			return new TreeNode(text) { Tag = tag };
		}

		private TreeNode CreatePlaceholderNode()
		{
			return new TreeNode("...");
		}

		private string EscapeSqlLiteral(string value)
		{
			return (value ?? string.Empty).Replace("'", "''");
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.F5)
			{
				this.RefreshCurrentView(preserveTree: true);
				return true;
			}

			if (keyData == Keys.F7)
			{
				this.RefreshSelectedView();
				return true;
			}

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
				var selectedNode = this._tree.SelectedNode;
				if (selectedNode != null)
				{
					Clipboard.SetText(selectedNode.Text);
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
