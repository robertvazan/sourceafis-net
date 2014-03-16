using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.Utils;
using System.Collections.ObjectModel;

namespace SourceAFIS
{
    class SkeletonMinutia
    {
        public bool IsConsidered = true;
        public readonly Point Position;
        public readonly List<SkeletonRidge> Ridges = new List<SkeletonRidge>();

        public SkeletonMinutia(Point position)
        {
            Position = position;
        }

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
