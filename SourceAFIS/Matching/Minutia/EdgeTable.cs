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
        public int MaxDistance = 1074;
        [Parameter(Lower = 2, Upper = 100)]
        public int MaxNeighbors = 9;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        NeighborEdge[][] Table;
        Template Template;
        List<NeighborEdge> Edges;

        public void Reset(Template template, bool lazy)
        {
            Template = template;
            Edges = new List<NeighborEdge>();
            Table = new NeighborEdge[Template.Minutiae.Length][];

            if (!lazy || Logger.IsActive)
                for (int reference = 0; reference < Table.Length; ++reference)
                    GetEdges(reference);

            Logger.Log(this);
        }

        public NeighborEdge[] GetEdges(int reference)
        {
            if (Table[reference] == null)
            {
                Point referencePosition = Template.Minutiae[reference].Position;
                int sqMaxDistance = Calc.Sq(MaxDistance);
                if (Template.Minutiae.Length - 1 > MaxNeighbors)
                {
                    int[] allSqDistances = new int[Template.Minutiae.Length];
                    for (int neighbor = 0; neighbor < Template.Minutiae.Length; ++neighbor)
                        if (neighbor != reference)
                            allSqDistances[neighbor] = Calc.DistanceSq(referencePosition, Template.Minutiae[neighbor].Position);
                    Array.Sort(allSqDistances);
                    sqMaxDistance = allSqDistances[MaxNeighbors];
                }
                for (int neighbor = 0; neighbor < Template.Minutiae.Length; ++neighbor)
                {
                    if (neighbor != reference && Calc.DistanceSq(referencePosition, Template.Minutiae[neighbor].Position) <= sqMaxDistance)
                    {
                        NeighborEdge record = new NeighborEdge();
                        record.Edge = EdgeConstructor.Construct(Template, reference, neighbor);
                        record.Neighbor = neighbor;
                        Edges.Add(record);
                    }
                }

                Edges.Sort((left, right) => Calc.ChainCompare(
                    Calc.Compare(left.Edge.Length, right.Edge.Length), Calc.Compare(left.Neighbor, right.Neighbor)));
                if (Edges.Count > MaxNeighbors)
                    Edges.RemoveRange(MaxNeighbors, Edges.Count - MaxNeighbors);
                Table[reference] = Edges.ToArray();
                Edges.Clear();
            }
            return Table[reference];
        }
    }
}
