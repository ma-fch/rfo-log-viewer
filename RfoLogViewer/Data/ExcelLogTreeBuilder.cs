using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RfoLogViewer.Models;

namespace RfoLogViewer.Data
{
    public static class ExcelLogTreeBuilder
    {
        private const string LogBeginFunction = "LOG_BEGIN";

        public static TreeNode Build(IList<LogEntry> entries)
        {
            var rootTag = new ExcelTreeNodeTag { ItemType = ExcelTreeItemType.Root };
            var root = new TreeNode("Log") { Tag = rootTag };

            var rootLogKeys = entries
                .Select(entry => entry.RootLogKey)
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (rootLogKeys.Count == 0)
            {
                rootLogKeys.Add("(no root log key)");
            }

            foreach (var rootLogKey in rootLogKeys)
            {
                var scopedEntries = string.Equals(rootLogKey, "(no root log key)", StringComparison.Ordinal)
                    ? entries.Where(entry => string.IsNullOrWhiteSpace(entry.RootLogKey)).ToList()
                    : entries.Where(entry => string.Equals(entry.RootLogKey, rootLogKey, StringComparison.OrdinalIgnoreCase)).ToList();

                var rootKeyTag = new ExcelTreeNodeTag
                {
                    ItemType = ExcelTreeItemType.RootLogKey,
                    RootLogKey = rootLogKey,
                    Status = ComputeStatus(scopedEntries, null)
                };
                var rootKeyNode = new TreeNode(rootLogKey) { Tag = rootKeyTag };
                LogNodeStatusHelper.ApplyToNode(rootKeyNode, rootKeyTag.Status);

                BuildLogBeginNodes(rootKeyNode, scopedEntries);
                LogNodeStatusHelper.ApplyToNode(rootKeyNode, MaxChildStatus(rootKeyNode));
                root.Nodes.Add(rootKeyNode);
            }

            return root;
        }

        private static void BuildLogBeginNodes(TreeNode rootLogKeyNode, IList<LogEntry> entries)
        {
            var beginEntries = entries
                .Where(entry => string.Equals(entry.Function, LogBeginFunction, StringComparison.OrdinalIgnoreCase))
                .OrderBy(entry => entry.DateTime ?? DateTime.MinValue)
                .ThenBy(entry => entry.LogId)
                .ToList();

            if (beginEntries.Count == 0)
            {
                return;
            }

            var stack = new Stack<(TreeNode Node, int Depth)>();
            foreach (var entry in beginEntries)
            {
                var depth = entry.Depth ?? 0;
                while (stack.Count > 0 && stack.Peek().Depth >= depth)
                {
                    stack.Pop();
                }

                var label = FormatNodeTitle(!string.IsNullOrWhiteSpace(entry.Message)
                    ? entry.Message
                    : entry.LogKey ?? "Session");

                var tag = new ExcelTreeNodeTag
                {
                    ItemType = ExcelTreeItemType.LogSession,
                    RootLogKey = entry.RootLogKey,
                    LogStructId = entry.LogStructId,
                    FilterLogStructIds = new HashSet<long>()
                };
                if (entry.LogStructId.HasValue)
                {
                    tag.FilterLogStructIds.Add(entry.LogStructId.Value);
                }

                var node = new TreeNode(label) { Tag = tag };
                if (stack.Count > 0)
                {
                    stack.Peek().Node.Nodes.Add(node);
                }
                else
                {
                    rootLogKeyNode.Nodes.Add(node);
                }

                stack.Push((node, depth));
            }

            foreach (TreeNode child in rootLogKeyNode.Nodes)
            {
                CollectDescendantStructIds(child);
            }

            AssignStatuses(rootLogKeyNode, entries);
        }

        private static string FormatNodeTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return title;
            }

            const string prefix = "Begin ";
            if (title.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return title.Substring(prefix.Length);
            }

            return title;
        }

        private static HashSet<long> CollectDescendantStructIds(TreeNode node)
        {
            if (node.Tag is ExcelTreeNodeTag tag && tag.ItemType == ExcelTreeItemType.LogSession)
            {
                var ids = tag.FilterLogStructIds ?? new HashSet<long>();
                foreach (TreeNode child in node.Nodes)
                {
                    foreach (var childId in CollectDescendantStructIds(child))
                    {
                        ids.Add(childId);
                    }
                }

                tag.FilterLogStructIds = ids;
                return ids;
            }

            var all = new HashSet<long>();
            foreach (TreeNode child in node.Nodes)
            {
                foreach (var id in CollectDescendantStructIds(child))
                {
                    all.Add(id);
                }
            }

            return all;
        }

        private static void AssignStatuses(TreeNode node, IList<LogEntry> entries)
        {
            foreach (TreeNode child in node.Nodes)
            {
                AssignStatuses(child, entries);
            }

            if (node.Tag is ExcelTreeNodeTag tag)
            {
                tag.Status = tag.ItemType == ExcelTreeItemType.LogSession
                    ? ComputeStatus(entries, tag.FilterLogStructIds)
                    : MaxChildStatus(node);
                LogNodeStatusHelper.ApplyToNode(node, tag.Status);
            }
        }

        private static LogNodeStatus MaxChildStatus(TreeNode node)
        {
            var status = LogNodeStatus.Normal;
            foreach (TreeNode child in node.Nodes)
            {
                if (child.Tag is ExcelTreeNodeTag childTag)
                {
                    status = LogNodeStatusHelper.Max(status, childTag.Status);
                }
            }

            return status;
        }

        private static LogNodeStatus ComputeStatus(IEnumerable<LogEntry> entries, ICollection<long> logStructIds)
        {
            var hasError = false;
            var hasWarning = false;
            foreach (var entry in entries)
            {
                if (logStructIds != null)
                {
                    if (!entry.LogStructId.HasValue || !logStructIds.Contains(entry.LogStructId.Value))
                    {
                        continue;
                    }
                }

                if (string.Equals(entry.LogType, "E", StringComparison.OrdinalIgnoreCase))
                {
                    hasError = true;
                    break;
                }

                if (string.Equals(entry.LogType, "W", StringComparison.OrdinalIgnoreCase))
                {
                    hasWarning = true;
                }
            }

            if (hasError)
            {
                return LogNodeStatus.Error;
            }

            if (hasWarning)
            {
                return LogNodeStatus.Warning;
            }

            return LogNodeStatus.Normal;
        }
    }
}
