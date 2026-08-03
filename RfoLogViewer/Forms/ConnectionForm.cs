using RfoLogViewer.Data;
using RfoLogViewer.Properties;
using RfoLogViewer.Services;
using System;
using System.Collections.Generic;
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
        public bool SaveAsDefaultConnectionEnabled => this._chkSaveAsDefaultConnection.Checked;

        private readonly Dictionary<string, ConnectionProfile> _savedConnectionProfiles = new Dictionary<string, ConnectionProfile>(StringComparer.OrdinalIgnoreCase);

        private bool _loadingConnectionProfiles;

        public ConnectionForm()
        {
            this.InitializeComponent();
            if (!this.DesignMode)
            {
                this.Icon = AppIcon.Get();
                this.LoadSavedSettings();
                this.EnableExcelFileDragDrop(this);
            }
        }

        public static void SaveSettings(string login, string password, string dataSource, long contextId, bool savePassword)
        {
            var settings = Settings.Default;
            var profile = ConnectionProfileStore.CreateProfile(
                dataSource,
                login,
                password,
                contextId,
                savePassword);
            var profiles = ConnectionProfileStore.Load(settings.ConnectionProfiles);
            settings.ConnectionProfiles = ConnectionProfileStore.Save(ConnectionProfileStore.Upsert(profiles, profile));
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
            var profiles = ConnectionProfileStore.Load(settings.ConnectionProfiles);
            ConnectionProfile currentProfile = null;
            if (!string.IsNullOrWhiteSpace(settings.LastDataSource))
            {
                currentProfile = ConnectionProfileStore.CreateProfile(
                    settings.LastDataSource,
                    settings.LastLogin,
                    settings.SavePassword ? settings.LastPassword : string.Empty,
                    this.ParseContextId(settings.LastContextId),
                    settings.SavePassword);
            }
            var orderedProfiles = ConnectionProfileStore.Upsert(profiles, currentProfile);

            this._loadingConnectionProfiles = true;
            try
            {
                this._savedConnectionProfiles.Clear();
                this._txtDataSource.BeginUpdate();
                this._txtDataSource.Items.Clear();

                foreach (var profile in orderedProfiles)
                {
                    if ((profile == null) || string.IsNullOrWhiteSpace(profile.DataSource) || this._savedConnectionProfiles.ContainsKey(profile.DataSource))
                    {
                        continue;
                    }

                    this._savedConnectionProfiles.Add(profile.DataSource, profile);
                    this._txtDataSource.Items.Add(profile.DataSource);
                }

                ConnectionProfile selectedProfile = null;
                if ((currentProfile != null) && !string.IsNullOrWhiteSpace(currentProfile.DataSource))
                {
                    this._savedConnectionProfiles.TryGetValue(currentProfile.DataSource, out selectedProfile);
                }

                if (selectedProfile == null)
                {
                    foreach (var profile in orderedProfiles)
                    {
                        selectedProfile = profile;
                        break;
                    }
                }

                if (selectedProfile != null)
                {
                    this._txtDataSource.SelectedItem = selectedProfile.DataSource;
                    this.ApplyConnectionProfile(selectedProfile);
                }
                else
                {
                    this._txtLogin.Text = settings.LastLogin ?? string.Empty;
                    this._txtDataSource.Text = settings.LastDataSource ?? string.Empty;
                    this._txtContextId.Text = settings.LastContextId ?? string.Empty;
                    this._chkSavePassword.Checked = settings.SavePassword;
                    this._txtPassword.Text = settings.SavePassword ? settings.LastPassword ?? string.Empty : string.Empty;
                }
            }
            finally
            {
                this._txtDataSource.EndUpdate();
                this._loadingConnectionProfiles = false;
            }
        }

        private void ApplyConnectionProfile(ConnectionProfile profile)
        {
            if (profile == null)
            {
                return;
            }

            this._txtLogin.Text = profile.Login ?? string.Empty;
            this._txtPassword.Text = profile.SavePassword ? profile.Password ?? string.Empty : string.Empty;
            this._txtContextId.Text = profile.ContextId > 0 ? profile.ContextId.ToString() : string.Empty;
            this._chkSavePassword.Checked = profile.SavePassword;
        }

        private long ParseContextId(string value)
        {
            return long.TryParse((value ?? string.Empty).Trim(), out var contextId) ? contextId : 0;
        }

        private void TxtDataSource_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (this._loadingConnectionProfiles)
            {
                return;
            }

            var dataSource = this._txtDataSource.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(dataSource))
            {
                dataSource = this._txtDataSource.Text.Trim();
            }

            if (string.IsNullOrWhiteSpace(dataSource))
            {
                return;
            }

            if (this._savedConnectionProfiles.TryGetValue(dataSource, out var profile))
            {
                this.ApplyConnectionProfile(profile);
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

                ExcelLogFileOpener.OpenViewer(this, dialog.FileName);
            }
        }

        private void ConnectionForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = ExcelLogFilePaths.CanAcceptExcelDrag(e.Data)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        private bool _excelDropInProgress;

        private void ConnectionForm_DragDrop(object sender, DragEventArgs e)
        {
            if (this._excelDropInProgress)
            {
                return;
            }

            if (!ExcelLogFilePaths.TryGetExcelFileFromDrag(e.Data, out var filePath))
            {
                MessageBox.Show(this,
                    "Could not resolve the dropped Excel file path. Drop the file from Windows Explorer, or use Open Excel File...",
                    "Read Excel log file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this._excelDropInProgress = true;
            try
            {
                ExcelLogFileOpener.OpenViewer(this, filePath);
            }
            finally
            {
                this.BeginInvoke(new Action(() => this._excelDropInProgress = false));
            }
        }

        private void EnableExcelFileDragDrop(Control control)
        {
            control.AllowDrop = true;
            control.DragEnter += this.ConnectionForm_DragEnter;
            control.DragDrop += this.ConnectionForm_DragDrop;

            foreach (Control child in control.Controls)
            {
                this.EnableExcelFileDragDrop(child);
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
