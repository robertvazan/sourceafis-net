using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Meta;

namespace SourceAFIS.Extraction.Filters
{
    public sealed class InnerMask
    {
        [DpiAdjusted]
        [Parameter(Lower = 0, Upper = 50)]
        public int MinBorderDistance = 15;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        void ShrinkBy(BinaryMap temporary, BinaryMap inner, int amount)
        {
            temporary.Clear();
            temporary.Copy(inner, new RectangleC(amount, 0, inner.Width - amount, inner.Height), new Point(0, 0));
            temporary.And(inner, new RectangleC(0, 0, inner.Width - amount, inner.Height), new Point(amount, 0));
            temporary.And(inner, new RectangleC(0, amount, inner.Width, inner.Height - amount), new Point(0, 0));
            temporary.And(inner, new RectangleC(0, 0, inner.Width, inner.Height - amount), new Point(0, amount));
            inner.Copy(temporary);
        }

        public BinaryMap Compute(BinaryMap outer)
        {
            BinaryMap inner = new BinaryMap(outer.Size);
            inner.Copy(outer, new RectangleC(1, 1, outer.Width - 2, outer.Height - 2), new Point(1, 1));
            BinaryMap temporary = new BinaryMap(outer.Size);
            if (MinBorderDistance >= 1)
                ShrinkBy(temporary, inner, 1);
            int total = 1;
            for (int step = 1; total + step <= MinBorderDistance; step *= 2)
            {
                ShrinkBy(temporary, inner, step);
                total += step;
            }
            if (total < MinBorderDistance)
                ShrinkBy(temporary, inner, MinBorderDistance - total);
            Logger.Log(inner);
            return inner;
        }
    }
}
