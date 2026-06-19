using System;

namespace RfoLogViewer.Models
{
    public sealed class ContextEntry
    {
        public long ContextId { get; set; }
        public DateTime ReportingDate { get; set; }
        public string Workspace { get; set; }
        public string Description { get; set; }
    }
}
