using System;
using System.IO;
using System.Windows.Forms;
using RfoLogViewer.Data;
using RfoLogViewer.Forms;

namespace RfoLogViewer.Services
{
    internal static class ExcelLogFileOpener
    {
        public static void OpenViewer(IWin32Window owner, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show(owner, "No Excel file was selected.", "Read Excel log file",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var fullPath = ExcelLogFilePaths.ResolveForOpen(filePath);
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                MessageBox.Show(owner, $"The Excel file could not be resolved:{Environment.NewLine}{filePath}",
                    "Read Excel log file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (owner is Control control)
                {
                    control.Cursor = Cursors.WaitCursor;
                }

                var entries = ExcelLogReader.Load(fullPath);
                if (entries.Count == 0)
                {
                    MessageBox.Show(owner,
                        $"No log rows found in the selected file:{Environment.NewLine}{fullPath}",
                        "Read Excel log file", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!ExcelLogReader.LooksLikeRfoLogExport(entries))
                {
                    MessageBox.Show(owner,
                        "The file was opened but does not look like an RFo log export. " +
                        $"No log columns were recognized in:{Environment.NewLine}{fullPath}",
                        "Read Excel log file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var rootNode = ExcelLogTreeBuilder.Build(entries);
                var viewer = new ExcelLogViewerForm(fullPath, entries, rootNode);
                viewer.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, ex.Message, "Read Excel log file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (owner is Control control)
                {
                    control.Cursor = Cursors.Default;
                }
            }
        }
    }
}
