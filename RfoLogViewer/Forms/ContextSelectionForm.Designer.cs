namespace RfoLogViewer.Forms
{
    partial class ContextSelectionForm
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
            this.layout = new System.Windows.Forms.Panel();
            this._grid = new System.Windows.Forms.DataGridView();
            this.colContextId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReportingDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWorkspace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttons = new System.Windows.Forms.FlowLayoutPanel();
            this._btnOk = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this.layout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            this.buttons.SuspendLayout();
            this.SuspendLayout();
            //
            // layout
            //
            this.layout.Controls.Add(this._grid);
            this.layout.Controls.Add(this.buttons);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Name = "layout";
            this.layout.Padding = new System.Windows.Forms.Padding(12);
            this.layout.Size = new System.Drawing.Size(900, 480);
            this.layout.TabIndex = 0;
            //
            // _grid
            //
            this._grid.AllowUserToAddRows = false;
            this._grid.AllowUserToDeleteRows = false;
            this._grid.AllowUserToResizeRows = false;
            this._grid.AutoGenerateColumns = false;
            this._grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colContextId,
            this.colReportingDate,
            this.colWorkspace,
            this.colDescription});
            this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grid.Location = new System.Drawing.Point(12, 12);
            this._grid.MultiSelect = false;
            this._grid.Name = "_grid";
            this._grid.ReadOnly = true;
            this._grid.RowHeadersVisible = false;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._grid.Size = new System.Drawing.Size(876, 412);
            this._grid.TabIndex = 0;
            this._grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Grid_CellDoubleClick);
            this._grid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Grid_KeyDown);
            //
            // colContextId
            //
            this.colContextId.DataPropertyName = "ContextId";
            this.colContextId.FillWeight = 80F;
            this.colContextId.HeaderText = "Context Id";
            this.colContextId.Name = "colContextId";
            this.colContextId.ReadOnly = true;
            //
            // colReportingDate
            //
            this.colReportingDate.DataPropertyName = "ReportingDate";
            this.colReportingDate.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.colReportingDate.DefaultCellStyle.Format = "yyyy-MM-dd";
            this.colReportingDate.FillWeight = 100F;
            this.colReportingDate.HeaderText = "Reporting Date";
            this.colReportingDate.Name = "colReportingDate";
            this.colReportingDate.ReadOnly = true;
            //
            // colWorkspace
            //
            this.colWorkspace.DataPropertyName = "Workspace";
            this.colWorkspace.FillWeight = 120F;
            this.colWorkspace.HeaderText = "Workspace";
            this.colWorkspace.Name = "colWorkspace";
            this.colWorkspace.ReadOnly = true;
            //
            // colDescription
            //
            this.colDescription.DataPropertyName = "Description";
            this.colDescription.FillWeight = 200F;
            this.colDescription.HeaderText = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            //
            // buttons
            //
            this.buttons.Controls.Add(this._btnOk);
            this.buttons.Controls.Add(this._btnCancel);
            this.buttons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttons.Location = new System.Drawing.Point(12, 424);
            this.buttons.Name = "buttons";
            this.buttons.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.buttons.Size = new System.Drawing.Size(876, 44);
            this.buttons.TabIndex = 1;
            //
            // _btnOk
            //
            this._btnOk.Location = new System.Drawing.Point(783, 11);
            this._btnOk.Name = "_btnOk";
            this._btnOk.Size = new System.Drawing.Size(90, 23);
            this._btnOk.TabIndex = 0;
            this._btnOk.Text = "OK";
            this._btnOk.UseVisualStyleBackColor = true;
            this._btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            //
            // _btnCancel
            //
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Location = new System.Drawing.Point(687, 11);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(90, 23);
            this._btnCancel.TabIndex = 1;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            //
            // ContextSelectionForm
            //
            this.AcceptButton = this._btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(900, 480);
            this.Controls.Add(this.layout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 360);
            this.Name = "ContextSelectionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Context";
            this.layout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            this.buttons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel layout;
        private System.Windows.Forms.DataGridView _grid;
        private System.Windows.Forms.DataGridViewTextBoxColumn colContextId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReportingDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWorkspace;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.FlowLayoutPanel buttons;
        private System.Windows.Forms.Button _btnOk;
        private System.Windows.Forms.Button _btnCancel;
    }
}
