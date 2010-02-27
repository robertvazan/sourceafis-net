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
        [Nested]
        public EdgeConstructor EdgeConstructor = new EdgeConstructor();

        [Parameter(Lower = 2, Upper = 100)]
        public int MaxNeighbors = 10;
        [DpiAdjusted]
        [Parameter(Lower = 0, Upper = 50)]
        public int MaxDistanceError = 20;
        [Parameter]
        public byte MaxAngleError = Angle.FromDegreesB(10);

        struct EdgeRecord
        {
            public EdgeInfo Edge;
            public int Neighbor;
        }

        EdgeRecord[][] Map;

        public void Reset(Template probe)
        {
            Map = new EdgeRecord[probe.Minutiae.Length][];

            List<EdgeRecord> edges = new List<EdgeRecord>();

            for (int reference = 0; reference < Map.Length; ++reference)
            {
                foreach (int neighbor in NeighborIterator.GetNeighbors(probe, reference))
                {
                    EdgeRecord record = new EdgeRecord();
                    record.Edge = EdgeConstructor.Construct(probe, reference, neighbor);
                    record.Neighbor = neighbor;
                    edges.Add(record);
                }

                edges.Sort(delegate(EdgeRecord left, EdgeRecord right) { return Calc.Compare(left.Edge.Length, right.Edge.Length); });
                if (edges.Count > MaxNeighbors)
                    edges.RemoveRange(MaxNeighbors, edges.Count - MaxNeighbors);
                Map[reference] = edges.ToArray();
                edges.Clear();
            }
        }

        public IEnumerable<int> GetMatchingNeighbors(int reference, EdgeInfo candidateEdge)
        {
            foreach (EdgeRecord probeRecord in Map[reference])
            {
                EdgeInfo probeEdge = probeRecord.Edge;
                if (Math.Abs(probeEdge.Length - candidateEdge.Length) <= MaxDistanceError
                    && Angle.Distance(probeEdge.ReferenceAngle, candidateEdge.ReferenceAngle) <= MaxAngleError
                    && Angle.Distance(probeEdge.NeighborAngle, candidateEdge.NeighborAngle) <= MaxAngleError)
                {
                    yield return probeRecord.Neighbor;
                }
            }
        }
    }
}
