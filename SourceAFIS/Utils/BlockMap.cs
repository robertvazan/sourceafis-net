using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Utils
{
    class BlockMap
    {
        public class PointGrid
        {
            public readonly int[] AllX;
            public readonly int[] AllY;

            public PointGrid(Point size)
            {
                AllX = new int[size.X];
                AllY = new int[size.Y];
            }

            public Point this[int y, int x] { get { return new Point(AllX[x], AllY[y]); } }
            public Point this[Point at] { get { return new Point(AllX[at.X], AllY[at.Y]); } }
        }

        public class RectangleGrid
        {
            readonly PointGrid Corners;

            public RectangleGrid(PointGrid corners)
            {
                Corners = corners;
            }

            public Rectangle this[int y, int x] { get { return Rectangle.Between(Corners[y, x], Corners[y + 1, x + 1]); } }
            public Rectangle this[Point at] { get { return Rectangle.Between(Corners[at], Corners[at.Y + 1, at.X + 1]); } }
        }

        public readonly Point PixelCount;
        public readonly Point BlockCount;
        public readonly Point CornerCount;
        public readonly Rectangle AllBlocks;
        public readonly Rectangle AllCorners;
        public readonly PointGrid Corners;
        public readonly RectangleGrid BlockAreas;
        public readonly PointGrid BlockCenters;
        public readonly RectangleGrid CornerAreas;

        public BlockMap(Point pixelSize, int maxBlockSize)
        {
            PixelCount = pixelSize;
            BlockCount = new Point(
                MathEx.DivRoundUp(PixelCount.X, maxBlockSize),
                MathEx.DivRoundUp(PixelCount.Y, maxBlockSize));
            CornerCount = BlockToCornerCount(BlockCount);

            AllBlocks = new Rectangle(BlockCount);
            AllCorners = new Rectangle(CornerCount);

            Corners = InitCorners();
            BlockAreas = new RectangleGrid(Corners);
            BlockCenters = InitBlockCenters();
            CornerAreas = InitCornerAreas();
        }

        static Point BlockToCornerCount(Point BlockCount)
        {
            return new Point(BlockCount.X + 1, BlockCount.Y + 1);
        }

        static Point CornerToBlockCount(Point CornerCount)
        {
            return new Point(CornerCount.X - 1, CornerCount.Y - 1);
        }

        PointGrid InitCorners()
        {
            PointGrid grid = new PointGrid(CornerCount);
            for (int y = 0; y < CornerCount.Y; ++y)
                grid.AllY[y] = y * PixelCount.Y / BlockCount.Y;
            for (int x = 0; x < CornerCount.X; ++x)
                grid.AllX[x] = x * PixelCount.X / BlockCount.X;
            return grid;
        }

        PointGrid InitBlockCenters()
        {
            PointGrid grid = new PointGrid(BlockCount);
            for (int y = 0; y < BlockCount.Y; ++y)
                grid.AllY[y] = BlockAreas[y, 0].Center.Y;
            for (int x = 0; x < BlockCount.X; ++x)
                grid.AllX[x] = BlockAreas[0, x].Center.X;
            return grid;
        }

        RectangleGrid InitCornerAreas()
        {
            PointGrid grid = new PointGrid(new Point(CornerCount.X + 1, CornerCount.Y + 1));
            
            grid.AllY[0] = 0;
            for (int y = 0; y < BlockCount.Y; ++y)
                grid.AllY[y + 1] = BlockCenters[y, 0].Y;
            grid.AllY[BlockCount.Y + 1] = PixelCount.Y;

            grid.AllX[0] = 0;
            for (int x = 0; x < BlockCount.X; ++x)
                grid.AllX[x + 1] = BlockCenters[0, x].X;
            grid.AllX[BlockCount.X + 1] = PixelCount.X;

            return new RectangleGrid(grid);
        }
    }
}
