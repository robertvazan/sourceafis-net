using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Model
{
    public static class KnotRemover
    {
        public static void Filter(SkeletonBuilder skeleton)
        {
            foreach (SkeletonBuilder.Minutia minutia in skeleton.Minutiae)
            {
                if (minutia.Ridges.Count == 2 && minutia.Ridges[0].Reversed != minutia.Ridges[1])
                {
                    SkeletonBuilder.Ridge extended = minutia.Ridges[0].Reversed;
                    SkeletonBuilder.Ridge removed = minutia.Ridges[1];
                    if (extended.Points.Count < removed.Points.Count)
                    {
                        Calc.Swap(ref extended, ref removed);
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
            DotRemover.Filter(skeleton);
        }
    }
}
