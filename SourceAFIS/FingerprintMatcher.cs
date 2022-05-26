// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using SourceAFIS.Configuration;
using SourceAFIS.Features;
using SourceAFIS.Matcher;
using SourceAFIS.Primitives;

namespace SourceAFIS
{
    /// <summary>Fingerprint template representation optimized for fast 1:N matching.</summary>
    /// <remarks>
    /// <para>
    /// <c>FingerprintMatcher</c> maintains data structures that improve matching speed at the cost of some RAM.
    /// It can efficiently match one probe fingerprint to many candidate fingerprints.
    /// </para>
    /// <para>
    /// New matcher is created by passing probe fingerprint template to <see cref="FingerprintMatcher(FingerprintTemplate)" /> constructor.
    /// Candidate fingerprint templates are then passed one by one to <see cref="Match(FingerprintTemplate)" /> method.
    /// </para>
    /// </remarks>
    /// <seealso href="https://sourceafis.machinezoo.com/java">SourceAFIS for Java tutorial</seealso>
    /// <seealso cref="FingerprintTemplate" />
    public class FingerprintMatcher
    {
        internal readonly FingerprintTemplate Template;
        internal readonly Dictionary<int, List<IndexedEdge>> EdgeHash;

        /// <summary>Creates fingerprint template representation optimized for fast 1:N matching.</summary>
        /// <remarks>
        /// <para>
        /// Once the probe template is processed, candidate templates can be compared to it
        /// by calling <see cref="Match(FingerprintTemplate)" />.
        /// </para>
        /// <para>
        /// This constructor is expensive in terms of RAM footprint and CPU usage.
        /// Initialized <c>FingerprintMatcher</c> should be reused for multiple <see cref="Match(FingerprintTemplate)" /> calls in 1:N matching.
        /// </para>
        /// </remarks>
        /// <param name="probe">Probe fingerprint template to be matched to candidate fingerprints.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="probe" /> is <c>null</c>.</exception>
        /// <seealso cref="Match(FingerprintTemplate)" />
        public FingerprintMatcher(FingerprintTemplate probe)
        {
            if (probe == null)
                throw new ArgumentNullException(nameof(probe));
            Template = probe;
            EdgeHash = BuildEdgeHash(probe);
        }

        static Dictionary<int, List<IndexedEdge>> BuildEdgeHash(FingerprintTemplate template)
        {
            var map = new Dictionary<int, List<IndexedEdge>>();
            for (int reference = 0; reference < template.Minutiae.Length; ++reference)
                for (int neighbor = 0; neighbor < template.Minutiae.Length; ++neighbor)
                    if (reference != neighbor)
                    {
                        var edge = new IndexedEdge(template.Minutiae, reference, neighbor);
                        foreach (int hash in ShapeCoverage(edge))
                        {
                            List<IndexedEdge> list;
                            if (!map.TryGetValue(hash, out list))
                                map[hash] = list = new List<IndexedEdge>();
                            list.Add(edge);
                        }
                    }
            // https://sourceafis.machinezoo.com/transparency/edge-hash
            FingerprintTransparency.Current.LogEdgeHash(map);
            return map;
        }
        static List<int> ShapeCoverage(EdgeShape edge)
        {
            int minLengthBin = (edge.Length - Parameters.MaxDistanceError) / Parameters.MaxDistanceError;
            int maxLengthBin = (edge.Length + Parameters.MaxDistanceError) / Parameters.MaxDistanceError;
            int angleBins = (int)Math.Ceiling(2 * Math.PI / Parameters.MaxAngleError);
            int minReferenceBin = (int)(DoubleAngle.Difference(edge.ReferenceAngle, Parameters.MaxAngleError) / Parameters.MaxAngleError);
            int maxReferenceBin = (int)(DoubleAngle.Add(edge.ReferenceAngle, Parameters.MaxAngleError) / Parameters.MaxAngleError);
            int endReferenceBin = (maxReferenceBin + 1) % angleBins;
            int minNeighborBin = (int)(DoubleAngle.Difference(edge.NeighborAngle, Parameters.MaxAngleError) / Parameters.MaxAngleError);
            int maxNeighborBin = (int)(DoubleAngle.Add(edge.NeighborAngle, Parameters.MaxAngleError) / Parameters.MaxAngleError);
            int endNeighborBin = (maxNeighborBin + 1) % angleBins;
            var coverage = new List<int>();
            for (int lengthBin = minLengthBin; lengthBin <= maxLengthBin; ++lengthBin)
                for (int referenceBin = minReferenceBin; referenceBin != endReferenceBin; referenceBin = (referenceBin + 1) % angleBins)
                    for (int neighborBin = minNeighborBin; neighborBin != endNeighborBin; neighborBin = (neighborBin + 1) % angleBins)
                        coverage.Add((referenceBin << 24) + (neighborBin << 16) + lengthBin);
            return coverage;
        }
        /// <summary>Matches candidate fingerprint to probe fingerprint and calculates similarity score.</summary>
        /// <remarks>
        /// <para>
        /// Candidate fingerprint in <paramref name="candidate" /> parameter is matched to probe fingerprint
        /// previously passed to <see cref="FingerprintMatcher(FingerprintTemplate)" /> constructor.
        /// </para>
        /// <para>
        /// Returned similarity score is a non-negative number that increases with similarity between probe and candidate fingerprints.
        /// Application should compare the score to a threshold with expression <c>(score >= threshold)</c> to arrive at boolean match/non-match decision.
        /// Threshold 10 corresponds to FMR (False Match Rate, see <see href="https://en.wikipedia.org/wiki/Biometrics#Performance">Biometric Performance</see>
        /// and <see href="https://en.wikipedia.org/wiki/Confusion_matrix">Confusion matrix</see>) of 10%, threshold 20 to FMR 1%, threshold 30 to FMR 0.1%, and so on.
        /// </para>
        /// <para>
        /// Recommended threshold is 40, which corresponds to FMR 0.01%.
        /// Correspondence between threshold and FMR is approximate and varies with quality of fingerprints being matched.
        /// Increasing threshold rapidly reduces FMR, but it also slowly increases FNMR (False Non-Match Rate).
        /// Threshold must be tailored to the needs of the application.
        /// </para>
        /// <para>
        /// This method is thread-safe. Multiple threads can match candidates against single <c>FingerprintMatcher</c>.
        /// </para>
        /// </remarks>
        /// <param name="candidate">Fingerprint template to be matched with probe fingerprint represented by this <c>FingerprintMatcher</c>.</param>
        /// <returns>Similarity score between probe and candidate fingerprints.</returns>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="candidate" /> is <c>null</c>.</exception>
        public double Match(FingerprintTemplate candidate)
        {
            if (candidate == null)
                throw new ArgumentNullException(nameof(candidate));
            var thread = MatcherThread.Current;
            thread.SelectMatcher(this);
            thread.SelectCandidate(candidate);
            return thread.Match();
        }
    }
}
