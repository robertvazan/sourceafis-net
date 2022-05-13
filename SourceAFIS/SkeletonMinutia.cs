// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;

namespace SourceAFIS
{
    class SkeletonMinutia
    {
        public readonly IntPoint Position;
        public readonly List<SkeletonRidge> Ridges = new List<SkeletonRidge>();

        public SkeletonMinutia(IntPoint position) { Position = position; }

        public void AttachStart(SkeletonRidge ridge)
        {
            if (!Ridges.Contains(ridge))
            {
                Ridges.Add(ridge);
                ridge.Start = this;
            }
        }
        public void DetachStart(SkeletonRidge ridge)
        {
            if (Ridges.Contains(ridge))
            {
                Ridges.Remove(ridge);
                if (ridge.Start == this)
                    ridge.Start = null;
            }
        }
    }
}
