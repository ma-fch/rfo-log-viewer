using System;

namespace RfoLogViewer.Models
{
    internal static class LogQueryHelper
    {
        public const string ClobQueryFunction = "PACK_LOG.LOG_CLOB_QUERY";

        public static bool HasViewableQuery(LogEntry entry)
        {
            return entry != null
                && string.Equals(entry.Function, ClobQueryFunction, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetQueryFileName(long logId)
        {
            return $"log_{logId}.txt";
        }
    }
}
