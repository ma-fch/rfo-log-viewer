using System;
using System.Drawing;
using System.Windows.Forms;
using RfoLogViewer.Properties;

namespace RfoLogViewer.Forms
{
    public sealed class ConnectionForm : Form
    {
        private readonly TextBox _txtLogin;
        private readonly TextBox _txtPassword;
        private readonly TextBox _txtDataSource;
        private readonly TextBox _txtContextId;
        private readonly CheckBox _chkSavePassword;
        private readonly Button _btnOk;
        private readonly Button _btnCancel;

        public string Login => this._txtLogin.Text.Trim();
        public string Password => this._txtPassword.Text;
        public string DataSource => this._txtDataSource.Text.Trim();
        public long ContextId => long.TryParse(this._txtContextId.Text.Trim(), out var id) ? id : 0;
        public bool SavePasswordEnabled => this._chkSavePassword.Checked;

        public ConnectionForm()
        {
            this.Text = "RFo Log Viewer - Connection";
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(520, 280);
            this.Font = new Font("Segoe UI", 9F);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(12)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (var i = 0; i < 6; i++)
            {
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            }
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            this._txtLogin = CreateTextBox();
            this._txtPassword = CreateTextBox();
            this._txtPassword.UseSystemPasswordChar = true;
            this._txtDataSource = CreateTextBox();
            this._txtContextId = CreateTextBox();
            this._chkSavePassword = new CheckBox
            {
                Text = "Save password",
                AutoSize = true,
                Dock = DockStyle.Left
            };

            AddRow(layout, 0, "Login", this._txtLogin);
            AddRow(layout, 1, "Password", this._txtPassword);
            AddRow(layout, 2, "DB connection id", this._txtDataSource);
            AddRow(layout, 3, "Context ID", this._txtContextId);
            layout.Controls.Add(this._chkSavePassword, 1, 4);

            var hint = new Label
            {
                Text = "DB connection id is the TNS name (alias) or data source" + Environment.NewLine +
                    "(like adb-xxx.ad.regbanking.net/db1_pdb1).",
                AutoSize = false,
                Dock = DockStyle.Fill,
                ForeColor = SystemColors.GrayText
            };
            layout.Controls.Add(hint, 0, 5);
            layout.SetColumnSpan(hint, 2);

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft
            };
            this._btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
            this._btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };
            buttons.Controls.Add(this._btnOk);
            buttons.Controls.Add(this._btnCancel);
            layout.Controls.Add(buttons, 0, 6);
            layout.SetColumnSpan(buttons, 2);

            this.Controls.Add(layout);
            this.AcceptButton = this._btnOk;
            this.CancelButton = this._btnCancel;

            this.LoadSavedSettings();
        }

        public static void SaveSettings(string login, string password, string dataSource, long contextId, bool savePassword)
        {
            var settings = Settings.Default;
            settings.LastLogin = login ?? string.Empty;
            settings.LastDataSource = dataSource ?? string.Empty;
            settings.LastContextId = contextId > 0 ? contextId.ToString() : string.Empty;
            settings.SavePassword = savePassword;
            settings.LastPassword = savePassword ? password ?? string.Empty : string.Empty;
            settings.Save();
        }

        private void LoadSavedSettings()
        {
            var settings = Settings.Default;
            this._txtLogin.Text = settings.LastLogin ?? string.Empty;
            this._txtDataSource.Text = settings.LastDataSource ?? string.Empty;
            this._txtContextId.Text = settings.LastContextId ?? string.Empty;
            this._chkSavePassword.Checked = settings.SavePassword;
            if (settings.SavePassword)
            {
                this._txtPassword.Text = settings.LastPassword ?? string.Empty;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (string.IsNullOrWhiteSpace(this._txtLogin.Text))
            {
                this._txtLogin.Focus();
            }
        }

        public bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(this.Login))
            {
                MessageBox.Show(this, "Please enter a login.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this._txtLogin.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(this.DataSource))
            {
                MessageBox.Show(this, "Please enter a DB connection identifier.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this._txtDataSource.Focus();
                return false;
            }
            if (this.ContextId <= 0)
            {
                MessageBox.Show(this, "Please enter a valid numeric context ID.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this._txtContextId.Focus();
                return false;
            }
            return true;
        }

        private static TextBox CreateTextBox()
        {
            return new TextBox { Dock = DockStyle.Fill, Anchor = AnchorStyles.Left | AnchorStyles.Right };
        }

        private static void AddRow(TableLayoutPanel layout, int row, string label, Control control)
        {
            layout.Controls.Add(new Label
            {
                Text = label,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            }, 0, row);
            layout.Controls.Add(control, 1, row);
        }
    }
}
