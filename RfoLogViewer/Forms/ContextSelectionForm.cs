using RfoLogViewer.Models;
using RfoLogViewer.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RfoLogViewer.Forms
{
	internal sealed class ContextSelectionForm : Form
	{
		private readonly DataGridView _grid;
		private readonly Button _btnOk;
		private readonly Button _btnCancel;

		public long? SelectedContextId
		{
			get
			{
				if (this._grid.CurrentRow?.DataBoundItem is ContextEntry entry)
				{
					return entry.ContextId;
				}
				return null;
			}
		}

		public ContextSelectionForm(IList<ContextEntry> contexts, long currentContextId)
		{
			this.Text = "Select Context";
			this.Icon = AppIcon.Get();
			this.FormBorderStyle = FormBorderStyle.Sizable;
			this.MaximizeBox = true;
			this.MinimizeBox = false;
			this.StartPosition = FormStartPosition.CenterParent;
			this.ClientSize = new Size(900, 480);
			this.Font = new Font("Segoe UI", 9F);
			this.MinimumSize = new Size(640, 360);

			this._grid = new DataGridView
			{
				Dock = DockStyle.Fill,
				ReadOnly = true,
				AllowUserToAddRows = false,
				AllowUserToDeleteRows = false,
				AllowUserToResizeRows = false,
				RowHeadersVisible = false,
				SelectionMode = DataGridViewSelectionMode.FullRowSelect,
				MultiSelect = false,
				AutoGenerateColumns = false,
				AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
			};
			this._grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				DataPropertyName = nameof(ContextEntry.ContextId),
				HeaderText = "Context Id",
				FillWeight = 80
			});
			this._grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				DataPropertyName = nameof(ContextEntry.ReportingDate),
				HeaderText = "Reporting Date",
				FillWeight = 100,
				DefaultCellStyle = { Format = "yyyy-MM-dd" }
			});
			this._grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				DataPropertyName = nameof(ContextEntry.Workspace),
				HeaderText = "Workspace",
				FillWeight = 120
			});
			this._grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				DataPropertyName = nameof(ContextEntry.Description),
				HeaderText = "Description",
				FillWeight = 200
			});
			this._grid.DataSource = contexts.ToList();
			this._grid.CellDoubleClick += (_, __) => this.ConfirmSelection();
			this._grid.KeyDown += (_, e) =>
			{
				if (e.KeyCode == Keys.Enter)
				{
					e.SuppressKeyPress = true;
					this.ConfirmSelection();
				}
			};

			var buttons = new FlowLayoutPanel
			{
				Dock = DockStyle.Bottom,
				Height = 44,
				Padding = new Padding(12, 8, 12, 8),
				FlowDirection = FlowDirection.RightToLeft
			};
			this._btnOk = new Button { Text = "OK", DialogResult = DialogResult.None, Width = 90 };
			this._btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };
			this._btnOk.Click += (_, __) => this.ConfirmSelection();
			buttons.Controls.Add(this._btnOk);
			buttons.Controls.Add(this._btnCancel);

			var layout = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
			layout.Controls.Add(this._grid);
			layout.Controls.Add(buttons);

			this.Controls.Add(layout);
			this.AcceptButton = this._btnOk;
			this.CancelButton = this._btnCancel;

			this.SelectContext(currentContextId);
		}

		private void SelectContext(long contextId)
		{
			foreach (DataGridViewRow row in this._grid.Rows)
			{
				if (row.DataBoundItem is ContextEntry entry && entry.ContextId == contextId)
				{
					row.Selected = true;
					this._grid.CurrentCell = row.Cells[0];
					this._grid.FirstDisplayedScrollingRowIndex = row.Index;
					return;
				}
			}

			if (this._grid.Rows.Count > 0)
			{
				this._grid.Rows[0].Selected = true;
				this._grid.CurrentCell = this._grid.Rows[0].Cells[0];
			}
		}

		private void ConfirmSelection()
		{
			if (!this.SelectedContextId.HasValue)
			{
				MessageBox.Show(this, "Please select a context.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
