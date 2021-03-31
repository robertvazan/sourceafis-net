// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using Serilog;

namespace SourceAFIS.Cmd
{
    class ScalarAccuracy
    {
        public double Eer;
        public double Fmr100;
        public double Fmr1K;
        public double Fmr10K;
        public static ScalarAccuracy Of(SampleDataset dataset)
        {
            return PersistentCache.Get("accuracy", dataset.Path, () =>
            {
                var trio = QuantileFunction.Of(dataset);
                var accuracy = new ScalarAccuracy();
                accuracy.Fmr100 = QuantileFunction.FnmrAtFmr(trio.Matching, trio.Nonmatching, 1.0 / 100);
                accuracy.Fmr1K = QuantileFunction.FnmrAtFmr(trio.Matching, trio.Nonmatching, 1.0 / 1_000);
                accuracy.Fmr10K = QuantileFunction.FnmrAtFmr(trio.Matching, trio.Nonmatching, 1.0 / 10_000);
                accuracy.Eer = QuantileFunction.Eer(trio.Matching, trio.Nonmatching);
                return accuracy;
            });
        }
        public static ScalarAccuracy Average()
        {
            var average = new ScalarAccuracy();
            int count = SampleDataset.All.Count;
            foreach (var dataset in SampleDataset.All)
            {
                var accuracy = Of(dataset);
                average.Eer += accuracy.Eer / count;
                average.Fmr100 += accuracy.Fmr100 / count;
                average.Fmr1K += accuracy.Fmr1K / count;
                average.Fmr10K += accuracy.Fmr10K / count;
            }
            return average;
        }
        public static void Report(string name, ScalarAccuracy accuracy)
        {
            Log.Information("Accuracy/{0}: EER = {1:F2}%, FMR100 = {2:F2}%, FMR1K = {3:F2}%, FMR10K = {4:F2}%", name,
                100 * accuracy.Eer, 100 * accuracy.Fmr100, 100 * accuracy.Fmr1K, 100 * accuracy.Fmr10K);
        }
        public static void Report()
        {
            foreach (var dataset in SampleDataset.All)
                Report(dataset.Name, Of(dataset));
            Report("average", Average());
        }
    }
}
