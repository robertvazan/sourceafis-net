using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Templates;

namespace SourceAFIS.Matching.Minutia
{
    public sealed class EdgeTable
    {
        [Nested]
        public EdgeConstructor EdgeConstructor = new EdgeConstructor();

        [Parameter(Lower = 30, Upper = 1500)]
        public int MaxDistance = 490;
        [Parameter(Lower = 2, Upper = 100)]
        public int MaxNeighbors = 9;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        public NeighborEdge[][] Table;

        public void Reset(Template template)
        {
            lock (template)
                Table = template.MatcherCache as NeighborEdge[][];

            if (Table == null)
            {
                BuildTable(template);

                lock (template)
                    template.MatcherCache = Table;
            }

            Logger.Log(this);
        }

        void BuildTable(Template template)
        {
            Table = new NeighborEdge[template.Minutiae.Length][];
            var edges = new List<NeighborEdge>();
            var allSqDistances = new int[template.Minutiae.Length];

            for (int reference = 0; reference < Table.Length; ++reference)
            {
                Point referencePosition = template.Minutiae[reference].Position;
                int sqMaxDistance = Calc.Sq(MaxDistance);
                if (template.Minutiae.Length - 1 > MaxNeighbors)
                {
                    for (int neighbor = 0; neighbor < template.Minutiae.Length; ++neighbor)
                        allSqDistances[neighbor] = Calc.DistanceSq(referencePosition, template.Minutiae[neighbor].Position);
                    Array.Sort(allSqDistances);
                    sqMaxDistance = allSqDistances[MaxNeighbors];
                }
                for (int neighbor = 0; neighbor < template.Minutiae.Length; ++neighbor)
                {
                    if (neighbor != reference && Calc.DistanceSq(referencePosition, template.Minutiae[neighbor].Position) <= sqMaxDistance)
                    {
                        NeighborEdge record = new NeighborEdge();
                        record.Edge = EdgeConstructor.Construct(template, reference, neighbor);
                        record.Neighbor = neighbor;
                        edges.Add(record);
                    }
                }

                edges.Sort(NeighborEdgeComparer.Instance);
                if (edges.Count > MaxNeighbors)
                    edges.RemoveRange(MaxNeighbors, edges.Count - MaxNeighbors);
                Table[reference] = edges.ToArray();
                edges.Clear();
            }
        }

        class NeighborEdgeComparer : IComparer<NeighborEdge>
        {
            public int Compare(NeighborEdge left, NeighborEdge right)
            {
                int result = Calc.Compare(left.Edge.Length, right.Edge.Length);
                if (result != 0)
                    return result;
                return Calc.Compare(left.Neighbor, right.Neighbor);
            }

            public static NeighborEdgeComparer Instance = new NeighborEdgeComparer();
        }
    }
}
