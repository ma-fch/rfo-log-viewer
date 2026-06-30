using RfoLogViewer.Models;
using RfoLogViewer.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RfoLogViewer.Forms
{
    public partial class ContextSelectionForm : Form
    {
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

        public ContextSelectionForm()
        {
            this.InitializeComponent();
            if (!this.DesignMode)
            {
                this.Icon = AppIcon.Get();
            }
        }

        public ContextSelectionForm(IList<ContextEntry> contexts, long currentContextId) : this()
        {
            this._grid.DataSource = contexts.ToList();
            this.SelectContext(currentContextId);
        }

        private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.ConfirmSelection();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.ConfirmSelection();
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.ConfirmSelection();
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
