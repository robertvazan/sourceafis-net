using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.General
{
    public struct RangeF
    {
        public float Begin;
        public float End;

        public float Length { get { return End - Begin; } }

        public RangeF(float begin, float end)
        {
            Begin = begin;
            End = end;
        }

        public float GetFraction(float value)
        {
            return (value - Begin) / Length;
        }

        public float Interpolate(float fraction)
        {
            return Calc.Interpolate(Begin, End, fraction);
        }
    }
}
