using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Tuning.Errors;

namespace SourceAFIS.Tuning.Reports
{
    public sealed class MultiFingerStatistics
    {
        public ErrorStatistics Simple = new ErrorStatistics();
        public ErrorStatistics Take1Of2 = new ErrorStatistics();
        public ErrorStatistics Take2Of3 = new ErrorStatistics();
        public ErrorStatistics Take2Of4 = new ErrorStatistics();
        public ErrorStatistics Take3Of5 = new ErrorStatistics();

        public void Compute(ScoreTable table)
        {
            ROCCurve SimpleROC = new ROCCurve();
            SimpleROC.Compute(table);
            Simple.Compute(SimpleROC);
            
            Take1Of2.Compute(GetROC(table, MultiFingerPolicy.Take1Of2));
            Take2Of3.Compute(GetROC(table, MultiFingerPolicy.Take2Of3));
            Take2Of4.Compute(GetROC(table, MultiFingerPolicy.Take2Of4));
            Take3Of5.Compute(GetROC(table, MultiFingerPolicy.Take3Of5));
        }

        public void Average(List<MultiFingerStatistics> partial)
        {
            Simple.Average(partial.ConvertAll<ErrorStatistics>(delegate(MultiFingerStatistics item) { return item.Simple; }));
            Take1Of2.Average(partial.ConvertAll<ErrorStatistics>(delegate(MultiFingerStatistics item) { return item.Take1Of2; }));
            Take2Of3.Average(partial.ConvertAll<ErrorStatistics>(delegate(MultiFingerStatistics item) { return item.Take2Of3; }));
            Take2Of4.Average(partial.ConvertAll<ErrorStatistics>(delegate(MultiFingerStatistics item) { return item.Take2Of4; }));
            Take3Of5.Average(partial.ConvertAll<ErrorStatistics>(delegate(MultiFingerStatistics item) { return item.Take3Of5; }));
        }

        ROCCurve GetROC(ScoreTable table, MultiFingerPolicy policy)
        {
            ScoreTable multi = table.GetMultiFingerTable(policy);
            ROCCurve ROC = new ROCCurve();
            ROC.Compute(multi);
            return ROC;
        }
    }
}
