using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Model
{
    public sealed class RidgeTracer
    {
        public DetailLogger.Hook Logger = DetailLogger.Null;

        static bool[] IsMinutia = ConstructPixelClassifier();

        static bool[] ConstructPixelClassifier()
        {
            bool[] result = new bool[256];
            for (uint mask = 0; mask < 256; ++mask)
            {
                int count = Calc.CountBits(mask);
                result[mask] = (count == 1 || count > 2);
            }
            return result;
        }

        List<Point> FindMinutiae(BinaryMap binary)
        {
            List<Point> result = new List<Point>();
            for (int y = 0; y < binary.Height; ++y)
                for (int x = 0; x < binary.Width; ++x)
                    if (binary.GetBit(x, y) && IsMinutia[binary.GetNeighborhood(x, y)])
                        result.Add(new Point(x, y));
            return result;
        }

        Dictionary<Point, List<Point>> LinkNeighboringMinutiae(List<Point> minutiae)
        {
            Dictionary<Point, List<Point>> linking = new Dictionary<Point, List<Point>>();
            foreach (Point minutiaPos in minutiae)
            {
                List<Point> ownLinks = null;
                foreach (Point neighborRelative in Neighborhood.CornerNeighbors)
                {
                    Point neighborPos = Calc.Add(minutiaPos, neighborRelative);
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

        Dictionary<Point, SkeletonBuilder.Minutia> ComputeMinutiaCenters(Dictionary<Point, List<Point>> linking, SkeletonBuilder skeleton)
        {
            Dictionary<Point, SkeletonBuilder.Minutia> centers = new Dictionary<Point, SkeletonBuilder.Minutia>();
            foreach (Point currentPos in linking.Keys)
            {
                List<Point> linkedMinutiae = linking[currentPos];
                Point primaryPos = linkedMinutiae[0];
                if (!centers.ContainsKey(primaryPos))
                {
                    Point sum = new Point();
                    foreach (Point linkedPos in linkedMinutiae)
                        sum = Calc.Add(sum, linkedPos);
                    Point center = new Point(sum.X / linkedMinutiae.Count, sum.Y / linkedMinutiae.Count);
                    SkeletonBuilder.Minutia minutia = new SkeletonBuilder.Minutia(center);
                    skeleton.AddMinutia(minutia);
                    centers[primaryPos] = minutia;
                }
                centers[currentPos] = centers[primaryPos];
            }
            return centers;
        }

        void TraceRidges(BinaryMap binary, Dictionary<Point, SkeletonBuilder.Minutia> minutiaePoints)
        {
            Dictionary<Point, SkeletonBuilder.Ridge> leads = new Dictionary<Point, SkeletonBuilder.Ridge>();
            foreach (Point minutiaPoint in minutiaePoints.Keys)
            {
                foreach (Point startRelative in Neighborhood.CornerNeighbors)
                {
                    Point start = Calc.Add(minutiaPoint, startRelative);
                    if (binary.GetBitSafe(start, false) && !minutiaePoints.ContainsKey(start) && !leads.ContainsKey(start))
                    {
                        SkeletonBuilder.Ridge ridge = new SkeletonBuilder.Ridge();
                        ridge.Points.Add(minutiaPoint);
                        ridge.Points.Add(start);
                        Point previous = minutiaPoint;
                        Point current = start;
                        do
                        {
                            Point next = new Point();
                            foreach (Point nextRelative in Neighborhood.CornerNeighbors)
                            {
                                next = Calc.Add(current, nextRelative);
                                if (binary.GetBitSafe(next, false) && next != previous)
                                    break;
                            }
                            AssertException.Check(next != new Point());
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

        public void FixLinkingGaps(SkeletonBuilder skeleton)
        {
            foreach (SkeletonBuilder.Minutia minutia in skeleton.Minutiae)
            {
                foreach (SkeletonBuilder.Ridge ridge in minutia.Ridges)
                {
                    if (ridge.Points[0] != minutia.Position)
                    {
                        Point[] filling = Calc.ConstructLine(ridge.Points[0], minutia.Position);
                        for (int i = 1; i < filling.Length; ++i)
                            ridge.Reversed.Points.Add(filling[i]);
                    }
                }
            }
        }

        public void Trace(BinaryMap binary, SkeletonBuilder skeleton)
        {
            List<Point> minutiaPoints = FindMinutiae(binary);
            Dictionary<Point, List<Point>> linking = LinkNeighboringMinutiae(minutiaPoints);
            Dictionary<Point, SkeletonBuilder.Minutia> minutiaMap = ComputeMinutiaCenters(linking, skeleton);
            TraceRidges(binary, minutiaMap);
            FixLinkingGaps(skeleton);
            Logger.Log(skeleton);
        }
    }
}
