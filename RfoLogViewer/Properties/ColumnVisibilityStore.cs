using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RfoLogViewer.Properties
{
    internal static class ColumnVisibilityStore
    {
        public static Dictionary<string, bool> Load(string saved, IEnumerable<string> columnKeys)
        {
            var result = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in columnKeys)
            {
                result[key] = true;
            }

            if (string.IsNullOrWhiteSpace(saved))
            {
                return result;
            }

            foreach (var entry in saved.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = entry.Split('=');
                if (parts.Length != 2)
                {
                    continue;
                }

                var key = parts[0].Trim();
                if (!result.ContainsKey(key))
                {
                    continue;
                }

                result[key] = parts[1].Trim() == "1";
            }

            return result;
        }

        public static string Save(IReadOnlyDictionary<string, bool> visibility)
        {
            return string.Join(";", visibility.Select(kv => $"{kv.Key}={(kv.Value ? 1 : 0)}"));
        }

        public static void ApplyToGrid(DataGridView grid, IReadOnlyDictionary<string, bool> visibility)
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                var key = column.DataPropertyName ?? column.Name;
                if (visibility.TryGetValue(key, out bool visible))
                {
                    column.Visible = visible;
                }
            }
        }
    }
}
