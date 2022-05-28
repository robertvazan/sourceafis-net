// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using SourceAFIS.Configuration;

namespace SourceAFIS.Features
{
    class NeighborEdge : EdgeShape
    {
        public readonly int Neighbor;

        public NeighborEdge(ImmutableMinutia[] minutiae, int reference, int neighbor) : base(minutiae[reference], minutiae[neighbor]) => Neighbor = neighbor;

        public static NeighborEdge[][] BuildTable(ImmutableMinutia[] minutiae)
        {
            var edges = new NeighborEdge[minutiae.Length][];
            var star = new List<NeighborEdge>();
            var allSqDistances = new int[minutiae.Length];
            for (int reference = 0; reference < edges.Length; ++reference)
            {
                var referencePosition = minutiae[reference].Position;
                int maxSqDistance = int.MaxValue;
                if (minutiae.Length - 1 > Parameters.EdgeTableNeighbors)
                {
                    for (int neighbor = 0; neighbor < minutiae.Length; ++neighbor)
                        allSqDistances[neighbor] = (referencePosition - minutiae[neighbor].Position).LengthSq;
                    Array.Sort(allSqDistances);
                    maxSqDistance = allSqDistances[Parameters.EdgeTableNeighbors];
                }
                for (int neighbor = 0; neighbor < minutiae.Length; ++neighbor)
                {
                    if (neighbor != reference && (referencePosition - minutiae[neighbor].Position).LengthSq <= maxSqDistance)
                        star.Add(new NeighborEdge(minutiae, reference, neighbor));
                }
                star.Sort((a, b) =>
                {
                    int lengthCmp = a.Length.CompareTo(b.Length);
                    if (lengthCmp != 0)
                        return lengthCmp;
                    return a.Neighbor.CompareTo(b.Neighbor);
                });
                while (star.Count > Parameters.EdgeTableNeighbors)
                    star.RemoveAt(star.Count - 1);
                edges[reference] = star.ToArray();
                star.Clear();
            }
            // https://sourceafis.machinezoo.com/transparency/edge-table
            FingerprintTransparency.Current.Log("edge-table", edges);
            return edges;
        }
    }
}
