// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using SourceAFIS.Engine.Configuration;
using SourceAFIS.Engine.Features;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Matcher
{
    static class EdgeSpider
    {
        static readonly float ComplementaryMaxAngleError = FloatAngle.Complementary(Parameters.MaxAngleError);
        static List<MinutiaPair> MatchPairs(NeighborEdge[] pstar, NeighborEdge[] cstar, MinutiaPairPool pool)
        {
            var results = new List<MinutiaPair>();
            int start = 0;
            int end = 0;
            for (int cindex = 0; cindex < cstar.Length; ++cindex)
            {
                var cedge = cstar[cindex];
                while (start < pstar.Length && pstar[start].Shape.Length < cedge.Shape.Length - Parameters.MaxDistanceError)
                    ++start;
                if (end < start)
                    end = start;
                while (end < pstar.Length && pstar[end].Shape.Length <= cedge.Shape.Length + Parameters.MaxDistanceError)
                    ++end;
                for (int pindex = start; pindex < end; ++pindex)
                {
                    var pedge = pstar[pindex];
                    float rdiff = FloatAngle.Difference(pedge.Shape.ReferenceAngle, cedge.Shape.ReferenceAngle);
                    if (rdiff <= Parameters.MaxAngleError || rdiff >= ComplementaryMaxAngleError)
                    {
                        float ndiff = FloatAngle.Difference(pedge.Shape.NeighborAngle, cedge.Shape.NeighborAngle);
                        if (ndiff <= Parameters.MaxAngleError || ndiff >= ComplementaryMaxAngleError)
                        {
                            var pair = pool.Allocate();
                            pair.Probe = pedge.Neighbor;
                            pair.Candidate = cedge.Neighbor;
                            pair.Distance = cedge.Shape.Length;
                            results.Add(pair);
                        }
                    }
                }
            }
            return results;
        }
        static void CollectEdges(NeighborEdge[][] pedges, NeighborEdge[][] cedges, PairingGraph pairing, PriorityQueue<MinutiaPair> queue)
        {
            var reference = pairing.Tree[pairing.Count - 1];
            var pstar = pedges[reference.Probe];
            var cstar = cedges[reference.Candidate];
            foreach (var pair in MatchPairs(pstar, cstar, pairing.Pool))
            {
                pair.ProbeRef = reference.Probe;
                pair.CandidateRef = reference.Candidate;
                if (pairing.ByCandidate[pair.Candidate] == null && pairing.ByProbe[pair.Probe] == null)
                    queue.Add(pair);
                else
                    pairing.Support(pair);
            }
        }
        static void SkipPaired(PairingGraph pairing, PriorityQueue<MinutiaPair> queue)
        {
            while (queue.Count > 0 && (pairing.ByProbe[queue.Peek().Probe] != null || pairing.ByCandidate[queue.Peek().Candidate] != null))
                pairing.Support(queue.Remove());
        }
        public static void Crawl(NeighborEdge[][] pedges, NeighborEdge[][] cedges, PairingGraph pairing, MinutiaPair root, PriorityQueue<MinutiaPair> queue)
        {
            queue.Add(root);
            do
            {
                pairing.AddPair(queue.Remove());
                CollectEdges(pedges, cedges, pairing, queue);
                SkipPaired(pairing, queue);
            } while (queue.Count > 0);
        }
    }
}
