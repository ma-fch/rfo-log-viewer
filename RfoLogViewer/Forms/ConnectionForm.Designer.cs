namespace RfoLogViewer.Forms
{
    partial class ConnectionForm
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
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.lblLogin = new System.Windows.Forms.Label();
            this._txtLogin = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this._txtPassword = new System.Windows.Forms.TextBox();
            this.lblDataSource = new System.Windows.Forms.Label();
            this._txtDataSource = new System.Windows.Forms.TextBox();
            this.lblContextId = new System.Windows.Forms.Label();
            this._txtContextId = new System.Windows.Forms.TextBox();
            this.optionsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._chkSavePassword = new System.Windows.Forms.CheckBox();
            this._chkSaveAsDefaultConnection = new System.Windows.Forms.CheckBox();
            this.hint = new System.Windows.Forms.Label();
            this.footer = new System.Windows.Forms.TableLayoutPanel();
            this.footerSpacer = new System.Windows.Forms.Panel();
            this.excelPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._btnOpenExcelLogFile = new System.Windows.Forms.Button();
            this.okCancelPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._btnOk = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this.layout.SuspendLayout();
            this.optionsPanel.SuspendLayout();
            this.footer.SuspendLayout();
            this.excelPanel.SuspendLayout();
            this.okCancelPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // layout
            //
            this.layout.ColumnCount = 2;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.lblLogin, 0, 0);
            this.layout.Controls.Add(this._txtLogin, 1, 0);
            this.layout.Controls.Add(this.lblPassword, 0, 1);
            this.layout.Controls.Add(this._txtPassword, 1, 1);
            this.layout.Controls.Add(this.lblDataSource, 0, 2);
            this.layout.Controls.Add(this._txtDataSource, 1, 2);
            this.layout.Controls.Add(this.lblContextId, 0, 3);
            this.layout.Controls.Add(this._txtContextId, 1, 3);
            this.layout.Controls.Add(this.optionsPanel, 1, 4);
            this.layout.Controls.Add(this.hint, 0, 5);
            this.layout.Controls.Add(this.footer, 0, 6);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Name = "layout";
            this.layout.Padding = new System.Windows.Forms.Padding(12);
            this.layout.RowCount = 7;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.SetColumnSpan(this.hint, 2);
            this.layout.SetColumnSpan(this.footer, 2);
            this.layout.Size = new System.Drawing.Size(520, 280);
            this.layout.TabIndex = 0;
            //
            // lblLogin
            //
            this.lblLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLogin.Location = new System.Drawing.Point(15, 12);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(144, 34);
            this.lblLogin.TabIndex = 0;
            this.lblLogin.Text = "Login";
            this.lblLogin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // _txtLogin
            //
            this._txtLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._txtLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this._txtLogin.Location = new System.Drawing.Point(165, 15);
            this._txtLogin.Name = "_txtLogin";
            this._txtLogin.Size = new System.Drawing.Size(340, 23);
            this._txtLogin.TabIndex = 1;
            //
            // lblPassword
            //
            this.lblPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPassword.Location = new System.Drawing.Point(15, 46);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(144, 34);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Password";
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // _txtPassword
            //
            this._txtPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this._txtPassword.Location = new System.Drawing.Point(165, 49);
            this._txtPassword.Name = "_txtPassword";
            this._txtPassword.Size = new System.Drawing.Size(340, 23);
            this._txtPassword.TabIndex = 3;
            this._txtPassword.UseSystemPasswordChar = true;
            //
            // lblDataSource
            //
            this.lblDataSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataSource.Location = new System.Drawing.Point(15, 80);
            this.lblDataSource.Name = "lblDataSource";
            this.lblDataSource.Size = new System.Drawing.Size(144, 34);
            this.lblDataSource.TabIndex = 4;
            this.lblDataSource.Text = "DB connection id";
            this.lblDataSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // _txtDataSource
            //
            this._txtDataSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this._txtDataSource.Location = new System.Drawing.Point(165, 83);
            this._txtDataSource.Name = "_txtDataSource";
            this._txtDataSource.Size = new System.Drawing.Size(340, 23);
            this._txtDataSource.TabIndex = 5;
            //
            // lblContextId
            //
            this.lblContextId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblContextId.Location = new System.Drawing.Point(15, 114);
            this.lblContextId.Name = "lblContextId";
            this.lblContextId.Size = new System.Drawing.Size(144, 34);
            this.lblContextId.TabIndex = 6;
            this.lblContextId.Text = "Context ID";
            this.lblContextId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // _txtContextId
            //
            this._txtContextId.Dock = System.Windows.Forms.DockStyle.Fill;
            this._txtContextId.Location = new System.Drawing.Point(165, 117);
            this._txtContextId.Name = "_txtContextId";
            this._txtContextId.Size = new System.Drawing.Size(340, 23);
            this._txtContextId.TabIndex = 7;
            //
            // optionsPanel
            //
            this.optionsPanel.AutoSize = true;
            this.optionsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.optionsPanel.Controls.Add(this._chkSavePassword);
            this.optionsPanel.Controls.Add(this._chkSaveAsDefaultConnection);
            this.optionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optionsPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.optionsPanel.Location = new System.Drawing.Point(165, 151);
            this.optionsPanel.Name = "optionsPanel";
            this.optionsPanel.Size = new System.Drawing.Size(340, 28);
            this.optionsPanel.TabIndex = 8;
            this.optionsPanel.WrapContents = false;
            //
            // _chkSavePassword
            //
            this._chkSavePassword.AutoSize = true;
            this._chkSavePassword.Location = new System.Drawing.Point(0, 3);
            this._chkSavePassword.Margin = new System.Windows.Forms.Padding(0, 3, 16, 0);
            this._chkSavePassword.Name = "_chkSavePassword";
            this._chkSavePassword.Size = new System.Drawing.Size(104, 19);
            this._chkSavePassword.TabIndex = 0;
            this._chkSavePassword.Text = "Save password";
            this._chkSavePassword.UseVisualStyleBackColor = true;
            //
            // _chkSaveAsDefaultConnection
            //
            this._chkSaveAsDefaultConnection.AutoSize = true;
            this._chkSaveAsDefaultConnection.Checked = true;
            this._chkSaveAsDefaultConnection.CheckState = System.Windows.Forms.CheckState.Checked;
            this._chkSaveAsDefaultConnection.Location = new System.Drawing.Point(120, 3);
            this._chkSaveAsDefaultConnection.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this._chkSaveAsDefaultConnection.Name = "_chkSaveAsDefaultConnection";
            this._chkSaveAsDefaultConnection.Size = new System.Drawing.Size(158, 19);
            this._chkSaveAsDefaultConnection.TabIndex = 1;
            this._chkSaveAsDefaultConnection.Text = "Save as default connection";
            this._chkSaveAsDefaultConnection.UseVisualStyleBackColor = true;
            //
            // hint
            //
            this.hint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.hint.Location = new System.Drawing.Point(15, 185);
            this.hint.Name = "hint";
            this.hint.Size = new System.Drawing.Size(490, 34);
            this.hint.TabIndex = 9;
            this.hint.Text = "DB connection id is the TNS name (alias) or data source\r\n(like adb-xxx.ad.regbanking.net/db1_pdb1).";
            //
            // footer
            //
            this.footer.ColumnCount = 3;
            this.footer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.footer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.footer.Controls.Add(this.footerSpacer, 0, 0);
            this.footer.Controls.Add(this.excelPanel, 1, 0);
            this.footer.Controls.Add(this.okCancelPanel, 2, 0);
            this.footer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footer.Location = new System.Drawing.Point(15, 222);
            this.footer.Name = "footer";
            this.footer.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.footer.RowCount = 1;
            this.footer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.footer.Size = new System.Drawing.Size(490, 43);
            this.footer.TabIndex = 10;
            //
            // footerSpacer
            //
            this.footerSpacer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footerSpacer.Location = new System.Drawing.Point(3, 6);
            this.footerSpacer.Name = "footerSpacer";
            this.footerSpacer.Size = new System.Drawing.Size(1, 34);
            this.footerSpacer.TabIndex = 0;
            //
            // excelPanel
            //
            this.excelPanel.AutoSize = true;
            this.excelPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.excelPanel.Controls.Add(this._btnOpenExcelLogFile);
            this.excelPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.excelPanel.Location = new System.Drawing.Point(10, 6);
            this.excelPanel.Margin = new System.Windows.Forms.Padding(0, 0, 64, 0);
            this.excelPanel.Name = "excelPanel";
            this.excelPanel.Size = new System.Drawing.Size(156, 29);
            this.excelPanel.TabIndex = 1;
            this.excelPanel.WrapContents = false;
            //
            // _btnOpenExcelLogFile
            //
            this._btnOpenExcelLogFile.Location = new System.Drawing.Point(3, 3);
            this._btnOpenExcelLogFile.Margin = new System.Windows.Forms.Padding(0);
            this._btnOpenExcelLogFile.Name = "_btnOpenExcelLogFile";
            this._btnOpenExcelLogFile.Size = new System.Drawing.Size(150, 23);
            this._btnOpenExcelLogFile.TabIndex = 0;
            this._btnOpenExcelLogFile.Text = "Open Excel File...";
            this._btnOpenExcelLogFile.UseVisualStyleBackColor = true;
            this._btnOpenExcelLogFile.Click += new System.EventHandler(this.BtnOpenExcelLogFile_Click);
            //
            // okCancelPanel
            //
            this.okCancelPanel.AutoSize = true;
            this.okCancelPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.okCancelPanel.Controls.Add(this._btnOk);
            this.okCancelPanel.Controls.Add(this._btnCancel);
            this.okCancelPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.okCancelPanel.Location = new System.Drawing.Point(233, 6);
            this.okCancelPanel.Margin = new System.Windows.Forms.Padding(0);
            this.okCancelPanel.Name = "okCancelPanel";
            this.okCancelPanel.Size = new System.Drawing.Size(192, 29);
            this.okCancelPanel.TabIndex = 2;
            this.okCancelPanel.WrapContents = false;
            //
            // _btnOk
            //
            this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnOk.Location = new System.Drawing.Point(3, 3);
            this._btnOk.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this._btnOk.Name = "_btnOk";
            this._btnOk.Size = new System.Drawing.Size(90, 23);
            this._btnOk.TabIndex = 0;
            this._btnOk.Text = "OK";
            this._btnOk.UseVisualStyleBackColor = true;
            //
            // _btnCancel
            //
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Location = new System.Drawing.Point(99, 3);
            this._btnCancel.Margin = new System.Windows.Forms.Padding(0);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(90, 23);
            this._btnCancel.TabIndex = 1;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            //
            // ConnectionForm
            //
            this.AcceptButton = this._btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(520, 280);
            this.Controls.Add(this.layout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RFo Log Viewer - Connection";
            this.layout.ResumeLayout(false);
            this.layout.PerformLayout();
            this.optionsPanel.ResumeLayout(false);
            this.optionsPanel.PerformLayout();
            this.footer.ResumeLayout(false);
            this.footer.PerformLayout();
            this.excelPanel.ResumeLayout(false);
            this.okCancelPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.TextBox _txtLogin;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox _txtPassword;
        private System.Windows.Forms.Label lblDataSource;
        private System.Windows.Forms.TextBox _txtDataSource;
        private System.Windows.Forms.Label lblContextId;
        private System.Windows.Forms.TextBox _txtContextId;
        private System.Windows.Forms.FlowLayoutPanel optionsPanel;
        private System.Windows.Forms.CheckBox _chkSavePassword;
        private System.Windows.Forms.CheckBox _chkSaveAsDefaultConnection;
        private System.Windows.Forms.Label hint;
        private System.Windows.Forms.TableLayoutPanel footer;
        private System.Windows.Forms.Panel footerSpacer;
        private System.Windows.Forms.FlowLayoutPanel excelPanel;
        private System.Windows.Forms.Button _btnOpenExcelLogFile;
        private System.Windows.Forms.FlowLayoutPanel okCancelPanel;
        private System.Windows.Forms.Button _btnOk;
        private System.Windows.Forms.Button _btnCancel;
    }
}
