// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

namespace SourceAFIS.Primitives
{
    class BlockGrid
    {
        public readonly IntPoint Blocks;
        public readonly IntPoint Corners;
        public readonly int[] X;
        public readonly int[] Y;

        public BlockGrid(IntPoint size)
        {
            Blocks = size;
            Corners = new IntPoint(size.X + 1, size.Y + 1);
            X = new int[size.X + 1];
            Y = new int[size.Y + 1];
        }
        public BlockGrid(int width, int height) : this(new IntPoint(width, height)) { }

        public IntPoint Corner(int atX, int atY) => new IntPoint(X[atX], Y[atY]);
        public IntPoint Corner(IntPoint at) => Corner(at.X, at.Y);
        public IntRect Block(int atX, int atY) => IntRect.Between(Corner(atX, atY), Corner(atX + 1, atY + 1));
        public IntRect Block(IntPoint at) => Block(at.X, at.Y);
    }
}
