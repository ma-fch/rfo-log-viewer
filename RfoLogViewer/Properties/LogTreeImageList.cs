using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace RfoLogViewer.Properties
{
    internal static class LogTreeImageList
    {
        private static readonly string ResourcePrefix =
            typeof(Program).Namespace + ".Resources.TreeIcons.";

        private static ImageList _images;

        public static ImageList Get()
        {
            if (_images != null)
            {
                return _images;
            }

            _images = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(16, 16)
            };

            // Index 0 is unused; legacy picture_index values are 1-based.
            _images.Images.Add("_", CreateBlank());
            _images.Images.Add("log", LoadEmbeddedIcon("console.ico"));
            _images.Images.Add("period", LoadEmbeddedIcon("calendar.ico"));
            _images.Images.Add("root_log_key", LoadEmbeddedIcon("gears.ico"));
            _images.Images.Add("session_ok", LoadEmbeddedIcon("gear_ok.ico"));
            _images.Images.Add("session_error", LoadEmbeddedIcon("gear_error.ico"));
            _images.Images.Add("session_running", LoadEmbeddedIcon("gear_refresh.ico"));
            _images.Images.Add("session_warning", LoadEmbeddedIcon("gear_warning.ico"));
            _images.Images.Add("session_stopped", LoadEmbeddedIcon("gear_stop.ico"));

            return _images;
        }

        public static int ToImageListIndex(int pictureIndex)
        {
            return pictureIndex > 0 ? pictureIndex : 0;
        }

        private static Image LoadEmbeddedIcon(string fileName)
        {
            var resourceName = ResourcePrefix + fileName;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("Missing embedded tree icon resource: " + resourceName);
                }

                using (var icon = new Icon(stream))
                {
                    return icon.ToBitmap();
                }
            }
        }

        private static Bitmap CreateBlank()
        {
            return new Bitmap(16, 16);
        }
    }
}
