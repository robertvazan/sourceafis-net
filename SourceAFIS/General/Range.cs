using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.General
{
    public struct Range
    {
        public Range(int begin, int end)
        {
            Begin = begin;
            End = end;
        }

        public Range(int length)
        {
            Begin = 0;
            End = length;
        }

        public int Length { get { return End - Begin; } }

        public int Begin;
        public int End;

        public int Interpolate(int index, int count)
        {
            return MathEx.Interpolate(index, count, Length) + Begin;
        }
    }
}
