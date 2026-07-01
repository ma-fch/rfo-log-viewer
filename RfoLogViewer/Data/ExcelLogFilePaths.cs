using System;
using System.IO;
using System.Windows.Forms;

namespace RfoLogViewer.Data
{
    internal static class ExcelLogFilePaths
    {
        public static bool IsExcelExtension(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            return extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".xls", StringComparison.OrdinalIgnoreCase);
        }

        public static string ResolveForOpen(string rawPath)
        {
            if (!TryNormalizeDraggedPath(rawPath, out var path))
            {
                return null;
            }

            if (Path.GetExtension(path).Equals(".lnk", StringComparison.OrdinalIgnoreCase))
            {
                path = ResolveShortcutTarget(path);
                if (string.IsNullOrWhiteSpace(path))
                {
                    return null;
                }
            }

            try
            {
                path = Path.GetFullPath(path);
            }
            catch (Exception)
            {
                return null;
            }

            return File.Exists(path) && IsExcelExtension(path) ? path : null;
        }

        public static bool TryNormalizeDraggedPath(string rawPath, out string fullPath)
        {
            fullPath = null;
            if (string.IsNullOrWhiteSpace(rawPath))
            {
                return false;
            }

            var path = rawPath.Trim().Trim('"', '\0', ' ');

            if (path.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    path = new Uri(path).LocalPath;
                }
                catch (UriFormatException)
                {
                    return false;
                }
            }

            if (!IsAbsoluteFilePath(path))
            {
                return false;
            }

            try
            {
                fullPath = Path.GetFullPath(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TryGetExcelFileFromDrag(IDataObject data, out string filePath)
        {
            filePath = null;
            if (data == null)
            {
                return false;
            }

            foreach (var rawPath in GetDroppedPaths(data))
            {
                var resolved = ResolveForOpen(rawPath);
                if (!string.IsNullOrWhiteSpace(resolved))
                {
                    filePath = resolved;
                    return true;
                }
            }

            return false;
        }

        public static bool CanAcceptExcelDrag(IDataObject data)
        {
            if (data == null)
            {
                return false;
            }

            foreach (var rawPath in GetDroppedPaths(data))
            {
                if (TryNormalizeDraggedPath(rawPath, out var normalized) && IsExcelExtension(normalized))
                {
                    return true;
                }

                if (Path.GetExtension(rawPath.Trim().Trim('"', '\0', ' '))
                    .Equals(".lnk", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string[] GetDroppedPaths(IDataObject data)
        {
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                var raw = data.GetData(DataFormats.FileDrop, autoConvert: true);
                if (raw is string[] paths)
                {
                    return paths;
                }

                if (raw is string singlePath)
                {
                    return new[] { singlePath };
                }
            }

            if (data.GetDataPresent(DataFormats.Text))
            {
                var text = Convert.ToString(data.GetData(DataFormats.Text))?.Trim();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return new[] { text };
                }
            }

            return Array.Empty<string>();
        }

        private static bool IsAbsoluteFilePath(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                return false;
            }

            if (path.StartsWith(@"\\", StringComparison.Ordinal))
            {
                return true;
            }

            if (path.Length >= 2 && path[1] == ':')
            {
                return path.Length > 2 && (path[2] == '\\' || path[2] == '/');
            }

            return true;
        }

        private static string ResolveShortcutTarget(string shortcutPath)
        {
            if (string.IsNullOrWhiteSpace(shortcutPath) || !File.Exists(shortcutPath))
            {
                return null;
            }

            try
            {
                var shellType = Type.GetTypeFromProgID("WScript.Shell");
                if (shellType == null)
                {
                    return null;
                }

                var shell = Activator.CreateInstance(shellType);
                var shortcut = shellType.InvokeMember(
                    "CreateShortcut",
                    System.Reflection.BindingFlags.InvokeMethod,
                    null,
                    shell,
                    new object[] { shortcutPath });
                if (shortcut == null)
                {
                    return null;
                }

                return Convert.ToString(shortcut.GetType().InvokeMember(
                    "TargetPath",
                    System.Reflection.BindingFlags.GetProperty,
                    null,
                    shortcut,
                    null));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
