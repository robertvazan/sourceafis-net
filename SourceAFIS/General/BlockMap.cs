using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SourceAFIS.General
{
    public class BlockMap
    {
        public delegate void ForEachFunction(Point at);

        public Size PixelCount;
        public Size BlockCount;
        public Size CornerCount;
        public Point[,] Corners;
        public RectangleC[,] BlockAreas;
        public Point[,] BlockCenters;
        public RectangleC[,] CornerAreas;
        public Point[,] BlockList;
        public Point[,] CornerList;

        public void Initialize(int maxBlockSize)
        {
            BlockCount = new Size(
                Calc.DivRoundUp(PixelCount.Width, maxBlockSize),
                Calc.DivRoundUp(PixelCount.Height, maxBlockSize));
            CornerCount = BlockToCornerCount(BlockCount);
        }

        static Size BlockToCornerCount(Size BlockCount)
        {
            return new Size(BlockCount.Width + 1, BlockCount.Height + 1);
        }

        static Size CornerToBlockCount(Size CornerCount)
        {
            return new Size(CornerCount.Width - 1, CornerCount.Height - 1);
        }

        public void InitCorners()
        {
            if (Corners == null)
            {
                Corners = new Point[CornerCount.Height, CornerCount.Width];
                for (int y = 0; y < CornerCount.Height; ++y)
                    for (int x = 0; x < CornerCount.Width; ++x)
                    {
                        Corners[y, x].X = x * PixelCount.Width / BlockCount.Width;
                        Corners[y, x].Y = y * PixelCount.Width / BlockCount.Width;
                    }
            }
        }

        public void InitBlockAreas()
        {
            if (BlockAreas == null)
            {
                InitCorners();
                BlockAreas = new RectangleC[BlockCount.Height, BlockCount.Width];
                for (int y = 0; y < BlockCount.Height; ++y)
                    for (int x = 0; x < BlockCount.Width; ++x)
                    {
                        BlockAreas[y, x].X = Corners[y, x].X;
                        BlockAreas[y, x].Y = Corners[y, x].Y;
                        BlockAreas[y, x].Right = Corners[y, x + 1].X;
                        BlockAreas[y, x].Top = Corners[y + 1, x].Y;
                    }
            }
        }

        public void InitBlockCenters()
        {
            if (BlockCenters == null)
            {
                InitBlockAreas();
                BlockCenters = new Point[BlockCount.Height, BlockCount.Width];
                for (int y = 0; y < BlockCount.Height; ++y)
                    for (int x = 0; x < BlockCount.Width; ++x)
                        BlockCenters[y, x] = BlockAreas[y, x].Center;
            }
        }

        public void InitCornerAreas()
        {
            if (CornerAreas == null)
            {
                InitBlockCenters();
                CornerAreas = new RectangleC[CornerCount.Height, CornerCount.Width];
                for (int y = 0; y < CornerCount.Height; ++y)
                    for (int x = 0; x < CornerCount.Width; ++x)
                    {
                        if (x > 0)
                            CornerAreas[y, x].X = BlockCenters[0, x - 1].X;
                        else
                            CornerAreas[y, x].X = 0;
                        if (y > 0)
                            CornerAreas[y, x].Y = BlockCenters[y - 1, 0].Y;
                        else
                            CornerAreas[y, x].Y = 0;
                        if (x < BlockCount.Width)
                            CornerAreas[y, x].Right = BlockCenters[0, x].X;
                        else
                            CornerAreas[y, x].Right = PixelCount.Width;
                        if (y < BlockCount.Height)
                            CornerAreas[y, x].Top = BlockCenters[y, 0].Y;
                        else
                            CornerAreas[y, x].Top = PixelCount.Height;
                    }
            }
        }

        public void InitBlockList()
        {
            if (BlockList == null)
            {
                BlockList = new Point[BlockCount.Height, BlockCount.Width];
                for (int y = 0; y < BlockCount.Height; ++y)
                    for (int x = 0; x < BlockCount.Width; ++x)
                        BlockList[y, x] = new Point(x, y);
            }
        }

        public void InitCornerList()
        {
            if (CornerList == null)
            {
                CornerList = new Point[CornerCount.Height, CornerCount.Width];
                for (int y = 0; y < CornerCount.Height; ++y)
                    for (int x = 0; x < CornerCount.Width; ++x)
                        CornerList[y, x] = new Point(x, y);
            }
        }
    }
}
