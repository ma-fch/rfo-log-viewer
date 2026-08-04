using RfoLogViewer.Data;
using RfoLogViewer.Forms;
using System;
using System.Text;
using System.Windows.Forms;

namespace RfoLogViewer
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			using (var connectionForm = new ConnectionForm())
			{
				while (true)
				{
					if (connectionForm.ShowDialog() != DialogResult.OK)
					{
						return;
					}
					if (!connectionForm.ValidateInput())
					{
						continue;
					}

					OracleLogRepository repository = null;
					try
					{
						repository = new OracleLogRepository(
							connectionForm.DataSource,
							connectionForm.Login,
							connectionForm.Password);

						var userId = repository.GetUserId(connectionForm.Login);
						repository.OpenContext(connectionForm.ContextId, userId);

						if (connectionForm.SaveAsDefaultConnectionEnabled)
						{
							ConnectionForm.SaveSettings(
								connectionForm.Login,
								connectionForm.Password,
								connectionForm.DataSource,
								connectionForm.ContextId,
								connectionForm.SavePasswordEnabled);
						}

						Application.Run(new MainForm(
							repository,
							userId,
							connectionForm.ContextId,
							connectionForm.SaveAsDefaultConnectionEnabled));
						break;
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
}
