using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using ExcelDataReader;
using RfoLogViewer.Models;

namespace RfoLogViewer.Data
{
    public static class ExcelLogReader
    {
        public static IList<LogEntry> Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path is required.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Excel log file was not found.", filePath);
            }

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                if (!reader.Read())
                {
                    return Array.Empty<LogEntry>();
                }

                var columnIndex = BuildColumnIndex(reader);
                var entries = new List<LogEntry>();
                while (reader.Read())
                {
                    entries.Add(MapRow(reader, columnIndex));
                }

                return entries;
            }
        }

        private static Dictionary<string, int> BuildColumnIndex(IExcelDataReader reader)
        {
            var columnIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = Convert.ToString(reader.GetValue(i))?.Trim();
                if (!string.IsNullOrEmpty(name) && !columnIndex.ContainsKey(name))
                {
                    columnIndex[name] = i;
                }
            }

            return columnIndex;
        }

        private static LogEntry MapRow(IExcelDataReader reader, IReadOnlyDictionary<string, int> columnIndex)
        {
            return new LogEntry
            {
                SessionId = GetNullableLong(reader, columnIndex, "SESSION_ID"),
                LogId = GetLong(reader, columnIndex, "LOG_ID"),
                LogKey = GetString(reader, columnIndex, "LOG_KEY"),
                LogType = GetString(reader, columnIndex, "LOG_TYPE"),
                Severity = GetNullableInt(reader, columnIndex, "SEVERITY"),
                TechFunc = GetString(reader, columnIndex, "TECH_FUNC"),
                Depth = GetNullableInt(reader, columnIndex, "DEPTH"),
                DateTime = GetNullableDateTime(reader, columnIndex, "DATETIME"),
                ProductName = GetString(reader, columnIndex, "PRODUCT_NAME"),
                Function = GetString(reader, columnIndex, "FUNCTION"),
                Step = GetString(reader, columnIndex, "STEP"),
                Message = GetString(reader, columnIndex, "MESSAGE"),
                Parameters = GetString(reader, columnIndex, "PARAMETERS"),
                NumMsg = GetNullableLong(reader, columnIndex, "NUMMSG"),
                ProcessDesc = GetString(reader, columnIndex, "PROCESS_DESC"),
                TaskId = GetNullableLong(reader, columnIndex, "TASK_ID"),
                IsMySessionId = GetString(reader, columnIndex, "IS_MY_SESSION_ID"),
                RootTaskId = GetNullableLong(reader, columnIndex, "ROOT_TASK_ID"),
                Machine = GetString(reader, columnIndex, "MACHINE"),
                Cooperator = GetString(reader, columnIndex, "COOPERATOR"),
                LogStructId = GetNullableLong(reader, columnIndex, "LOG_STRUCT_ID"),
                RootLogKey = GetString(reader, columnIndex, "ROOT_LOG_KEY"),
                PartitionKey = GetString(reader, columnIndex, "PARTITION_KEY")
            };
        }

        private static string GetString(IExcelDataReader reader, IReadOnlyDictionary<string, int> columnIndex, string column)
        {
            if (!columnIndex.TryGetValue(column, out var index) || reader.IsDBNull(index))
            {
                return null;
            }

            return Convert.ToString(reader.GetValue(index), CultureInfo.InvariantCulture);
        }

        private static long GetLong(IExcelDataReader reader, IReadOnlyDictionary<string, int> columnIndex, string column)
        {
            return GetNullableLong(reader, columnIndex, column) ?? 0L;
        }

        private static long? GetNullableLong(IExcelDataReader reader, IReadOnlyDictionary<string, int> columnIndex, string column)
        {
            if (!columnIndex.TryGetValue(column, out var index) || reader.IsDBNull(index))
            {
                return null;
            }

            return Convert.ToInt64(reader.GetValue(index), CultureInfo.InvariantCulture);
        }

        private static int? GetNullableInt(IExcelDataReader reader, IReadOnlyDictionary<string, int> columnIndex, string column)
        {
            if (!columnIndex.TryGetValue(column, out var index) || reader.IsDBNull(index))
            {
                return null;
            }

            return Convert.ToInt32(reader.GetValue(index), CultureInfo.InvariantCulture);
        }

        private static DateTime? GetNullableDateTime(IExcelDataReader reader, IReadOnlyDictionary<string, int> columnIndex, string column)
        {
            if (!columnIndex.TryGetValue(column, out var index) || reader.IsDBNull(index))
            {
                return null;
            }

            var value = reader.GetValue(index);
            if (value is DateTime dateTime)
            {
                return dateTime;
            }

            if (value is double numeric)
            {
                return DateTime.FromOADate(numeric);
            }

            if (DateTime.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                return parsed;
            }

            return null;
        }
    }
}
