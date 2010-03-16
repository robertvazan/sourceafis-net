using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Tuning.Errors;

namespace SourceAFIS.Tuning.Reports
{
    public sealed class ErrorStatistics
    {
        public ErrorRate EER = new ErrorRate();
        public ErrorRate PreferFAR = new ErrorRate();
        public ErrorRate FAR100 = new ErrorRate();
        public ErrorRate ZeroFAR = new ErrorRate();

        public void Compute(ROCCurve ROC)
        {
            EER.Compute(ROC, ErrorPolicy.EER);
            PreferFAR.Compute(ROC, ErrorPolicy.PreferFAR);
            FAR100.Compute(ROC, ErrorPolicy.FAR100);
            ZeroFAR.Compute(ROC, ErrorPolicy.ZeroFAR);
        }

        public void Average(List<ErrorStatistics> partial)
        {
            EER.Average(partial.ConvertAll<ErrorRate>(delegate(ErrorStatistics item) { return item.EER; }));
            PreferFAR.Average(partial.ConvertAll<ErrorRate>(delegate(ErrorStatistics item) { return item.PreferFAR; }));
            FAR100.Average(partial.ConvertAll<ErrorRate>(delegate(ErrorStatistics item) { return item.FAR100; }));
            ZeroFAR.Average(partial.ConvertAll<ErrorRate>(delegate(ErrorStatistics item) { return item.ZeroFAR; }));
        }
    }
}
