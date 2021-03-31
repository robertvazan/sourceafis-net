// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;

namespace SourceAFIS.Cmd
{
    class QuantileFunction
    {
        public class Trio
        {
            public double[] Matching;
            public double[] Nonmatching;
            public double[] Selfmatching;
        }
        public static Trio Of(SampleDataset dataset)
        {
            var fingerprints = dataset.Fingerprints;
            var scores = ScoreTable.Of(dataset);
            var matching = new List<double>();
            var nonmatching = new List<double>();
            var selfmatching = new List<double>();
            foreach (var probe in fingerprints)
            {
                foreach (var candidate in fingerprints)
                {
                    var score = scores[probe.Id][candidate.Id];
                    if (probe.Id == candidate.Id)
                        selfmatching.Add(score);
                    else if (probe.Finger.Id == candidate.Finger.Id)
                        matching.Add(score);
                    else
                        nonmatching.Add(score);
                }
            }
            matching.Sort();
            nonmatching.Sort();
            selfmatching.Sort();
            var trio = new Trio();
            trio.Matching = matching.ToArray();
            trio.Nonmatching = nonmatching.ToArray();
            trio.Selfmatching = selfmatching.ToArray();
            return trio;
        }
        public static double Read(double[] function, double probability)
        {
            double index = probability * (function.Length - 1);
            int indexLow = (int)index;
            int indexHigh = indexLow + 1;
            if (indexHigh >= function.Length)
                return function[indexLow];
            double shareHigh = index - indexLow;
            double shareLow = 1 - shareHigh;
            return function[indexLow] * shareLow + function[indexHigh] * shareHigh;
        }
        public static double Cdf(double[] function, double threshold)
        {
            double min = 0, max = 1;
            for (int i = 0; i < 30; ++i)
            {
                double probability = (min + max) / 2;
                double score = Read(function, probability);
                if (score >= threshold)
                    max = probability;
                else
                    min = probability;
            }
            return (min + max) / 2;
        }
        public static double FnmrAtFmr(double[] matching, double[] nonmatching, double fmr)
        {
            double threshold = Read(nonmatching, 1 - fmr);
            return Cdf(matching, threshold);
        }
        public static double Eer(double[] matching, double[] nonmatching)
        {
            double min = Read(nonmatching, 0), max = Read(nonmatching, 1);
            for (int i = 0; i < 30; ++i)
            {
                double threshold = (min + max) / 2;
                double fmr = 1 - Cdf(nonmatching, threshold);
                double fnmr = Cdf(matching, threshold);
                if (fnmr >= fmr)
                    max = threshold;
                else
                    min = threshold;
            }
            return Cdf(matching, (min + max) / 2);
        }
    }
}
