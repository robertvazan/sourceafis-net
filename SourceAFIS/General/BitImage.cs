using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SourceAFIS.General
{
    public sealed class BitImage
    {
        public readonly int WordWidth;
        public readonly int Width;
        public readonly int Height;
        public Size Size { get { return new Size(Width, Height); } }
        public RectangleC Rect { get { return new RectangleC(Size); } }

        public const int WordShift = 5;
        public const uint WordMask = 31;
        public const int WordSize = 32;
        public const int WordBytes = WordSize / 8;

        readonly uint[,] Map;

        public BitImage(int width, int height)
        {
            Width = width;
            Height = height;
            WordWidth = (width + (int)WordMask) >> WordShift;
            Map = new uint[height, WordWidth];
        }

        public BitImage(Size size)
            : this(size.Width, size.Height)
        {
        }

        public BitImage(BitImage other)
        {
            Width = other.Width;
            Height = other.Height;
            WordWidth = other.WordWidth;
            Map = new uint[other.Map.GetLength(0), other.Map.GetLength(1)];
            for (int y = 0; y < Map.GetLength(0); ++y)
                for (int x = 0; x < Map.GetLength(1); ++x)
                    Map[y, x] = other.Map[y, x];
        }

        public bool IsWordNonZero(int xw, int y) { return Map[y, xw] != 0; }
        public bool GetBit(int x, int y) { return (Map[y, x >> WordShift] & (1u << (int)((uint)x & WordMask))) != 0; }
        public void SetBitOne(int x, int y) { Map[y, x >> WordShift] |= 1u << (int)((uint)x & WordMask); }
        public void SetBitZero(int x, int y) { Map[y, x >> WordShift] &= ~(1u << (int)((uint)x & WordMask)); }
        public uint GetWord(int xw, int y) { return Map[y, xw]; }

        public void SetBit(int x, int y, bool value)
        {
            if (value)
                SetBitOne(x, y);
            else
                SetBitZero(x, y);
        }

        public bool GetBitSafe(int x, int y, bool defaultValue)
        {
            if (Rect.Contains(new Point(x, y)))
                return GetBit(x, y);
            else
                return defaultValue;
        }

        public bool GetBit(Point at) { return GetBit(at.X, at.Y); }
        public void SetBitOne(Point at) { SetBitOne(at.X, at.Y); }
        public void SetBitZero(Point at) { SetBitZero(at.X, at.Y); }
        public bool GetBitSafe(Point at, bool defaultValue) { return GetBitSafe(at.X, at.Y, defaultValue); }

        public void Clear()
        {
            for (int y = 0; y < Map.GetLength(0); ++y)
                for (int x = 0; x < Map.GetLength(1); ++x)
                    Map[y, x] = 0;
        }

        public void Invert()
        {
            for (int y = 0; y < Map.GetLength(0); ++y)
                for (int x = 0; x < Map.GetLength(1); ++x)
                    Map[y, x] = ~Map[y, x];
            if (((uint)Width & WordMask) != 0u)
                for (int y = 0; y < Map.GetLength(0); ++y)
                    Map[y, Map.GetLength(1) - 1] &= ~0u >> (WordSize - (int)((uint)Width & WordMask));
        }

        public BitImage GetInverted()
        {
            BitImage result = new BitImage(Size);
            for (int y = 0; y < Map.GetLength(0); ++y)
                for (int x = 0; x < Map.GetLength(1); ++x)
                    result.Map[y, x] = ~Map[y, x];
            if (((uint)Width & WordMask) != 0u)
                for (int y = 0; y < Map.GetLength(0); ++y)
                    result.Map[y, Map.GetLength(1) - 1] &= ~0u >> (WordSize - (int)((uint)Width & WordMask));
            return result;
        }

        public bool IsEmpty()
        {
            for (int y = 0; y < Map.GetLength(0); ++y)
                for (int x = 0; x < Map.GetLength(1); ++x)
                    if (Map[y, x] != 0)
                        return false;
            return true;
        }

        static void ShiftLeft(uint[] vector, int shift)
        {
            if (shift > 0)
            {
                for (int i = 0; i < vector.Length - 1; ++i)
                    vector[i] = (vector[i] >> shift) | (vector[i + 1] << (WordSize - shift));
                vector[vector.Length - 1] >>= shift;
            }
        }

        static void ShiftRight(uint[] vector, int shift)
        {
            if (shift > 0)
            {
                for (int i = vector.Length - 1; i > 0; --i)
                    vector[i] = (vector[i] << shift) | (vector[i - 1] >> (WordSize - shift));
                vector[0] <<= shift;
            }
        }

        void LoadLine(uint[] vector, Point at, int length)
        {
            int lastX = at.X + length - 1;
            int words = (lastX >> WordShift) - (at.X >> WordShift) + 1;
            for (int i = 0; i < words; ++i)
                vector[i] = Map[at.Y, (at.X >> WordShift) + i];
            for (int i = words; i < vector.Length; ++i)
                vector[i] = 0;
        }

        void SaveLine(uint[] vector, Point at, int length)
        {
            int lastX = at.X + length - 1;
            int words = (lastX >> WordShift) - (at.X >> WordShift) + 1;
            for (int i = 1; i < words - 1; ++i)
                Map[at.Y, (at.X >> WordShift) + i] = vector[i];

            uint beginMask = ~0u << (int)((uint)at.X & WordMask);
            Map[at.Y, at.X >> WordShift] = Map[at.Y, at.X >> WordShift] & ~beginMask | vector[0] & beginMask;

            uint endMask = ~0u >> (int)(WordMask - ((uint)lastX & WordMask));
            Map[at.Y, lastX >> WordShift] = Map[at.Y, lastX >> WordShift] & ~endMask | vector[words - 1] & endMask;
        }

        delegate void CombineFunction(uint[] target, uint[] source);

        void Combine(BitImage source, RectangleC area, Point at, CombineFunction function)
        {
            int shift = (int)((uint)area.X & WordMask) - (int)((uint)at.X & WordMask);
            int vectorSize = (area.Width >> WordShift) + 2;
            var destination = new uint[vectorSize];
            var copy = new uint[vectorSize];
            for (int y = 0; y < area.Height; ++y)
            {
                LoadLine(destination, new Point(at.X, at.Y + y), area.Width);
                source.LoadLine(copy, new Point(area.X, area.Y + y), area.Width);
                if (shift >= 0)
                    ShiftLeft(copy, shift);
                else
                    ShiftRight(copy, -shift);
                function(destination, copy);
                SaveLine(destination, new Point(at.X, at.Y + y), area.Width);
            }
        }

        public void Copy(BitImage source)
        {
            Copy(source, Rect, new Point());
        }

        public void Copy(BitImage source, RectangleC area, Point at)
        {
            int shift = (int)((uint)area.X & WordMask) - (int)((uint)at.X & WordMask);
            var vector = new uint[(area.Width >> WordShift) + 2];
            for (int y = 0; y < area.Height; ++y)
            {
                source.LoadLine(vector, new Point(area.X, area.Y + y), area.Width);
                if (shift >= 0)
                    ShiftLeft(vector, shift);
                else
                    ShiftRight(vector, -shift);
                SaveLine(vector, new Point(at.X, at.Y + y), area.Width);
            }
        }

        public void Or(BitImage source)
        {
            Or(source, Rect, new Point());
        }

        public void Or(BitImage source, RectangleC area, Point at)
        {
            Combine(source, area, at, delegate(uint[] target, uint[] srcVector)
            {
                for (int i = 0; i < target.Length; ++i)
                    target[i] |= srcVector[i];
            });
        }

        public void And(BitImage source)
        {
            And(source, Rect, new Point());
        }

        public void And(BitImage source, RectangleC area, Point at)
        {
            Combine(source, area, at, delegate(uint[] target, uint[] srcVector)
            {
                for (int i = 0; i < target.Length; ++i)
                    target[i] &= srcVector[i];
            });
        }

        public void Xor(BitImage source)
        {
            Xor(source, Rect, new Point());
        }

        public void Xor(BitImage source, RectangleC area, Point at)
        {
            Combine(source, area, at, delegate(uint[] target, uint[] srcVector)
            {
                for (int i = 0; i < target.Length; ++i)
                    target[i] ^= srcVector[i];
            });
        }

        public void OrNot(BitImage source)
        {
            OrNot(source, Rect, new Point());
        }

        public void OrNot(BitImage source, RectangleC area, Point at)
        {
            Combine(source, area, at, delegate(uint[] target, uint[] srcVector)
            {
                for (int i = 0; i < target.Length; ++i)
                    target[i] |= ~srcVector[i];
            });
        }

        public void AndNot(BitImage source)
        {
            AndNot(source, Rect, new Point());
        }

        public void AndNot(BitImage source, RectangleC area, Point at)
        {
            Combine(source, area, at, delegate(uint[] target, uint[] srcVector)
            {
                for (int i = 0; i < target.Length; ++i)
                    target[i] &= ~srcVector[i];
            });
        }

        public uint GetNeighborhood(Point at) { return GetNeighborhood(at.X, at.Y); }

        public uint GetNeighborhood(int x, int y)
        {
            if ((x & WordMask) >= 1 && (x & WordMask) <= 30)
            {
                int xWord = x >> WordShift;
                int shift = (int)((uint)(x - 1) & WordMask);
                return ((Map[y + 1, xWord] >> shift) & 7u)
                    | (((Map[y, xWord] >> shift) & 1u) << 3)
                    | (((Map[y, xWord] >> shift) & 4u) << 2)
                    | (((Map[y - 1, xWord] >> shift) & 7u) << 5);
            }
            else
            {
                uint mask = 0;
                if (GetBit(x - 1, y + 1))
                    mask |= 1;
                if (GetBit(x, y + 1))
                    mask |= 2;
                if (GetBit(x + 1, y + 1))
                    mask |= 4;
                if (GetBit(x - 1, y))
                    mask |= 8;
                if (GetBit(x + 1, y))
                    mask |= 16;
                if (GetBit(x - 1, y - 1))
                    mask |= 32;
                if (GetBit(x, y - 1))
                    mask |= 64;
                if (GetBit(x + 1, y - 1))
                    mask |= 128;
                return mask;
            }
        }

        public void Fill(RectangleC rect)
        {
            if (rect.Width > 0)
            {
                int initialWord = (int)((uint)rect.Left >> WordShift);
                int finalWord = (rect.Right - 1) >> WordShift;
                int initialShift = (int)((uint)rect.Left & WordMask);
                int finalShift = 32 - (int)((uint)rect.Right & WordMask);
                for (int xw = initialWord; xw <= finalWord; ++xw)
                {
                    uint mask = ~0u;
                    if (xw == initialWord && initialShift != 0)
                        mask = mask << initialShift;
                    if (xw == finalWord && finalShift != WordSize)
                        mask = (mask << finalShift) >> finalShift;
                    for (int y = rect.Bottom; y < rect.Top; ++y)
                        Map[y, xw] |= mask;
                }
            }
        }

        public BitImage FillBlocks(BlockMap blocks)
        {
            BitImage result = new BitImage(blocks.PixelCount);
            for (int blockY = 0; blockY < blocks.BlockCount.Height; ++blockY)
            {
                for (int blockX = 0; blockX < blocks.BlockCount.Width; ++blockX)
                    if (GetBit(blockX, blockY))
                        result.Fill(blocks.BlockAreas[blockY, blockX]);
            }
            return result;
        }

        public BitImage FillCornerAreas(BlockMap blocks)
        {
            BitImage result = new BitImage(blocks.PixelCount);
            for (int cornerY = 0; cornerY < blocks.CornerCount.Height; ++cornerY)
                for (int cornerX = 0; cornerX < blocks.CornerCount.Width; ++cornerX)
                    if (GetBit(cornerX, cornerY))
                        result.Fill(blocks.CornerAreas[cornerY, cornerX]);
            return result;
        }
    }
}
