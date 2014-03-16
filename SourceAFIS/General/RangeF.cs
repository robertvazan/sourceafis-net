using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.General
{
    public struct RangeF
    {
        public double Begin;
        public double End;

        public double Length { get { return End - Begin; } }

        public RangeF(double begin, double end)
        {
            Begin = begin;
            End = end;
        }

        public double GetFraction(double value)
        {
            return (value - Begin) / Length;
        }

        public double Interpolate(double fraction)
        {
            return Calc.Interpolate(Begin, End, fraction);
        }
    }
}
