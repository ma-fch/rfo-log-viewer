using RfoLogViewer.Data;
using RfoLogViewer.Properties;
using System;
using System.Windows.Forms;

namespace RfoLogViewer.Forms
{
    public partial class ConnectionForm : Form
    {
        public string Login => this._txtLogin.Text.Trim();
        public string Password => this._txtPassword.Text;
        public string DataSource => this._txtDataSource.Text.Trim();
        public long ContextId => long.TryParse(this._txtContextId.Text.Trim(), out var id) ? id : 0;
        public bool SavePasswordEnabled => this._chkSavePassword.Checked;

        public ConnectionForm()
        {
            this.InitializeComponent();
            this.Icon = AppIcon.Get();
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

        private void BtnOpenExcelLogFile_Click(object sender, EventArgs e)
        {
            this.OpenExcelLogFile();
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
    }
}
