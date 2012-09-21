using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using SourceAFIS.General;
using SourceAFIS.Meta;

namespace SourceAFIS.Extraction.Filters
{
    public sealed class HillOrientation
    {
        [DpiAdjusted]
        [Parameter(Lower = 0.5, Upper = 4)]
        public float MinHalfDistance = 2;
        [DpiAdjusted]
        [Parameter(Lower = 5, Upper = 13)]
        public float MaxHalfDistance = 6;
        [Parameter(Upper = 100)]
        public int NeighborListSplit = 50;
        [Parameter(Upper = 100)]
        public int NeighborsChecked = 20;
        [Parameter(Upper = 3)]
        public int SmoothingRadius = 1;

        public DetailLogger.Hook Logger = DetailLogger.Null;

        struct NeighborInfo
        {
            public Point Position;
            public PointF Orientation;
        }

        List<List<NeighborInfo>> PrepareNeighbors()
        {
            Random random = new Random(0);
            List<List<NeighborInfo>> allSplits = new List<List<NeighborInfo>>();
            for (int i = 0; i < NeighborListSplit; ++i)
            {
                List<NeighborInfo> neighbors = new List<NeighborInfo>();
                for (int j = 0; j < NeighborsChecked; ++j)
                {
                    NeighborInfo neighbor = new NeighborInfo();
                    do
                    {
                        float angle = Angle.FromFraction((float)random.NextDouble() * 0.5f);
                        float distance = Calc.InterpolateExponential(MinHalfDistance, MaxHalfDistance, (float)random.NextDouble());
                        neighbor.Position = Calc.Round(Calc.Multiply(distance, Angle.ToVector(angle)));
                    } while (neighbor.Position == new Point() || neighbor.Position.Y < 0);
                    neighbor.Orientation = Angle.ToVector(Angle.Add(Angle.ToOrientation(Angle.Atan(neighbor.Position)), Angle.PI));
                    if (!neighbors.Any(info => info.Position == neighbor.Position))
                        neighbors.Add(neighbor);
                }
                neighbors.Sort((left, right) => Calc.CompareYX(left.Position, right.Position));
                allSplits.Add(neighbors);
            }
            return allSplits;
        }

        Range GetMaskLineRange(BinaryMap mask, int y)
        {
            int first = -1;
            int last = -1;
            for (int x = 0; x < mask.Width; ++x)
                if (mask.GetBit(x, y))
                {
                    last = x;
                    if (first < 0)
                        first = x;
                }
            if (first >= 0)
                return new Range(first, last + 1);
            else
                return new Range();
        }

        PointF[,] AccumulateOrientations(float[,] input, BinaryMap mask, BlockMap blocks)
        {
            List<List<NeighborInfo>> neighbors = PrepareNeighbors();

            PointF[,] orientation = new PointF[input.GetLength(0), input.GetLength(1)];
            Parallel.For(0, mask.Height, delegate(int blockY)
            {
                Range validMaskRange = GetMaskLineRange(mask, blockY);
                if (validMaskRange.Length > 0)
                {
                    Range validXRange = new Range(blocks.BlockAreas[blockY, validMaskRange.Begin].Left,
                        blocks.BlockAreas[blockY, validMaskRange.End - 1].Right);
                    for (int y = blocks.BlockAreas[blockY, 0].Bottom; y < blocks.BlockAreas[blockY, 0].Top; ++y)
                    {
                        foreach (NeighborInfo neighbor in neighbors[y % neighbors.Count])
                        {
                            int radius = Math.Max(Math.Abs(neighbor.Position.X), Math.Abs(neighbor.Position.Y));
                            if (y - radius >= 0 && y + radius < input.GetLength(0))
                            {
                                Range xRange = new Range(Math.Max(radius, validXRange.Begin),
                                    Math.Min(input.GetLength(1) - radius, validXRange.End));
                                for (int x = xRange.Begin; x < xRange.End; ++x)
                                {
                                    float before = input[y - neighbor.Position.Y, x - neighbor.Position.X];
                                    float at = input[y, x];
                                    float after = input[y + neighbor.Position.Y, x + neighbor.Position.X];
                                    float strength = at - Math.Max(before, after);
                                    if (strength > 0)
                                        orientation[y, x] = Calc.Add(orientation[y, x], Calc.Multiply(strength, neighbor.Orientation));
                                }
                            }
                        }
                    }
                }
            });
            Logger.Log("Raw", orientation);
            return orientation;
        }

        PointF[,] SumBlocks(PointF[,] orientation, BlockMap blocks, BinaryMap mask)
        {
            PointF[,] sums = new PointF[blocks.BlockCount.Height, blocks.BlockCount.Width];
            Parallel.ForEach(blocks.AllBlocks, delegate(Point block)
            {
                if (mask.GetBit(block))
                {
                    PointF sum = new PointF();
                    RectangleC area = blocks.BlockAreas[block];
                    for (int y = area.Bottom; y < area.Top; ++y)
                        for (int x = area.Left; x < area.Right; ++x)
                            sum = Calc.Add(sum, orientation[y, x]);
                    sums[block.Y, block.X] = sum;
                }
            });
            return sums;
        }

        PointF[,] Smooth(PointF[,] orientation, BinaryMap mask)
        {
            PointF[,] smoothed = new PointF[mask.Height, mask.Width];
            Parallel.For(0, mask.Height, delegate(int y)
            {
                for (int x = 0; x < mask.Width; ++x)
                    if (mask.GetBit(x, y))
                    {
                        RectangleC neighbors = new RectangleC(
                            new Point(Math.Max(0, x - SmoothingRadius), Math.Max(0, y - SmoothingRadius)),
                            new Point(Math.Min(mask.Width, x + SmoothingRadius + 1), Math.Min(mask.Height, y + SmoothingRadius + 1)));
                        PointF sum = new PointF();
                        for (int ny = neighbors.Bottom; ny < neighbors.Top; ++ny)
                            for (int nx = neighbors.Left; nx < neighbors.Right; ++nx)
                                if (mask.GetBit(nx, ny))
                                    sum = Calc.Add(sum, orientation[ny, nx]);
                        smoothed[y, x] = sum;
                    }
            });
            return smoothed;
        }

        byte[,] ToAngles(PointF[,] vectors, BinaryMap mask)
        {
            byte[,] angles = new byte[mask.Height, mask.Width];
            Parallel.For(0, mask.Height, delegate(int y)
            {
                for (int x = 0; x < mask.Width; ++x)
                    if (mask.GetBit(x, y))
                        angles[y, x] = Angle.ToByte(Angle.Atan(vectors[y, x]));
            });
            return angles;
        }

        public byte[,] Detect(float[,] image, BinaryMap mask, BlockMap blocks)
        {
            PointF[,] accumulated = AccumulateOrientations(image, mask, blocks);
            PointF[,] byBlock = SumBlocks(accumulated, blocks, mask);
            PointF[,] smooth = Smooth(byBlock, mask);
            byte[,] angles = ToAngles(smooth, mask);
            Logger.Log(angles);
            return angles;
        }
    }
}
