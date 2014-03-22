using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SourceAFIS.Utils;

namespace SourceAFIS
{
    class FingerprintSkeleton
    {
        public Point Size;
        public List<SkeletonMinutia> Minutiae = new List<SkeletonMinutia>();

        public FingerprintSkeleton(bool[,] binary)
        {
            Size = Point.SizeOf(binary);
            var thinned = Thin(binary);
            var minutiaPoints = FindMinutiae(thinned);
            var linking = LinkNeighboringMinutiae(minutiaPoints);
            var minutiaMap = ComputeMinutiaCenters(linking);
            TraceRidges(thinned, minutiaMap);
            FixLinkingGaps();
            Filter();
        }

        List<Point> FindMinutiae(bool[,] thinned)
        {
            List<Point> result = new List<Point>();
            foreach (var at in Size)
				if (at.Get(thinned))
	            {
	                int count = 0;
	                foreach (var relative in Point.CornerNeighbors)
	                    if ((at + relative).Get(thinned, false))
	                        ++count;
	                if (count == 1 || count > 2)
	                    result.Add(at);
	            }
            return result;
        }

        static Dictionary<Point, List<Point>> LinkNeighboringMinutiae(List<Point> minutiae)
        {
            Dictionary<Point, List<Point>> linking = new Dictionary<Point, List<Point>>();
            foreach (Point minutiaPos in minutiae)
            {
                List<Point> ownLinks = null;
                foreach (Point neighborRelative in Point.CornerNeighbors)
                {
                    Point neighborPos = minutiaPos + neighborRelative;
                    if (linking.ContainsKey(neighborPos))
                    {
                        List<Point> neighborLinks = linking[neighborPos];
                        if (neighborLinks != ownLinks)
                        {
                            if (ownLinks != null)
                            {
                                neighborLinks.AddRange(ownLinks);
                                foreach (Point mergedPos in ownLinks)
                                    linking[mergedPos] = neighborLinks;
                            }
                            ownLinks = neighborLinks;
                        }
                    }
                }
                if (ownLinks == null)
                    ownLinks = new List<Point>();
                ownLinks.Add(minutiaPos);
                linking[minutiaPos] = ownLinks;
            }
            return linking;
        }

        Dictionary<Point, SkeletonMinutia> ComputeMinutiaCenters(Dictionary<Point, List<Point>> linking)
        {
            Dictionary<Point, SkeletonMinutia> centers = new Dictionary<Point, SkeletonMinutia>();
            foreach (Point currentPos in linking.Keys)
            {
                List<Point> linkedMinutiae = linking[currentPos];
                Point primaryPos = linkedMinutiae[0];
                if (!centers.ContainsKey(primaryPos))
                {
                    Point sum = new Point();
                    foreach (Point linkedPos in linkedMinutiae)
                        sum += linkedPos;
                    Point center = new Point(sum.X / linkedMinutiae.Count, sum.Y / linkedMinutiae.Count);
                    SkeletonMinutia minutia = new SkeletonMinutia(center);
                    AddMinutia(minutia);
                    centers[primaryPos] = minutia;
                }
                centers[currentPos] = centers[primaryPos];
            }
            return centers;
        }

        static void TraceRidges(bool[,] thinned, Dictionary<Point, SkeletonMinutia> minutiaePoints)
        {
            Dictionary<Point, SkeletonRidge> leads = new Dictionary<Point, SkeletonRidge>();
            foreach (Point minutiaPoint in minutiaePoints.Keys)
            {
                foreach (Point startRelative in Point.CornerNeighbors)
                {
                    Point start = minutiaPoint + startRelative;
                    if (start.Get(thinned, false) && !minutiaePoints.ContainsKey(start) && !leads.ContainsKey(start))
                    {
                        SkeletonRidge ridge = new SkeletonRidge();
                        ridge.Points.Add(minutiaPoint);
                        ridge.Points.Add(start);
                        Point previous = minutiaPoint;
                        Point current = start;
                        do
                        {
                            Point next = new Point();
                            foreach (Point nextRelative in Point.CornerNeighbors)
                            {
                                next = current + nextRelative;
                                if (next.Get(thinned, false) && next != previous)
                                    break;
                            }
                            previous = current;
                            current = next;
                            ridge.Points.Add(current);
                        } while (!minutiaePoints.ContainsKey(current));
                        Point end = current;

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
            foreach (SkeletonMinutia minutia in Minutiae)
            {
                foreach (SkeletonRidge ridge in minutia.Ridges)
                {
                    if (ridge.Points[0] != minutia.Position)
                    {
                        Point[] filling = MathEx.ConstructLine(ridge.Points[0], minutia.Position);
                        for (int i = 1; i < filling.Length; ++i)
                            ridge.Reversed.Points.Add(filling[i]);
                    }
                }
            }
        }

        enum NeighborhoodType
        {
            Skeleton,
            Ending,
            Removable
        }

        bool[,] Thin(bool[,] input)
        {
            const int maxIterations = 26;

            var neighborhoodTypes = GetNeighborhoodTypes();
            var partial = Size.Allocate<bool>();
            for (int y = 1; y < Size.Y - 1; ++y)
                for (int x = 1; x < Size.X - 1; ++x)
                    partial[y, x] = input[y, x];

            var thinned = Size.Allocate<bool>();
            bool removedAnything = true;
            for (int i = 0; i < maxIterations && removedAnything; ++i)
            {
                removedAnything = false;
                for (int evenY = 0; evenY < 2; ++evenY)
                    for (int evenX = 0; evenX < 2; ++evenX)
                        for (int y = 1 + evenY; y < Size.Y - 1; y += 2)
                            for (int x = 1 + evenX; x < Size.X - 1; x += 2)
                                if (partial[y, x] && !thinned[y, x] && !(partial[y - 1, x] && partial[y + 1, x] && partial[y, x - 1] && partial[y, x + 1]))
                                {
                                    uint neighbors = (partial[y + 1, x + 1] ? 128u : 0u)
                                        | (partial[y + 1, x] ? 64u : 0u)
                                        | (partial[y + 1, x - 1] ? 32u : 0u)
                                        | (partial[y, x + 1] ? 16u : 0u)
                                        | (partial[y, x - 1] ? 8u : 0u)
                                        | (partial[y - 1, x + 1] ? 4u : 0u)
                                        | (partial[y - 1, x] ? 2u : 0u)
                                        | (partial[y - 1, x - 1] ? 1u : 0u);
                                    if (neighborhoodTypes[neighbors] == NeighborhoodType.Removable
                                        || neighborhoodTypes[neighbors] == NeighborhoodType.Ending
                                        && IsFalseEnding(partial, new Point(x, y)))
                                    {
                                        removedAnything = true;
                                        partial[y, x] = false;
                                    }
                                    else
                                        thinned[y, x] = true;
                                }
            }

            return thinned;
        }

        static NeighborhoodType[] GetNeighborhoodTypes()
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

                int count = MathEx.CountBits(mask);

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

        static bool IsFalseEnding(bool[,] binary, Point ending)
        {
            foreach (Point relativeNeighbor in Point.CornerNeighbors)
            {
                Point neighbor = ending + relativeNeighbor;
                if (neighbor.Get(binary))
                {
                    int count = 0;
                    foreach (var relative2 in Point.CornerNeighbors)
                        if ((neighbor + relative2).Get(binary, false))
                            ++count;
                    return count > 2;
                }
            }
            return false;
        }

        void Filter()
        {
            RemoveDots();
            RemovePores();
            RemoveGaps();
            RemoveTails();
            RemoveFragments();
            DisableBranchMinutiae();
        }

        void RemovePores()
        {
            const int maxArmLength = 41;
            foreach (SkeletonMinutia minutia in Minutiae)
            {
                if (minutia.Ridges.Count == 3)
                {
                    for (int exit = 0; exit < 3; ++exit)
                    {
                        SkeletonRidge exitRidge = minutia.Ridges[exit];
                        SkeletonRidge arm1 = minutia.Ridges[(exit + 1) % 3];
                        SkeletonRidge arm2 = minutia.Ridges[(exit + 2) % 3];
                        if (arm1.End == arm2.End && exitRidge.End != arm1.End && arm1.End != minutia && exitRidge.End != minutia)
                        {
                            SkeletonMinutia end = arm1.End;
                            if (end.Ridges.Count == 3 && arm1.Points.Count <= maxArmLength && arm2.Points.Count <= maxArmLength)
                            {
                                arm1.Detach();
                                arm2.Detach();
                                SkeletonRidge merged = new SkeletonRidge();
                                merged.Start = minutia;
                                merged.End = end;
                                foreach (Point point in MathEx.ConstructLine(minutia.Position, end.Position))
                                    merged.Points.Add(point);
                            }
                            break;
                        }
                    }
                }
            }
            RemoveKnots();
        }

        struct Gap
        {
            public SkeletonMinutia End1;
            public SkeletonMinutia End2;
        }

        void RemoveGaps()
        {
            const int minEndingLength = 7;
            PriorityQueueF<Gap> queue = new PriorityQueueF<Gap>();
            foreach (SkeletonMinutia end1 in Minutiae)
                if (end1.Ridges.Count == 1 && end1.Ridges[0].Points.Count >= minEndingLength)
                    foreach (SkeletonMinutia end2 in Minutiae)
                        if (end2 != end1 && end2.Ridges.Count == 1 && end1.Ridges[0].End != end2
                            && end2.Ridges[0].Points.Count >= minEndingLength && IsWithinGapLimits(end1, end2))
                        {
                            Gap gap;
                            gap.End1 = end1;
                            gap.End2 = end2;
                            queue.Enqueue((end1.Position - end2.Position).SqLength, gap);
                        }

            var shadow = GetShadow();
            while (queue.Count > 0)
            {
                Gap gap = queue.Dequeue();
                if (gap.End1.Ridges.Count == 1 && gap.End2.Ridges.Count == 1)
                {
                    Point[] line = MathEx.ConstructLine(gap.End1.Position, gap.End2.Position);
                    if (!IsRidgeOverlapping(line, shadow))
                        AddGapRidge(shadow, gap, line);
                }
            }

            RemoveKnots();
        }

        static bool IsWithinGapLimits(SkeletonMinutia end1, SkeletonMinutia end2)
        {
            const int ruptureSize = 5;
            const int gapSize = 20;
            const byte gapAngle = 32;

            int distanceSq = (end1.Position - end2.Position).SqLength;
            if (distanceSq <= MathEx.Sq(ruptureSize))
                return true;
            if (distanceSq > MathEx.Sq(gapSize))
                return false;

            byte gapDirection = Angle.AtanB(end1.Position, end2.Position);
            byte direction1 = Angle.AtanB(end1.Position, GetAngleSampleForGapRemoval(end1));
            if (Angle.Distance(direction1, Angle.Opposite(gapDirection)) > gapAngle)
                return false;
            byte direction2 = Angle.AtanB(end2.Position, GetAngleSampleForGapRemoval(end2));
            if (Angle.Distance(direction2, gapDirection) > gapAngle)
                return false;
            return true;
        }

        static Point GetAngleSampleForGapRemoval(SkeletonMinutia minutia)
        {
            const int angleSampleOffset = 22;
            SkeletonRidge ridge = minutia.Ridges[0];
            if (angleSampleOffset < ridge.Points.Count)
                return ridge.Points[angleSampleOffset];
            else
                return ridge.End.Position;
        }

        static bool IsRidgeOverlapping(Point[] line, bool[,] shadow)
        {
            const int toleratedOverlapLength = 2;
            for (int i = toleratedOverlapLength; i < line.Length - toleratedOverlapLength; ++i)
                if (line[i].Get(shadow))
                    return true;
            return false;
        }

        static void AddGapRidge(bool[,] shadow, Gap gap, Point[] line)
        {
            SkeletonRidge ridge = new SkeletonRidge();
            foreach (Point point in line)
                ridge.Points.Add(point);
            ridge.Start = gap.End1;
            ridge.End = gap.End2;
            foreach (Point point in line)
                point.Set(shadow, true);
        }

        void RemoveTails()
        {
            const int minTailLength = 21;
            foreach (var minutia in Minutiae)
            {
                if (minutia.Ridges.Count == 1 && minutia.Ridges[0].End.Ridges.Count >= 3)
                    if (minutia.Ridges[0].Points.Count < minTailLength)
                        minutia.Ridges[0].Detach();
            }
            RemoveDots();
            RemoveKnots();
        }

        void RemoveFragments()
        {
            const int minFragmentLength = 22;
            foreach (var minutia in Minutiae)
                if (minutia.Ridges.Count == 1)
                {
                    var ridge = minutia.Ridges[0];
                    if (ridge.End.Ridges.Count == 1 && ridge.Points.Count < minFragmentLength)
                        ridge.Detach();
                }
            RemoveDots();
        }

        void RemoveKnots()
        {
            foreach (var minutia in Minutiae)
            {
                if (minutia.Ridges.Count == 2 && minutia.Ridges[0].Reversed != minutia.Ridges[1])
                {
                    SkeletonRidge extended = minutia.Ridges[0].Reversed;
                    SkeletonRidge removed = minutia.Ridges[1];
                    if (extended.Points.Count < removed.Points.Count)
                    {
                        MathEx.Swap(ref extended, ref removed);
                        extended = extended.Reversed;
                        removed = removed.Reversed;
                    }

                    extended.Points.RemoveAt(extended.Points.Count - 1);
                    foreach (Point point in removed.Points)
                        extended.Points.Add(point);

                    extended.End = removed.End;
                    removed.Detach();
                }
            }
            RemoveDots();
        }

        void RemoveDots()
        {
            var removed = new List<SkeletonMinutia>();
            foreach (SkeletonMinutia minutia in Minutiae)
                if (minutia.Ridges.Count == 0)
                    removed.Add(minutia);
            foreach (SkeletonMinutia minutia in removed)
                RemoveMinutia(minutia);
        }

        void DisableBranchMinutiae()
        {
            foreach (var minutia in Minutiae)
                if (minutia.Ridges.Count > 2)
                    minutia.IsConsidered = false;
        }

        public void AddMinutia(SkeletonMinutia minutia)
        {
            Minutiae.Add(minutia);
        }

        public void RemoveMinutia(SkeletonMinutia minutia)
        {
            Minutiae.Remove(minutia);
        }

        bool[,] GetShadow()
        {
            var shadow = Size.Allocate<bool>();
            foreach (SkeletonMinutia minutia in Minutiae)
            {
                minutia.Position.Set(shadow, true);
                foreach (SkeletonRidge ridge in minutia.Ridges)
                    if (ridge.Start.Position.Y <= ridge.End.Position.Y)
                        foreach (Point point in ridge.Points)
                            point.Set(shadow, true);
            }
            return shadow;
        }
    }
}
