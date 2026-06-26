using System;
using System.Drawing;
using System.Windows.Forms;
using RfoLogViewer.Properties;

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

        public static void ApplyToNode(TreeNode node, LogNodeStatus status, int pictureIndex = 0)
        {
            if (node == null)
            {
                return;
            }

            if (node.Tag is TreeNodeTag tag)
            {
                tag.Status = status;
                ApplyNodeIcon(node, tag, pictureIndex);
            }

            node.ForeColor = GetForeColor(status);
        }

        private static void ApplyNodeIcon(TreeNode node, TreeNodeTag tag, int pictureIndex)
        {
            int resolvedIndex;
            switch (tag.ItemType)
            {
                case LogTreeItemType.Period:
                    resolvedIndex = LogTreePictureIndex.Period;
                    break;
                case LogTreeItemType.Orphan:
                    resolvedIndex = LogTreePictureIndex.Log;
                    break;
                case LogTreeItemType.LogSession when tag.ShowTaskIcon:
                    if (pictureIndex > 0)
                    {
                        tag.PictureIndex = pictureIndex;
                    }
                    resolvedIndex = tag.PictureIndex;
                    break;
                default:
                    tag.PictureIndex = 0;
                    node.ImageIndex = -1;
                    node.SelectedImageIndex = -1;
                    return;
            }

            tag.PictureIndex = resolvedIndex;
            if (resolvedIndex > 0)
            {
                var imageIndex = LogTreeImageList.ToImageListIndex(resolvedIndex);
                node.ImageIndex = imageIndex;
                node.SelectedImageIndex = imageIndex;
            }
            else
            {
                node.ImageIndex = -1;
                node.SelectedImageIndex = -1;
            }
        }
    }
}
