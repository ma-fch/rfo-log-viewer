namespace RfoLogViewer.Forms
{
    partial class FindDialog
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
            this.lblFindWhat = new System.Windows.Forms.Label();
            this._txtSearch = new System.Windows.Forms.TextBox();
            this.buttons = new System.Windows.Forms.FlowLayoutPanel();
            this._btnOk = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this.layout.SuspendLayout();
            this.buttons.SuspendLayout();
            this.SuspendLayout();
            //
            // layout
            //
            this.layout.ColumnCount = 2;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.lblFindWhat, 0, 0);
            this.layout.Controls.Add(this._txtSearch, 1, 0);
            this.layout.Controls.Add(this.buttons, 0, 1);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Name = "layout";
            this.layout.Padding = new System.Windows.Forms.Padding(12);
            this.layout.RowCount = 2;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.SetColumnSpan(this.buttons, 2);
            this.layout.Size = new System.Drawing.Size(420, 110);
            this.layout.TabIndex = 0;
            //
            // lblFindWhat
            //
            this.lblFindWhat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFindWhat.Location = new System.Drawing.Point(15, 12);
            this.lblFindWhat.Name = "lblFindWhat";
            this.lblFindWhat.Size = new System.Drawing.Size(74, 34);
            this.lblFindWhat.TabIndex = 0;
            this.lblFindWhat.Text = "Find what:";
            this.lblFindWhat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // _txtSearch
            //
            this._txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this._txtSearch.Location = new System.Drawing.Point(95, 15);
            this._txtSearch.Name = "_txtSearch";
            this._txtSearch.Size = new System.Drawing.Size(310, 23);
            this._txtSearch.TabIndex = 1;
            //
            // buttons
            //
            this.buttons.Controls.Add(this._btnOk);
            this.buttons.Controls.Add(this._btnCancel);
            this.buttons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttons.Location = new System.Drawing.Point(15, 49);
            this.buttons.Name = "buttons";
            this.buttons.Size = new System.Drawing.Size(390, 48);
            this.buttons.TabIndex = 2;
            //
            // _btnOk
            //
            this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnOk.Location = new System.Drawing.Point(297, 3);
            this._btnOk.Name = "_btnOk";
            this._btnOk.Size = new System.Drawing.Size(90, 23);
            this._btnOk.TabIndex = 0;
            this._btnOk.Text = "OK";
            this._btnOk.UseVisualStyleBackColor = true;
            //
            // _btnCancel
            //
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Location = new System.Drawing.Point(201, 3);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(90, 23);
            this._btnCancel.TabIndex = 1;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            //
            // FindDialog
            //
            this.AcceptButton = this._btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(420, 110);
            this.Controls.Add(this.layout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find";
            this.layout.ResumeLayout(false);
            this.layout.PerformLayout();
            this.buttons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Label lblFindWhat;
        private System.Windows.Forms.TextBox _txtSearch;
        private System.Windows.Forms.FlowLayoutPanel buttons;
        private System.Windows.Forms.Button _btnOk;
        private System.Windows.Forms.Button _btnCancel;
    }
}
