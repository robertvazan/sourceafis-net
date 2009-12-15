using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SourceAFIS.General
{
    public class BinaryMap
    {
        public readonly int WordWidth;
        public readonly int Width;
        public readonly int Height;
        public Size Size { get { return new Size(Width, Height); } }

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

        public bool GetBit(Point at) { return GetBit(at.X, at.Y); }
        public void SetBitOne(Point at) { SetBitOne(at.X, at.Y); }
        public void SetBitZero(Point at) { SetBitZero(at.X, at.Y); }
    }
}
