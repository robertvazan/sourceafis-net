using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Model
{
    public sealed class MinutiaMask
    {
        public DetailLogger.Hook Logger = DetailLogger.Null;

        public void Filter(SkeletonBuilder skeleton, BinaryMap mask)
        {
            foreach (SkeletonBuilder.Minutia minutia in skeleton.Minutiae)
            {
                if (!mask.GetBitSafe(minutia.Position, false))
                    minutia.Valid = false;
            }
            Logger.Log(skeleton);
        }
    }
}
