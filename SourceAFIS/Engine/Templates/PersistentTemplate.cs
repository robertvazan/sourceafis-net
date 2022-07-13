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
        public short Width;
        public short Height;
        public short[] PositionsX;
        public short[] PositionsY;
        public float[] Directions;
        public string Types;

        public PersistentTemplate() { }
        public PersistentTemplate(FeatureTemplate template)
        {
            Version = FingerprintCompatibility.Version + "-net";
            Width = template.Size.X;
            Height = template.Size.Y;
            int count = template.Minutiae.Count;
            PositionsX = new short[count];
            PositionsY = new short[count];
            Directions = new float[count];
            var chars = new char[count];
            for (int i = 0; i < count; ++i)
            {
                var minutia = template.Minutiae[i];
                PositionsX[i] = minutia.Position.X;
                PositionsY[i] = minutia.Position.Y;
                Directions[i] = minutia.Direction;
                chars[i] = minutia.Type == MinutiaType.Bifurcation ? 'B' : 'E';
            }
            Types = new string(chars);
        }

        public FeatureTemplate Decode()
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
                if (!FloatAngle.Normalized(Directions[i]))
                    throw new ArgumentException("Denormalized minutia direction.");
                if (Types[i] != 'E' && Types[i] != 'B')
                    throw new ArgumentException("Unknown minutia type.");
            }
        }
    }
}
