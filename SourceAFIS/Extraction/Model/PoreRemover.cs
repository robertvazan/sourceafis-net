using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;
using SourceAFIS.Meta;

namespace SourceAFIS.Extraction.Model
{
    public sealed class PoreRemover : ISkeletonFilter
    {
        [DpiAdjusted]
        [Parameter(Lower = 3, Upper = 100)]
        public int MaxArmLength = 45;

        [Nested]
        public KnotRemover KnotRemover = new KnotRemover();

        public DetailLogger.Hook Logger = DetailLogger.Null;

        public void Filter(SkeletonBuilder skeleton)
        {
            foreach (SkeletonBuilder.Minutia minutia in skeleton.Minutiae)
            {
                if (minutia.Ridges.Count == 3)
                {
                    for (int exit = 0; exit < 3; ++exit)
                    {
                        SkeletonBuilder.Ridge exitRidge = minutia.Ridges[exit];
                        SkeletonBuilder.Ridge arm1 = minutia.Ridges[(exit + 1) % 3];
                        SkeletonBuilder.Ridge arm2 = minutia.Ridges[(exit + 2) % 3];
                        if (arm1.End == arm2.End && exitRidge.End != arm1.End && arm1.End != minutia && exitRidge.End != minutia)
                        {
                            SkeletonBuilder.Minutia end = arm1.End;
                            if (end.Ridges.Count == 3 && arm1.Points.Count <= MaxArmLength && arm2.Points.Count <= MaxArmLength)
                            {
                                arm1.Detach();
                                arm2.Detach();
                                SkeletonBuilder.Ridge merged = new SkeletonBuilder.Ridge();
                                merged.Start = minutia;
                                merged.End = end;
                                foreach (Point point in Calc.ConstructLine(minutia.Position, end.Position))
                                    merged.Points.Add(point);
                            }
                            break;
                        }
                    }
                }
            }
            KnotRemover.Filter(skeleton);
            Logger.Log(skeleton);
        }
    }
}
