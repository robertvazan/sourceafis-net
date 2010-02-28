using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching
{
    public sealed class EdgeTable
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

        public NeighborEdge[][] Table;

        public void Reset(Template template)
        {
            Table = new NeighborEdge[template.Minutiae.Length][];

            List<NeighborEdge> edges = new List<NeighborEdge>();

            for (int reference = 0; reference < Table.Length; ++reference)
            {
                foreach (int neighbor in NeighborIterator.GetNeighbors(template, reference))
                {
                    NeighborEdge record = new NeighborEdge();
                    record.Edge = EdgeConstructor.Construct(template, reference, neighbor);
                    record.Neighbor = neighbor;
                    edges.Add(record);
                }

                edges.Sort(delegate(NeighborEdge left, NeighborEdge right) { return Calc.Compare(left.Edge.Length, right.Edge.Length); });
                if (edges.Count > MaxNeighbors)
                    edges.RemoveRange(MaxNeighbors, edges.Count - MaxNeighbors);
                Table[reference] = edges.ToArray();
                edges.Clear();
            }
        }

        public IEnumerable<int> GetMatchingNeighbors(int reference, EdgeInfo candidateEdge)
        {
            foreach (NeighborEdge probeRecord in Table[reference])
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
