// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;

namespace SourceAFIS
{
	public class FingerprintMatcher
	{
		internal readonly FingerprintTemplate Template;
		internal readonly Dictionary<int, List<IndexedEdge>> EdgeHash;

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
