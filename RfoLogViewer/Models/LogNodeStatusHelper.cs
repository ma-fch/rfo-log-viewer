using System;
using System.Drawing;
using System.Windows.Forms;

namespace RfoLogViewer.Models
{
    internal static class LogNodeStatusHelper
    {
        public static readonly Color WarningColor = Color.FromArgb(255, 128, 64);

        public static LogNodeStatus FromCounts(decimal errors, decimal warnings)
        {
            if (errors > 0)
            {
                return LogNodeStatus.Error;
            }
            if (warnings > 0)
            {
                return LogNodeStatus.Warning;
            }
            return LogNodeStatus.Normal;
        }

        public static LogNodeStatus FromCode(string code)
        {
            switch (code)
            {
                case "E":
                    return LogNodeStatus.Error;
                case "W":
                    return LogNodeStatus.Warning;
                default:
                    return LogNodeStatus.Normal;
            }
        }

        public static LogNodeStatus Max(LogNodeStatus left, LogNodeStatus right)
        {
            return (LogNodeStatus)Math.Max((int)left, (int)right);
        }

        public static Color GetForeColor(LogNodeStatus status)
        {
            switch (status)
            {
                case LogNodeStatus.Error:
                    return Color.Red;
                case LogNodeStatus.Warning:
                    return WarningColor;
                default:
                    return SystemColors.WindowText;
            }
        }

        public static void ApplyToNode(TreeNode node, LogNodeStatus status)
        {
            if (node?.Tag is TreeNodeTag tag)
            {
                tag.Status = status;
            }
            node.ForeColor = GetForeColor(status);
        }
    }
}
