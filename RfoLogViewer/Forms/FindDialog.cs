using System;
using System.Windows.Forms;

namespace RfoLogViewer.Forms
{
    internal partial class FindDialog : Form
    {
        public string SearchText => this._txtSearch.Text;

        public FindDialog()
        {
            this.InitializeComponent();
        }

        public FindDialog(string initialText) : this()
        {
            this._txtSearch.Text = initialText ?? string.Empty;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this._txtSearch.Focus();
            this._txtSearch.SelectAll();
        }
    }
}
