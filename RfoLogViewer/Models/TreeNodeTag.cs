using System;

namespace RfoLogViewer.Models
{
    /// <summary>
    /// Tag stored on each TreeNode; mirrors item_type / item_sub_type / item_params from w_log.xml.
    /// </summary>
    public sealed class TreeNodeTag
    {
        public LogTreeItemType ItemType { get; set; }
        public DateTime PeriodBegin { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string RootLogKey { get; set; }
        public long? LogStructId { get; set; }
        public long? ParentLogStructId { get; set; }
        public decimal? RootDurationSeconds { get; set; }
        public bool ChildrenLoaded { get; set; }
        public LogNodeStatus Status { get; set; }
        public int PictureIndex { get; set; }
        public bool ShowTaskIcon { get; set; }

        public string NodeKey
        {
            get
            {
                switch (this.ItemType)
                {
                    case LogTreeItemType.Root:
                        return "ROOT";
                    case LogTreeItemType.Period:
                        return $"PERIOD|{this.PeriodBegin:yyyyMMdd}|{this.PeriodEnd:yyyyMMdd}";
                    case LogTreeItemType.RootLogKey:
                        return $"LK|{this.PeriodBegin:yyyyMMdd}|{this.PeriodEnd:yyyyMMdd}|{this.RootLogKey}";
                    case LogTreeItemType.Orphan:
                        return $"ORPHAN|{this.PeriodBegin:yyyyMMdd}|{this.PeriodEnd:yyyyMMdd}";
                    case LogTreeItemType.LogSession:
                        return $"L|{this.LogStructId}|{this.ParentLogStructId}|{this.RootDurationSeconds}";
                    default:
                        return Guid.NewGuid().ToString();
                }
            }
        }
    }
}
