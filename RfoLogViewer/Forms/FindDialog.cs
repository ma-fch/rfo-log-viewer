using System;
using System.Drawing;
using System.Windows.Forms;

namespace RfoLogViewer.Forms
{
    internal sealed class FindDialog : Form
    {
        private readonly TextBox _txtSearch;
        private readonly Button _btnOk;
        private readonly Button _btnCancel;

        public string SearchText => this._txtSearch.Text;

        public FindDialog(string initialText)
        {
            this.Text = "Find";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(420, 110);
            this.Font = new Font("Segoe UI", 9F);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(12)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            this._txtSearch = new TextBox { Dock = DockStyle.Fill, Text = initialText ?? string.Empty };
            layout.Controls.Add(new Label
            {
                Text = "Find what:",
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            }, 0, 0);
            layout.Controls.Add(this._txtSearch, 1, 0);

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft
            };
            this._btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
            this._btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };
            buttons.Controls.Add(this._btnOk);
            buttons.Controls.Add(this._btnCancel);
            layout.Controls.Add(buttons, 0, 1);
            layout.SetColumnSpan(buttons, 2);

            this.Controls.Add(layout);
            this.AcceptButton = this._btnOk;
            this.CancelButton = this._btnCancel;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this._txtSearch.Focus();
            this._txtSearch.SelectAll();
        }
    }
}
