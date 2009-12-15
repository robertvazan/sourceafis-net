using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;

namespace SourceAFIS.Extraction
{
    public sealed class ClippedContrast
    {
        public float ClipFraction = 0.1f;

        public byte[,] Compute(BlockMap blocks, short[, ,] histogram)
        {
            byte[,] result = new byte[blocks.CornerCount.Height, blocks.CornerCount.Width];
            Threader.Split<Point>(blocks.CornerList, delegate(Point corner)
            {
                int area = 0;
                for (int i = 0; i < 256; ++i)
                    area += histogram[corner.Y, corner.X, i];
                int clipLimit = Convert.ToInt32(area * ClipFraction);

                int accumulator = 0;
                int lowerBound = 255;
                for (int i = 0; i < 256; ++i)
                {
                    accumulator += histogram[corner.Y, corner.X, i];
                    if (accumulator > clipLimit)
                    {
                        lowerBound = i;
                        break;
                    }
                }

                accumulator = 0;
                int upperBound = 0;
                for (int i = 255; i >= 0; --i)
                {
                    accumulator += histogram[corner.Y, corner.X, i];
                    if (accumulator > clipLimit)
                    {
                        upperBound = i;
                        break;
                    }
                }
                
                result[corner.Y, corner.X] = (byte)(upperBound - lowerBound);
            });
            Logger.Log(this, result);
            return result;
        }
    }
}
