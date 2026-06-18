using System;

namespace RfoLogViewer.Models
{
    public sealed class LogEntry
    {
        public long LogId { get; set; }
        public DateTime? DateTime { get; set; }
        public string Message { get; set; }
        public string Function { get; set; }
        public string Step { get; set; }
        public string Parameters { get; set; }
        public long? RootTaskId { get; set; }
        public long? TaskId { get; set; }
        public string Machine { get; set; }
        public string Cooperator { get; set; }
        public string ProcessDesc { get; set; }
        public string IsMySessionId { get; set; }
        public string LogType { get; set; }
        public string LogKey { get; set; }
        public string TechFunc { get; set; }
        public string ProductName { get; set; }
        public int? Severity { get; set; }
        public int? Depth { get; set; }
        public long? NumMsg { get; set; }
        public long? SessionId { get; set; }
        public long? LogStructId { get; set; }
        public string RootLogKey { get; set; }
        public string PartitionKey { get; set; }
    }
}
