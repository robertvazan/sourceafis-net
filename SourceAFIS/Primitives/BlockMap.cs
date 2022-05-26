// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

namespace SourceAFIS.Primitives
{
    class BlockMap
    {
        public readonly IntPoint Pixels;
        public readonly BlockGrid Primary;
        public readonly BlockGrid Secondary;
        public BlockMap(int width, int height, int maxBlockSize)
        {
            Pixels = new IntPoint(width, height);
            Primary = new BlockGrid(new IntPoint(
                Integers.RoundUpDiv(Pixels.X, maxBlockSize),
                Integers.RoundUpDiv(Pixels.Y, maxBlockSize)));
            for (int y = 0; y <= Primary.Blocks.Y; ++y)
                Primary.Y[y] = y * Pixels.Y / Primary.Blocks.Y;
            for (int x = 0; x <= Primary.Blocks.X; ++x)
                Primary.X[x] = x * Pixels.X / Primary.Blocks.X;
            Secondary = new BlockGrid(Primary.Corners);
            Secondary.Y[0] = 0;
            for (int y = 0; y < Primary.Blocks.Y; ++y)
                Secondary.Y[y + 1] = Primary.Block(0, y).Center.Y;
            Secondary.Y[Secondary.Blocks.Y] = Pixels.Y;
            Secondary.X[0] = 0;
            for (int x = 0; x < Primary.Blocks.X; ++x)
                Secondary.X[x + 1] = Primary.Block(x, 0).Center.X;
            Secondary.X[Secondary.Blocks.X] = Pixels.X;
        }
    }
}
