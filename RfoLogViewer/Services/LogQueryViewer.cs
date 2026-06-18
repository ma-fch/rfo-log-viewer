using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RfoLogViewer.Services
{
    internal static class LogQueryViewer
    {
        public static void OpenQuery(IWin32Window owner, long logId, string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show(
                    owner,
                    $"No query found in fdw_log_query for log_id {logId}.",
                    "View query",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var filePath = Path.Combine(Path.GetTempPath(), $"log_{logId}.txt");
            File.WriteAllText(filePath, query, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            Process.Start(new ProcessStartInfo(filePath)
            {
                UseShellExecute = true
            });
        }
    }
}
