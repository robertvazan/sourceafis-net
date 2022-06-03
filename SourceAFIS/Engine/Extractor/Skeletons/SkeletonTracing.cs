// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using System.Linq;
using SourceAFIS.Engine.Features;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Extractor.Skeletons
{
    static class SkeletonTracing
    {
        static List<IntPoint> FindMinutiae(BooleanMatrix thinned)
        {
            var result = new List<IntPoint>();
            foreach (var at in thinned.Size.Iterate())
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
        static Dictionary<IntPoint, SkeletonMinutia> MinutiaCenters(Skeleton skeleton, Dictionary<IntPoint, List<IntPoint>> linking)
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
                    skeleton.AddMinutia(minutia);
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
        static void FixLinkingGaps(Skeleton skeleton)
        {
            foreach (var minutia in skeleton.Minutiae)
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
        public static Skeleton Trace(BooleanMatrix thinned, SkeletonType type)
        {
            var skeleton = new Skeleton(type, thinned.Size);
            var minutiaPoints = FindMinutiae(thinned);
            var linking = LinkNeighboringMinutiae(minutiaPoints);
            var minutiaMap = MinutiaCenters(skeleton, linking);
            TraceRidges(thinned, minutiaMap);
            FixLinkingGaps(skeleton);
            // https://sourceafis.machinezoo.com/transparency/traced-skeleton
            FingerprintTransparency.Current.LogSkeleton("traced-skeleton", skeleton);
            return skeleton;
        }
    }
}
