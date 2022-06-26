// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using SourceAFIS.Engine.Features;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Templates
{
    class PersistentTemplate
    {
        public string Version;
        public int Width;
        public int Height;
        public int[] PositionsX;
        public int[] PositionsY;
        public double[] Directions;
        public string Types;

        public PersistentTemplate() { }
        public PersistentTemplate(FeatureTemplate mutable)
        {
            Version = FingerprintCompatibility.Version;
            Width = mutable.Size.X;
            Height = mutable.Size.Y;
            int count = mutable.Minutiae.Count;
            PositionsX = new int[count];
            PositionsY = new int[count];
            Directions = new double[count];
            var chars = new char[count];
            for (int i = 0; i < count; ++i)
            {
                var minutia = mutable.Minutiae[i];
                PositionsX[i] = minutia.Position.X;
                PositionsY[i] = minutia.Position.Y;
                Directions[i] = minutia.Direction;
                chars[i] = minutia.Type == MinutiaType.Bifurcation ? 'B' : 'E';
            }
            Types = new string(chars);
        }

        public FeatureTemplate Mutable()
        {
            var minutiae = new List<Minutia>();
            for (int i = 0; i < Types.Length; ++i)
            {
                var type = Types[i] == 'B' ? MinutiaType.Bifurcation : MinutiaType.Ending;
                minutiae.Add(new(new(PositionsX[i], PositionsY[i]), Directions[i], type));
            }
            return new FeatureTemplate(new(Width, Height), minutiae);
        }
        public void Validate()
        {
            // Width and height are informative only. Don't validate them. Ditto for version string.
            if (PositionsX == null)
                throw new NullReferenceException("Null array of X positions.");
            if (PositionsY == null)
                throw new NullReferenceException("Null array of Y positions.");
            if (Directions == null)
                throw new NullReferenceException("Null array of minutia directions.");
            if (Types == null)
                throw new NullReferenceException("Null minutia type string.");
            if (PositionsX.Length != Types.Length || PositionsY.Length != Types.Length || Directions.Length != Types.Length)
                throw new ArgumentException("Inconsistent lengths of minutia property arrays.");
            for (int i = 0; i < Types.Length; ++i)
            {
                if (Math.Abs(PositionsX[i]) > 10_000 || Math.Abs(PositionsY[i]) > 10_000)
                    throw new ArgumentException("Minutia position out of range.");
                if (!DoubleAngle.Normalized(Directions[i]))
                    throw new ArgumentException("Denormalized minutia direction.");
                if (Types[i] != 'E' && Types[i] != 'B')
                    throw new ArgumentException("Unknown minutia type.");
            }
        }
    }
}
