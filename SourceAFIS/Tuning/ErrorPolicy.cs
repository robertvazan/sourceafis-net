using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Tuning
{
    public sealed class ErrorPolicy
    {
        public delegate float Evaluate(ROCPoint point);

        public static readonly Evaluate EER = delegate(ROCPoint point) { return point.FAR - point.FRR; };

        public static readonly Evaluate PreferFAR = delegate(ROCPoint point) { return point.FAR - Calc.Sq(point.FRR); };

        public static readonly Evaluate FAR100 = delegate(ROCPoint point) { return point.FAR - 0.01f; };

        public static readonly Evaluate ZeroFAR = delegate(ROCPoint point) { return point.FAR; };
    }
}
