// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;

namespace SourceAFIS
{
	public class FingerprintMatcher
	{
		void BuildEdgeHash()
		{
			for (int referenceMinutia = 0; referenceMinutia < Template.Minutiae.Count; ++referenceMinutia)
				for (int neighborMinutia = 0; neighborMinutia < Template.Minutiae.Count; ++neighborMinutia)
					if (referenceMinutia != neighborMinutia)
					{
						var edge = new IndexedEdge()
						{
							Shape = new EdgeShape(Template, referenceMinutia, neighborMinutia),
							Reference = referenceMinutia,
							Neighbor = neighborMinutia
						};
						foreach (var hash in GetShapeCoverage(edge.Shape))
						{
							List<IndexedEdge> list;
							if (!EdgeHash.TryGetValue(hash, out list))
								EdgeHash[hash] = list = new List<IndexedEdge>();
							list.Add(edge);
						}
					}
		}

		static IEnumerable<int> GetShapeCoverage(EdgeShape edge)
		{
			int minLengthBin = (edge.Length - MaxDistanceError) / MaxDistanceError;
			int maxLengthBin = (edge.Length + MaxDistanceError) / MaxDistanceError;
			int angleBins = MathEx.DivRoundUp(256, MaxAngleError);
			int minReferenceBin = Angle.Difference(edge.ReferenceAngle, MaxAngleError) / MaxAngleError;
			int maxReferenceBin = Angle.Add(edge.ReferenceAngle, MaxAngleError) / MaxAngleError;
			int endReferenceBin = (maxReferenceBin + 1) % angleBins;
			int minNeighborBin = Angle.Difference(edge.NeighborAngle, MaxAngleError) / MaxAngleError;
			int maxNeighborBin = Angle.Add(edge.NeighborAngle, MaxAngleError) / MaxAngleError;
			int endNeighborBin = (maxNeighborBin + 1) % angleBins;
			for (int lengthBin = minLengthBin; lengthBin <= maxLengthBin; ++lengthBin)
				for (int referenceBin = minReferenceBin; referenceBin != endReferenceBin; referenceBin = (referenceBin + 1) % angleBins)
					for (int neighborBin = minNeighborBin; neighborBin != endNeighborBin; neighborBin = (neighborBin + 1) % angleBins)
						yield return (referenceBin << 24) + (neighborBin << 16) + lengthBin;
		}
	}
}
