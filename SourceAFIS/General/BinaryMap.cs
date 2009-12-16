using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SourceAFIS.General
{
    public sealed class BinaryMap
    {
        public readonly int WordWidth;
        public readonly int Width;
        public readonly int Height;
        public Size Size { get { return new Size(Width, Height); } }
        public RectangleC Rect { get { return new RectangleC(new Point(0, 0), Size); } }

        public const int WordShift = 5;
        public const uint WordMask = 31;
        public const int WordSize = 32;

        readonly uint[,] Map;

        public BinaryMap(int width, int height)
        {
            Width = width;
            Height = height;
            WordWidth = (width + (int)WordMask) >> WordShift;
            Map = new uint[height, WordWidth];
        }

        public BinaryMap(Size size)
            : this(size.Width, size.Height)
        {
        }

        public BinaryMap(BinaryMap other)
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

        public void Invert()
        {
            for (int y = 0; y < Map.GetLength(0); ++y)
                for (int x = 0; x < Map.GetLength(1); ++x)
                    Map[y, x] = ~Map[y, x];
            if (((uint)Width & WordMask) != 0u)
                for (int y = 0; y < Map.GetLength(0); ++y)
                    Map[y, Map.GetLength(1) - 1] &= ~0u >> (WordSize - (int)((uint)Width & WordMask));
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

        void Combine(BinaryMap source, Rectangle area, Point at, CombineFunction function)
        {
            uint[] vector = new uint[(area.Width >> WordShift) + 2];
            uint[] srcVector = new uint[vector.Length];
            for (int y = 0; y < area.Height; ++y)
            {
                LoadLine(vector, new Point(at.X, at.Y + y), area.Width);
                source.LoadLine(srcVector, new Point(area.X, area.Y + y), area.Width);
                int shift = (int)((uint)area.X & WordMask) - (int)((uint)at.X & WordMask);
                if (shift >= 0)
                    ShiftLeft(srcVector, shift);
                else
                    ShiftRight(srcVector, -shift);
                function(vector, srcVector);
                SaveLine(vector, new Point(at.X, at.Y + y), area.Width);
            }
        }

        public void Copy(BinaryMap source)
        {
            Copy(source, new Rectangle(0, 0, Width, Height), new Point(0, 0));
        }

        public void Copy(BinaryMap source, Rectangle area, Point at)
        {
            uint[] vector = new uint[(area.Width >> WordShift) + 2];
            for (int y = 0; y < area.Height; ++y)
            {
                source.LoadLine(vector, new Point(area.X, area.Y + y), area.Width);
                int shift = (int)((uint)area.X & WordMask) - (int)((uint)at.X & WordMask);
                if (shift >= 0)
                    ShiftLeft(vector, shift);
                else
                    ShiftRight(vector, -shift);
                SaveLine(vector, new Point(at.X, at.Y + y), area.Width);
            }
        }

        public void Or(BinaryMap source)
        {
            Or(source, new Rectangle(0, 0, Width, Height), new Point(0, 0));
        }

        public void Or(BinaryMap source, Rectangle area, Point at)
        {
            Combine(source, area, at, delegate(uint[] target, uint[] srcVector)
            {
                for (int i = 0; i < target.Length; ++i)
                    target[i] |= srcVector[i];
            });
        }

        public void And(BinaryMap source)
        {
            And(source, new Rectangle(0, 0, Width, Height), new Point(0, 0));
        }

        public void And(BinaryMap source, Rectangle area, Point at)
        {
            Combine(source, area, at, delegate(uint[] target, uint[] srcVector)
            {
                for (int i = 0; i < target.Length; ++i)
                    target[i] &= srcVector[i];
            });
        }

        public void Xor(BinaryMap source)
        {
            Xor(source, new Rectangle(0, 0, Width, Height), new Point(0, 0));
        }

        public void Xor(BinaryMap source, Rectangle area, Point at)
        {
            Combine(source, area, at, delegate(uint[] target, uint[] srcVector)
            {
                for (int i = 0; i < target.Length; ++i)
                    target[i] ^= srcVector[i];
            });
        }

        public void OrNot(BinaryMap source)
        {
            OrNot(source, new Rectangle(0, 0, Width, Height), new Point(0, 0));
        }

        public void OrNot(BinaryMap source, Rectangle area, Point at)
        {
            Combine(source, area, at, delegate(uint[] target, uint[] srcVector)
            {
                for (int i = 0; i < target.Length; ++i)
                    target[i] |= ~srcVector[i];
            });
        }

        public void AndNot(BinaryMap source)
        {
            AndNot(source, new Rectangle(0, 0, Width, Height), new Point(0, 0));
        }

        public void AndNot(BinaryMap source, Rectangle area, Point at)
        {
            Combine(source, area, at, delegate(uint[] target, uint[] srcVector)
            {
                for (int i = 0; i < target.Length; ++i)
                    target[i] &= ~srcVector[i];
            });
        }
    }
}
