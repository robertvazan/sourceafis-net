using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Extraction.Model
{
    public interface ISkeletonFilter
    {
        void Filter(SkeletonBuilder skeleton);
    }
}
