using System;
using System.Collections.Generic;
using System.Text;
#if !COMPACT_FRAMEWORK
using System.Drawing;
#endif
using SourceAFIS.Dummy;

namespace SourceAFIS.General
{
    public static class Neighborhood
    {
        public static readonly Point[] EdgeNeighbors = new Point[] {
            new Point(0, -1),
            new Point(-1, 0),
            new Point(1, 0),
            new Point(0, 1)
        };

        public static readonly Point[] CornerNeighbors = new Point[] {
            new Point(-1, -1),
            new Point(0, -1),
            new Point(1, -1),
            new Point(-1, 0),
            new Point(1, 0),
            new Point(-1, 1),
            new Point(0, 1),
            new Point(1, 1)
        };
    }
}
