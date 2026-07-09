using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SequenceFrameViewer.Helpers;

public class NaturalSortComparer : IComparer<string>
{
    private static readonly Regex NumberRegex = new(@"(\d+)", RegexOptions.Compiled);

    public int Compare(string? x, string? y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        var xParts = NumberRegex.Split(x);
        var yParts = NumberRegex.Split(y);

        int maxParts = Math.Max(xParts.Length, yParts.Length);

        for (int i = 0; i < maxParts; i++)
        {
            if (i >= xParts.Length) return -1;
            if (i >= yParts.Length) return 1;

            int result;

            if (long.TryParse(xParts[i], out long xNum) && long.TryParse(yParts[i], out long yNum))
            {
                result = xNum.CompareTo(yNum);
            }
            else
            {
                result = string.Compare(xParts[i], yParts[i], StringComparison.OrdinalIgnoreCase);
            }

            if (result != 0)
                return result;
        }

        return 0;
    }
}
