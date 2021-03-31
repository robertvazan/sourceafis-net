// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Linq;

namespace SourceAFIS.Cmd
{
    class ScoreTable
    {
        public static double[][] Of(SampleDataset dataset)
        {
            return PersistentCache.Get("scores", dataset.Path, () =>
            {
                var fingerprints = dataset.Fingerprints;
                var templates = fingerprints.Select(fp => NativeTemplate.Of(fp)).ToList();
                var scores = new double[fingerprints.Count][];
                foreach (var probe in fingerprints)
                {
                    var matcher = new FingerprintMatcher(templates[probe.Id]);
                    scores[probe.Id] = new double[fingerprints.Count];
                    foreach (var candidate in fingerprints)
                        scores[probe.Id][candidate.Id] = matcher.Match(templates[candidate.Id]);
                }
                return scores;
            });
        }
    }
}
