using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching
{
    public sealed class ProbeNeighbors
    {
        [Nested]
        public NeighborIterator NeighborIterator = new NeighborIterator();

        public int MaxNeighbors = 10;
        [DpiAdjusted]
        public int MaxDistanceError = 20;
        public byte MaxAngleError = Angle.FromDegreesB(10);

        struct EdgeInfo
        {
            public float Length;
            public byte ReferenceAngle;
            public byte NeighborAngle;
            public int Neighbor;
        }

        EdgeInfo[][] Map;

        public void Reset(Template probe)
        {
            Map = new EdgeInfo[probe.Minutiae.Length][];

            List<EdgeInfo> edges = new List<EdgeInfo>();
            EdgeAnalysis analysis = new EdgeAnalysis();
            analysis.Template = probe;

            for (int reference = 0; reference < Map.Length; ++reference)
            {
                analysis.ReferenceIndex = reference;
                foreach (int neighbor in NeighborIterator.GetNeighbors(probe, reference))
                {
                    analysis.NeighborIndex = neighbor;
                    analysis.ComputeAll();
                    EdgeInfo edge = new EdgeInfo();
                    edge.Length = analysis.EdgeLength;
                    edge.ReferenceAngle = analysis.ReferenceAngle;
                    edge.NeighborAngle = analysis.NeighborAngle;
                    edge.Neighbor = neighbor;
                    edges.Add(edge);
                }

                edges.Sort(delegate(EdgeInfo left, EdgeInfo right) { return Calc.Compare(left.Length, right.Length); });
                if (edges.Count > MaxNeighbors)
                    edges.RemoveRange(MaxNeighbors, edges.Count - MaxNeighbors);
                Map[reference] = edges.ToArray();
                edges.Clear();
            }
        }

        public IEnumerable<int> GetMatchingNeighbors(int reference, EdgeAnalysis candidateEdge)
        {
            foreach (EdgeInfo edge in Map[reference])
            {
                if (Math.Abs(edge.Length - candidateEdge.EdgeLength) <= MaxDistanceError
                    && Angle.Distance(edge.ReferenceAngle, candidateEdge.ReferenceAngle) <= MaxAngleError
                    && Angle.Distance(edge.NeighborAngle, candidateEdge.NeighborAngle) <= MaxAngleError)
                {
                    yield return edge.Neighbor;
                }
            }
        }
    }
}
