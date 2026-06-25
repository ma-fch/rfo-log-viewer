using System.Collections.Generic;

namespace RfoLogViewer.Models
{
    public enum ExcelTreeItemType
    {
        Root,
        RootLogKey,
        LogSession
    }

    public sealed class ExcelTreeNodeTag
    {
        public ExcelTreeItemType ItemType { get; set; }
        public string RootLogKey { get; set; }
        public long? LogStructId { get; set; }
        public HashSet<long> FilterLogStructIds { get; set; }
        public LogNodeStatus Status { get; set; }
    }
}
