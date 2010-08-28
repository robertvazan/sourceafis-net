using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Tuning.Errors
{
    public sealed class ErrorPolicy
    {
        public delegate float Evaluate(ROCPoint point);

        public static readonly Evaluate EER = point => point.FAR - point.FRR;

        public static readonly Evaluate PreferFAR = point => point.FAR - Calc.Sq(point.FRR);

        public static readonly Evaluate FAR100 = point => point.FAR - 0.01f;

        public static readonly Evaluate ZeroFAR = point => point.FAR;
    }
}
