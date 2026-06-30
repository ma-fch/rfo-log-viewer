using System;
using System.Windows.Forms;

namespace RfoLogViewer.Properties
{
    internal static class SplitContainerLayoutStore
    {
        public static int GetClampedDistance(SplitContainer split, int savedDistance)
        {
            if (split == null || savedDistance <= 0 || split.Width <= 0)
            {
                return -1;
            }

            var available = split.Width - split.SplitterWidth;
            if (available < split.Panel1MinSize + split.Panel2MinSize)
            {
                return -1;
            }

            var max = available - split.Panel2MinSize;
            return Math.Max(split.Panel1MinSize, Math.Min(savedDistance, max));
        }
    }
}
