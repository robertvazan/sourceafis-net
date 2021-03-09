// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using System.Linq;

namespace SourceAFIS
{
	class Skeleton
	{
		public readonly SkeletonType Type;
		public readonly IntPoint Size;
		public readonly List<SkeletonMinutia> Minutiae = new List<SkeletonMinutia>();

		public Skeleton(BooleanMatrix binary, SkeletonType type)
		{
			Type = type;
			// https://sourceafis.machinezoo.com/transparency/binarized-skeleton
			FingerprintTransparency.Current.Log(Prefix(Type) + "binarized-skeleton", binary);
			Size = binary.Size;
			var thinned = Thin(binary);
			var minutiaPoints = FindMinutiae(thinned);
			var linking = LinkNeighboringMinutiae(minutiaPoints);
			var minutiaMap = MinutiaCenters(linking);
			TraceRidges(thinned, minutiaMap);
			FixLinkingGaps();
			// https://sourceafis.machinezoo.com/transparency/traced-skeleton
			FingerprintTransparency.Current.LogSkeleton("traced-skeleton", this);
			Filter();
		}

		public static string Prefix(SkeletonType type) { return type == SkeletonType.Ridges ? "ridges-" : "valleys-"; }
		enum NeighborhoodType
		{
			Skeleton,
			Ending,
			Removable
		}
		BooleanMatrix Thin(BooleanMatrix input)
		{
			var neighborhoodTypes = NeighborhoodTypes();
			var mutable = new BooleanMatrix(Size);
			for (int y = 1; y < Size.Y - 1; ++y)
				for (int x = 1; x < Size.X - 1; ++x)
					mutable[x, y] = input[x, y];
			var thinned = new BooleanMatrix(Size);
			bool removedAnything = true;
			for (int i = 0; i < Parameters.ThinningIterations && removedAnything; ++i)
			{
				removedAnything = false;
				for (int evenY = 0; evenY < 2; ++evenY)
					for (int evenX = 0; evenX < 2; ++evenX)
						for (int y = 1 + evenY; y < Size.Y - 1; y += 2)
							for (int x = 1 + evenX; x < Size.X - 1; x += 2)
								if (mutable[x, y] && !thinned[x, y] && !(mutable[x, y - 1] && mutable[x, y + 1] && mutable[x - 1, y] && mutable[x + 1, y]))
								{
									uint neighbors = (mutable[x + 1, y + 1] ? 128u : 0u)
										| (mutable[x, y + 1] ? 64u : 0u)
										| (mutable[x - 1, y + 1] ? 32u : 0u)
										| (mutable[x + 1, y] ? 16u : 0u)
										| (mutable[x - 1, y] ? 8u : 0u)
										| (mutable[x + 1, y - 1] ? 4u : 0u)
										| (mutable[x, y - 1] ? 2u : 0u)
										| (mutable[x - 1, y - 1] ? 1u : 0u);
									if (neighborhoodTypes[neighbors] == NeighborhoodType.Removable
										|| neighborhoodTypes[neighbors] == NeighborhoodType.Ending
										&& IsFalseEnding(mutable, new IntPoint(x, y)))
									{
										removedAnything = true;
										mutable[x, y] = false;
									}
									else
										thinned[x, y] = true;
								}
			}
			// https://sourceafis.machinezoo.com/transparency/thinned-skeleton
			FingerprintTransparency.Current.Log(Prefix(Type) + "thinned-skeleton", thinned);
			return thinned;
		}
		static NeighborhoodType[] NeighborhoodTypes()
		{
			var types = new NeighborhoodType[256];
			for (uint mask = 0; mask < 256; ++mask)
			{
				bool TL = (mask & 1) != 0;
				bool TC = (mask & 2) != 0;
				bool TR = (mask & 4) != 0;
				bool CL = (mask & 8) != 0;
				bool CR = (mask & 16) != 0;
				bool BL = (mask & 32) != 0;
				bool BC = (mask & 64) != 0;
				bool BR = (mask & 128) != 0;
				uint count = Integers.PopulationCount(mask);
				bool diagonal = !TC && !CL && TL || !CL && !BC && BL || !BC && !CR && BR || !CR && !TC && TR;
				bool horizontal = !TC && !BC && (TR || CR || BR) && (TL || CL || BL);
				bool vertical = !CL && !CR && (TL || TC || TR) && (BL || BC || BR);
				bool end = (count == 1);
				if (end)
					types[mask] = NeighborhoodType.Ending;
				else if (!diagonal && !horizontal && !vertical)
					types[mask] = NeighborhoodType.Removable;
			}
			return types;
		}
		static bool IsFalseEnding(BooleanMatrix binary, IntPoint ending)
		{
			foreach (var relativeNeighbor in IntPoint.CornerNeighbors)
			{
				var neighbor = ending + relativeNeighbor;
				if (binary[neighbor])
				{
					int count = 0;
					foreach (var relative2 in IntPoint.CornerNeighbors)
						if (binary.Get(neighbor + relative2, false))
							++count;
					return count > 2;
				}
			}
			return false;
		}
		List<IntPoint> FindMinutiae(BooleanMatrix thinned)
		{
			var result = new List<IntPoint>();
			foreach (var at in Size.Iterate())
			{
				if (thinned[at])
				{
					int count = 0;
					foreach (var relative in IntPoint.CornerNeighbors)
						if (thinned.Get(at + relative, false))
							++count;
					if (count == 1 || count > 2)
						result.Add(at);
				}
			}
			return result;
		}
		static Dictionary<IntPoint, List<IntPoint>> LinkNeighboringMinutiae(List<IntPoint> minutiae)
		{
			var linking = new Dictionary<IntPoint, List<IntPoint>>();
			foreach (var minutiaPos in minutiae)
			{
				List<IntPoint> ownLinks = null;
				foreach (var neighborRelative in IntPoint.CornerNeighbors)
				{
					var neighborPos = minutiaPos + neighborRelative;
					if (linking.ContainsKey(neighborPos))
					{
						var neighborLinks = linking[neighborPos];
						if (neighborLinks != ownLinks)
						{
							if (ownLinks != null)
							{
								neighborLinks.AddRange(ownLinks);
								foreach (var mergedPos in ownLinks)
									linking[mergedPos] = neighborLinks;
							}
							ownLinks = neighborLinks;
						}
					}
				}
				if (ownLinks == null)
					ownLinks = new List<IntPoint>();
				ownLinks.Add(minutiaPos);
				linking[minutiaPos] = ownLinks;
			}
			return linking;
		}
		Dictionary<IntPoint, SkeletonMinutia> MinutiaCenters(Dictionary<IntPoint, List<IntPoint>> linking)
		{
			var centers = new Dictionary<IntPoint, SkeletonMinutia>();
			foreach (var currentPos in linking.Keys.OrderBy(p => p))
			{
				var linkedMinutiae = linking[currentPos];
				var primaryPos = linkedMinutiae[0];
				if (!centers.ContainsKey(primaryPos))
				{
					var sum = IntPoint.Zero;
					foreach (var linkedPos in linkedMinutiae)
						sum += linkedPos;
					var center = new IntPoint(sum.X / linkedMinutiae.Count, sum.Y / linkedMinutiae.Count);
					var minutia = new SkeletonMinutia(center);
					AddMinutia(minutia);
					centers[primaryPos] = minutia;
				}
				centers[currentPos] = centers[primaryPos];
			}
			return centers;
		}
		static void TraceRidges(BooleanMatrix thinned, Dictionary<IntPoint, SkeletonMinutia> minutiaePoints)
		{
			var leads = new Dictionary<IntPoint, SkeletonRidge>();
			foreach (var minutiaPoint in minutiaePoints.Keys.OrderBy(p => p))
			{
				foreach (var startRelative in IntPoint.CornerNeighbors)
				{
					var start = minutiaPoint + startRelative;
					if (thinned.Get(start, false) && !minutiaePoints.ContainsKey(start) && !leads.ContainsKey(start))
					{
						var ridge = new SkeletonRidge();
						ridge.Points.Add(minutiaPoint);
						ridge.Points.Add(start);
						var previous = minutiaPoint;
						var current = start;
						do
						{
							var next = IntPoint.Zero;
							foreach (var nextRelative in IntPoint.CornerNeighbors)
							{
								next = current + nextRelative;
								if (thinned.Get(next, false) && next != previous)
									break;
							}
							previous = current;
							current = next;
							ridge.Points.Add(current);
						} while (!minutiaePoints.ContainsKey(current));
						var end = current;
						ridge.Start = minutiaePoints[minutiaPoint];
						ridge.End = minutiaePoints[end];
						leads[ridge.Points[1]] = ridge;
						leads[ridge.Reversed.Points[1]] = ridge;
					}
				}
			}
		}
		void FixLinkingGaps()
		{
			foreach (var minutia in Minutiae)
			{
				foreach (var ridge in minutia.Ridges)
				{
					if (ridge.Points[0] != minutia.Position)
					{
						var filling = ridge.Points[0].LineTo(minutia.Position);
						for (int i = 1; i < filling.Length; ++i)
							ridge.Reversed.Points.Add(filling[i]);
					}
				}
			}
		}
		void Filter()
		{
			RemoveDots();
			// https://sourceafis.machinezoo.com/transparency/removed-dots
			FingerprintTransparency.Current.LogSkeleton("removed-dots", this);
			RemovePores();
			RemoveGaps();
			RemoveTails();
			RemoveFragments();
		}
		void RemoveDots()
		{
			var removed = new List<SkeletonMinutia>();
			foreach (var minutia in Minutiae)
				if (minutia.Ridges.Count == 0)
					removed.Add(minutia);
			foreach (var minutia in removed)
				RemoveMinutia(minutia);
		}
		void RemovePores()
		{
			foreach (var minutia in Minutiae)
			{
				if (minutia.Ridges.Count == 3)
				{
					for (int exit = 0; exit < 3; ++exit)
					{
						var exitRidge = minutia.Ridges[exit];
						var arm1 = minutia.Ridges[(exit + 1) % 3];
						var arm2 = minutia.Ridges[(exit + 2) % 3];
						if (arm1.End == arm2.End && exitRidge.End != arm1.End && arm1.End != minutia && exitRidge.End != minutia)
						{
							var end = arm1.End;
							if (end.Ridges.Count == 3 && arm1.Points.Count <= Parameters.MaxPoreArm && arm2.Points.Count <= Parameters.MaxPoreArm)
							{
								arm1.Detach();
								arm2.Detach();
								var merged = new SkeletonRidge();
								merged.Start = minutia;
								merged.End = end;
								foreach (var point in minutia.Position.LineTo(end.Position))
									merged.Points.Add(point);
							}
							break;
						}
					}
				}
			}
			RemoveKnots();
			// https://sourceafis.machinezoo.com/transparency/removed-pores
			FingerprintTransparency.Current.LogSkeleton("removed-pores", this);
		}
		class Gap : IComparable<Gap>
		{
			public int Distance;
			public SkeletonMinutia End1;
			public SkeletonMinutia End2;
			public int CompareTo(Gap other)
			{
				int distanceCmp = Distance.CompareTo(other.Distance);
				if (distanceCmp != 0)
					return distanceCmp;
				int end1Cmp = End1.Position.CompareTo(other.End1.Position);
				if (end1Cmp != 0)
					return end1Cmp;
				return End2.Position.CompareTo(other.End2.Position);
			}
		}
		void RemoveGaps()
		{
			var queue = new PriorityQueue<Gap>();
			foreach (var end1 in Minutiae)
				if (end1.Ridges.Count == 1 && end1.Ridges[0].Points.Count >= Parameters.ShortestJoinedEnding)
					foreach (var end2 in Minutiae)
						if (end2 != end1 && end2.Ridges.Count == 1 && end1.Ridges[0].End != end2
							&& end2.Ridges[0].Points.Count >= Parameters.ShortestJoinedEnding && IsWithinGapLimits(end1, end2))
						{
							var gap = new Gap();
							gap.Distance = (end1.Position - end2.Position).LengthSq;
							gap.End1 = end1;
							gap.End2 = end2;
							queue.Add(gap);
						}
			var shadow = Shadow();
			while (queue.Count > 0)
			{
				var gap = queue.Remove();
				if (gap.End1.Ridges.Count == 1 && gap.End2.Ridges.Count == 1)
				{
					var line = gap.End1.Position.LineTo(gap.End2.Position);
					if (!IsRidgeOverlapping(line, shadow))
						AddGapRidge(shadow, gap, line);
				}
			}
			RemoveKnots();
			// https://sourceafis.machinezoo.com/transparency/removed-gaps
			FingerprintTransparency.Current.LogSkeleton("removed-gaps", this);
		}
		static bool IsWithinGapLimits(SkeletonMinutia end1, SkeletonMinutia end2)
		{
			int distanceSq = (end1.Position - end2.Position).LengthSq;
			if (distanceSq <= Integers.Sq(Parameters.MaxRuptureSize))
				return true;
			if (distanceSq > Integers.Sq(Parameters.MaxGapSize))
				return false;
			double gapDirection = DoubleAngle.Atan(end1.Position, end2.Position);
			double direction1 = DoubleAngle.Atan(end1.Position, AngleSampleForGapRemoval(end1));
			if (DoubleAngle.Distance(direction1, DoubleAngle.Opposite(gapDirection)) > Parameters.MaxGapAngle)
				return false;
			double direction2 = DoubleAngle.Atan(end2.Position, AngleSampleForGapRemoval(end2));
			if (DoubleAngle.Distance(direction2, gapDirection) > Parameters.MaxGapAngle)
				return false;
			return true;
		}
		static IntPoint AngleSampleForGapRemoval(SkeletonMinutia minutia)
		{
			var ridge = minutia.Ridges[0];
			if (Parameters.GapAngleOffset < ridge.Points.Count)
				return ridge.Points[Parameters.GapAngleOffset];
			else
				return ridge.End.Position;
		}
		static bool IsRidgeOverlapping(IntPoint[] line, BooleanMatrix shadow)
		{
			for (int i = Parameters.ToleratedGapOverlap; i < line.Length - Parameters.ToleratedGapOverlap; ++i)
				if (shadow[line[i]])
					return true;
			return false;
		}
		static void AddGapRidge(BooleanMatrix shadow, Gap gap, IntPoint[] line)
		{
			var ridge = new SkeletonRidge();
			foreach (var point in line)
				ridge.Points.Add(point);
			ridge.Start = gap.End1;
			ridge.End = gap.End2;
			foreach (var point in line)
				shadow[point] = true;
		}
		void RemoveTails()
		{
			foreach (var minutia in Minutiae)
			{
				if (minutia.Ridges.Count == 1 && minutia.Ridges[0].End.Ridges.Count >= 3)
					if (minutia.Ridges[0].Points.Count < Parameters.MinTailLength)
						minutia.Ridges[0].Detach();
			}
			RemoveDots();
			RemoveKnots();
			// https://sourceafis.machinezoo.com/transparency/removed-tails
			FingerprintTransparency.Current.LogSkeleton("removed-tails", this);
		}
		void RemoveFragments()
		{
			foreach (var minutia in Minutiae)
				if (minutia.Ridges.Count == 1)
				{
					var ridge = minutia.Ridges[0];
					if (ridge.End.Ridges.Count == 1 && ridge.Points.Count < Parameters.MinFragmentLength)
						ridge.Detach();
				}
			RemoveDots();
			// https://sourceafis.machinezoo.com/transparency/removed-fragments
			FingerprintTransparency.Current.LogSkeleton("removed-fragments", this);
		}
		void RemoveKnots()
		{
			foreach (var minutia in Minutiae)
			{
				if (minutia.Ridges.Count == 2 && minutia.Ridges[0].Reversed != minutia.Ridges[1])
				{
					var extended = minutia.Ridges[0].Reversed;
					var removed = minutia.Ridges[1];
					if (extended.Points.Count < removed.Points.Count)
					{
						var tmp = extended;
						extended = removed;
						removed = tmp;
						extended = extended.Reversed;
						removed = removed.Reversed;
					}
					extended.Points.RemoveAt(extended.Points.Count - 1);
					foreach (var point in removed.Points)
						extended.Points.Add(point);
					extended.End = removed.End;
					removed.Detach();
				}
			}
			RemoveDots();
		}
		public void AddMinutia(SkeletonMinutia minutia) { Minutiae.Add(minutia); }
		public void RemoveMinutia(SkeletonMinutia minutia) { Minutiae.Remove(minutia); }
		BooleanMatrix Shadow()
		{
			var shadow = new BooleanMatrix(Size);
			foreach (var minutia in Minutiae)
			{
				shadow[minutia.Position] = true;
				foreach (var ridge in minutia.Ridges)
					if (ridge.Start.Position.Y <= ridge.End.Position.Y)
						foreach (var point in ridge.Points)
							shadow[point] = true;
			}
			return shadow;
		}
	}
}
