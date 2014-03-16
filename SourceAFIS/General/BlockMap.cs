using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.General
{
    public sealed class BlockMap
    {
        public sealed class PointGrid
        {
            public readonly int[] AllX;
            public readonly int[] AllY;

            public PointGrid(Size size)
            {
                AllX = new int[size.Width];
                AllY = new int[size.Height];
            }

            public Point this[int y, int x] { get { return new Point(AllX[x], AllY[y]); } }
            public Point this[Point at] { get { return new Point(AllX[at.X], AllY[at.Y]); } }
        }

        public sealed class RectangleGrid
        {
            readonly PointGrid Corners;

            public RectangleGrid(PointGrid corners)
            {
                Corners = corners;
            }

            public RectangleC this[int y, int x] { get { return new RectangleC(Corners[y, x], Corners[y + 1, x + 1]); } }
            public RectangleC this[Point at] { get { return new RectangleC(Corners[at], Corners[at.Y + 1, at.X + 1]); } }
        }

        public readonly Size PixelCount;
        public readonly Size BlockCount;
        public readonly Size CornerCount;
        public readonly RectangleC AllBlocks;
        public readonly RectangleC AllCorners;
        public readonly PointGrid Corners;
        public readonly RectangleGrid BlockAreas;
        public readonly PointGrid BlockCenters;
        public readonly RectangleGrid CornerAreas;

        public BlockMap(Size pixelSize, int maxBlockSize)
        {
            PixelCount = pixelSize;
            BlockCount = new Size(
                MathEx.DivRoundUp(PixelCount.Width, maxBlockSize),
                MathEx.DivRoundUp(PixelCount.Height, maxBlockSize));
            CornerCount = BlockToCornerCount(BlockCount);

            AllBlocks = new RectangleC(BlockCount);
            AllCorners = new RectangleC(CornerCount);

            Corners = InitCorners();
            BlockAreas = new RectangleGrid(Corners);
            BlockCenters = InitBlockCenters();
            CornerAreas = InitCornerAreas();
        }

        static Size BlockToCornerCount(Size BlockCount)
        {
            return new Size(BlockCount.Width + 1, BlockCount.Height + 1);
        }

        static Size CornerToBlockCount(Size CornerCount)
        {
            return new Size(CornerCount.Width - 1, CornerCount.Height - 1);
        }

        PointGrid InitCorners()
        {
            PointGrid grid = new PointGrid(CornerCount);
            for (int y = 0; y < CornerCount.Height; ++y)
                grid.AllY[y] = y * PixelCount.Height / BlockCount.Height;
            for (int x = 0; x < CornerCount.Width; ++x)
                grid.AllX[x] = x * PixelCount.Width / BlockCount.Width;
            return grid;
        }

        PointGrid InitBlockCenters()
        {
            PointGrid grid = new PointGrid(BlockCount);
            for (int y = 0; y < BlockCount.Height; ++y)
                grid.AllY[y] = BlockAreas[y, 0].Center.Y;
            for (int x = 0; x < BlockCount.Width; ++x)
                grid.AllX[x] = BlockAreas[0, x].Center.X;
            return grid;
        }

        RectangleGrid InitCornerAreas()
        {
            PointGrid grid = new PointGrid(new Size(CornerCount.Width + 1, CornerCount.Height + 1));
            
            grid.AllY[0] = 0;
            for (int y = 0; y < BlockCount.Height; ++y)
                grid.AllY[y + 1] = BlockCenters[y, 0].Y;
            grid.AllY[BlockCount.Height] = PixelCount.Height;

            grid.AllX[0] = 0;
            for (int x = 0; x < BlockCount.Width; ++x)
                grid.AllX[x + 1] = BlockCenters[0, x].X;
            grid.AllX[BlockCount.Width] = PixelCount.Width;

            return new RectangleGrid(grid);
        }
    }
}
