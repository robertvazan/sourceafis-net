using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Meta;

namespace SourceAFIS.Extraction.Filters
{
    public sealed class AbsoluteContrast
    {
        [Parameter(Upper = 255)]
        public int Limit = 15;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        public BinaryMap DetectLowContrast(byte[,] contrast)
        {
            BinaryMap result = new BinaryMap(contrast.GetLength(1), contrast.GetLength(0));
            for (int y = 0; y < result.Height; ++y)
                for (int x = 0; x < result.Width; ++x)
                    if (contrast[y, x] < Limit)
                        result.SetBitOne(x, y);
            Logger.Log(result);
            return result;
        }
    }
}
