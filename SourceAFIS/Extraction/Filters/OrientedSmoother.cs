using System;
using System.Collections.Generic;
using System.Text;
#if !COMPACT_FRAMEWORK
using System.Threading.Tasks;
#endif
using SourceAFIS.General;
using SourceAFIS.Meta;

namespace SourceAFIS.Extraction.Filters
{
    public sealed class OrientedSmoother
    {
        public byte AngleOffset;
        [Nested]
        public LinesByOrientation Lines = new LinesByOrientation();

        public DetailLogger.Hook Logger = DetailLogger.Null;

        public float[,] Smooth(float[,] input, byte[,] orientation, BinaryMap mask, BlockMap blocks)
        {
            Point[][] lines = Lines.Construct();
            float[,] output = new float[input.GetLength(0), input.GetLength(1)];
            Parallel.ForEach(blocks.AllBlocks, delegate(Point block)
            {
                if (mask.GetBit(block))
                {
                    Point[] line = lines[Angle.Quantize(Angle.Add(orientation[block.Y, block.X], AngleOffset), lines.Length)];
                    foreach (Point linePoint in line)
                    {
                        RectangleC target = blocks.BlockAreas[block];
                        RectangleC source = target.GetShifted(linePoint);
                        source.Clip(new RectangleC(blocks.PixelCount));
                        target = source.GetShifted(Calc.Negate(linePoint));
                        for (int y = target.Bottom; y < target.Top; ++y)
                            for (int x = target.Left; x < target.Right; ++x)
                                output[y, x] += input[y + linePoint.Y, x + linePoint.X];
                    }
                    RectangleC blockArea = blocks.BlockAreas[block];
                    for (int y = blockArea.Bottom; y < blockArea.Top; ++y)
                        for (int x = blockArea.Left; x < blockArea.Right; ++x)
                            output[y, x] *= 1f / line.Length;
                }
            });
            Logger.Log(output);
            return output;
        }
    }
}
