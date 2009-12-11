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

        public int Length { get { return End - Begin; } }

        public int Begin;
        public int End;

        public int Interpolate(int index, int count)
        {
            return index * Length / count + Begin;
        }
    }
}
