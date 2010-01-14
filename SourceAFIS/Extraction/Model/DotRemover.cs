using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Model
{
    public sealed class DotRemover : ISkeletonFilter
    {
        public void Filter(SkeletonBuilder skeleton)
        {
            List<SkeletonBuilder.Minutia> minutiae = new List<SkeletonBuilder.Minutia>(skeleton.Minutiae);
            foreach (SkeletonBuilder.Minutia minutia in minutiae)
                if (minutia.Ridges.Count == 0)
                    skeleton.RemoveMinutia(minutia);
            Logger.Log(this, skeleton);
        }
    }
}
