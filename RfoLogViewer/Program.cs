using System;
using System.Windows.Forms;
using RfoLogViewer.Data;
using RfoLogViewer.Forms;

namespace RfoLogViewer
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var connectionForm = new ConnectionForm())
            {
                if (connectionForm.ShowDialog() != DialogResult.OK || !connectionForm.ValidateInput())
                {
                    return;
                }

                ConnectionForm.SaveSettings(
                    connectionForm.Login,
                    connectionForm.Password,
                    connectionForm.DataSource,
                    connectionForm.ContextId,
                    connectionForm.SavePasswordEnabled);

                OracleLogRepository repository = null;
                try
                {
                    repository = new OracleLogRepository(
                        connectionForm.DataSource,
                        connectionForm.Login,
                        connectionForm.Password);

                    var userId = repository.GetUserId(connectionForm.Login);
                    repository.OpenContext(connectionForm.ContextId, userId);

                    Application.Run(new MainForm(repository));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        "Connection failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    repository?.Dispose();
                }
            }
        }
    }
}
