using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class EdgeTable
    {
        [Nested]
        public EdgeConstructor EdgeConstructor = new EdgeConstructor();

        [Parameter(Lower = 30, Upper = 1500)]
        public int MaxDistance = 191;
        [Parameter(Lower = 2, Upper = 100)]
        public int MaxNeighbors = 9;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        public NeighborEdge[][] Table;

        public void Reset(Template template)
        {
            Table = new NeighborEdge[template.Minutiae.Length][];

            List<NeighborEdge> edges = new List<NeighborEdge>();

            for (int reference = 0; reference < Table.Length; ++reference)
            {
                Point referencePosition = template.Minutiae[reference].Position;
                for (int neighbor = 0; neighbor < template.Minutiae.Length; ++neighbor)
                {
                    if (Calc.DistanceSq(referencePosition, template.Minutiae[neighbor].Position)
                        <= Calc.Sq(MaxDistance) && neighbor != reference)
                    {
                        NeighborEdge record = new NeighborEdge();
                        record.Edge = EdgeConstructor.Construct(template, reference, neighbor);
                        record.Neighbor = neighbor;
                        edges.Add(record);
                    }
                }

                edges.Sort((left, right) => Calc.ChainCompare(
                    Calc.Compare(left.Edge.Length, right.Edge.Length), Calc.Compare(left.Neighbor, right.Neighbor)));
                if (edges.Count > MaxNeighbors)
                    edges.RemoveRange(MaxNeighbors, edges.Count - MaxNeighbors);
                Table[reference] = edges.ToArray();
                edges.Clear();
            }

            Logger.Log(this);
        }
    }
}
