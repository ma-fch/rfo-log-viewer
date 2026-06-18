using System.Linq;
using System.Windows.Forms;

namespace RfoLogViewer.Properties
{
    internal static class DataGridViewLayoutStore
    {
        public static string Serialize(DataGridView grid)
        {
            if (grid?.Columns == null || grid.Columns.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(";",
                grid.Columns.Cast<DataGridViewColumn>()
                    .OrderBy(column => column.DisplayIndex)
                    .Select(column => $"{GetColumnKey(column)}|{column.Width}|{column.DisplayIndex}"));
        }

        public static void Apply(DataGridView grid, string layout)
        {
            if (grid == null || string.IsNullOrWhiteSpace(layout))
            {
                return;
            }

            foreach (var entry in layout.Split(';'))
            {
                var parts = entry.Split('|');
                if (parts.Length != 3)
                {
                    continue;
                }

                var column = FindColumn(grid, parts[0]);
                if (column == null)
                {
                    continue;
                }

                if (int.TryParse(parts[1], out var width) && width >= column.MinimumWidth)
                {
                    column.Width = width;
                }

                if (int.TryParse(parts[2], out var displayIndex))
                {
                    column.DisplayIndex = displayIndex;
                }
            }
        }

        private static DataGridViewColumn FindColumn(DataGridView grid, string key)
        {
            return grid.Columns.Cast<DataGridViewColumn>()
                .FirstOrDefault(column => GetColumnKey(column) == key);
        }

        private static string GetColumnKey(DataGridViewColumn column)
        {
            if (!string.IsNullOrEmpty(column.DataPropertyName))
            {
                return column.DataPropertyName;
            }

            if (!string.IsNullOrEmpty(column.Name))
            {
                return column.Name;
            }

            return column.HeaderText ?? string.Empty;
        }
    }
}
