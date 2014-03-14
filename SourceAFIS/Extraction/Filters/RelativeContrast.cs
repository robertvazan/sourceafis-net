using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Filters
{
    public static class RelativeContrast
    {
        const int SampleSize = 168568;
        const float SampleFraction = 0.49f;
        const float RelativeLimit = 0.34f;

        public static BinaryMap DetectLowContrast(byte[,] contrast, BlockMap blocks)
        {
            List<byte> sortedContrast = new List<byte>();
            foreach (byte contrastItem in contrast)
                sortedContrast.Add(contrastItem);
            sortedContrast.Sort();
            sortedContrast.Reverse();

            int pixelsPerBlock = Calc.GetArea(blocks.PixelCount) / blocks.AllBlocks.TotalArea;
            int sampleCount = Math.Min(sortedContrast.Count, SampleSize / pixelsPerBlock);
            int consideredBlocks = Math.Max(Convert.ToInt32(sampleCount * SampleFraction), 1);
            
            int averageContrast = 0;
            for (int i = 0; i < consideredBlocks; ++i)
                averageContrast += sortedContrast[i];
            averageContrast /= consideredBlocks;
            byte limit = Convert.ToByte(averageContrast * RelativeLimit);

            BinaryMap result = new BinaryMap(blocks.BlockCount.Width, blocks.BlockCount.Height);
            for (int y = 0; y < result.Height; ++y)
                for (int x = 0; x < result.Width; ++x)
                    if (contrast[y, x] < limit)
                        result.SetBitOne(x, y);
            return result;
        }
    }
}
