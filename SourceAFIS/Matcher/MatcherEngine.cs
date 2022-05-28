// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS.Matcher
{
    static class MatcherEngine
    {
        public static double Match(FingerprintMatcher probe, FingerprintTemplate candidate)
        {
            // Thread-local storage is fairly fast, but it's still a hash lookup,
            // so do not access FingerprintTransparency.Current repeatedly in tight loops.
            var transparency = FingerprintTransparency.Current;
            var thread = MatcherThread.Current;
            try
            {
                thread.Pairing.ReserveProbe(probe);
                thread.Pairing.ReserveCandidate(candidate);
                thread.Pairing.SupportEnabled = transparency.AcceptsPairing();
                RootEnumerator.Enumerate(probe, candidate, thread.Roots);
                // https://sourceafis.machinezoo.com/transparency/root-pairs
                transparency.LogRootPairs(thread.Roots.Count, thread.Roots.Pairs);
                double high = 0;
                int best = -1;
                for (int i = 0; i < thread.Roots.Count; ++i)
                {
                    EdgeSpider.Crawl(probe.Template.Edges, candidate.Edges, thread.Pairing, thread.Roots.Pairs[i], thread.Queue);
                    // https://sourceafis.machinezoo.com/transparency/pairing
                    transparency.LogPairing(thread.Pairing);
                    Scoring.Compute(probe.Template, candidate, thread.Pairing, thread.Score);
                    // https://sourceafis.machinezoo.com/transparency/score
                    transparency.LogScore(thread.Score);
                    double partial = thread.Score.ShapedScore;
                    if (best < 0 || partial > high)
                    {
                        high = partial;
                        best = i;
                    }
                    thread.Pairing.Clear();
                }
                if (best >= 0 && (transparency.AcceptsBestPairing() || transparency.AcceptsBestScore()))
                {
                    thread.Pairing.SupportEnabled = transparency.AcceptsBestPairing();
                    EdgeSpider.Crawl(probe.Template.Edges, candidate.Edges, thread.Pairing, thread.Roots.Pairs[best], thread.Queue);
                    // https://sourceafis.machinezoo.com/transparency/pairing
                    transparency.LogBestPairing(thread.Pairing);
                    Scoring.Compute(probe.Template, candidate, thread.Pairing, thread.Score);
                    // https://sourceafis.machinezoo.com/transparency/score
                    transparency.LogBestScore(thread.Score);
                    thread.Pairing.Clear();
                }
                thread.Roots.Discard();
                // https://sourceafis.machinezoo.com/transparency/best-match
                transparency.LogBestMatch(best);
                return high;
            }
            catch (Exception)
            {
                MatcherThread.Kill();
                throw;
            }
        }
    }
}
