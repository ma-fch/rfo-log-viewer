using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RfoLogViewer.Properties
{
    internal static class AppIcon
    {
        private static Icon _icon;

        public static Icon Get()
        {
            if (_icon != null)
            {
                return _icon;
            }

            using (var stream = typeof(AppIcon).Assembly.GetManifestResourceStream("RfoLogViewer.duck.ico"))
            {
                if (stream != null)
                {
                    using (var temp = new Icon(stream))
                    {
                        _icon = (Icon)temp.Clone();
                    }

                    return _icon;
                }
            }

            var iconPath = Path.Combine(Application.StartupPath, "duck.ico");
            if (File.Exists(iconPath))
            {
                _icon = new Icon(iconPath);
                return _icon;
            }

            _icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            return _icon;
        }
    }
}
