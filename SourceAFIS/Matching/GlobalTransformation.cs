using System;
using System.Collections.Generic;
using System.Text;
#if !COMPACT_FRAMEWORK
using System.Drawing;
#endif
using SourceAFIS.General;
using SourceAFIS.Dummy;
using SourceAFIS.Meta;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.Matching
{
    public sealed class GlobalTransformation
    {
        [Parameter(Upper = 100000)]
        public int MaxIterations = 10000;

        Random Random;

        public Transformation2D Compute(MinutiaPairing pairing, Template probe, Template candidate)
        {
            Random = new Random(0);
            Transformation2D bestTransformation = new Transformation2D();
            long bestDistance = MeasureDistance(pairing, probe, candidate, bestTransformation);
            Transformation2D triedTransformation = new Transformation2D();
            for (int i = 0; i < MaxIterations; ++i)
            {
                Mutate(bestTransformation, triedTransformation);
                long distance = MeasureDistance(pairing, probe, candidate, triedTransformation);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestTransformation.Assign(triedTransformation);
                }
            }
            return bestTransformation;
        }

        void Mutate(Transformation2D input, Transformation2D output)
        {
            output.Assign(input);
            switch (Random.Next(6))
            {
                case 0: output.RotatedX.X = Mutate(input.RotatedX.X); break;
                case 1: output.RotatedX.Y = Mutate(input.RotatedX.Y); break;
                case 2: output.RotatedY.X = Mutate(input.RotatedY.X); break;
                case 3: output.RotatedY.Y = Mutate(input.RotatedY.Y); break;
                case 4: output.Move.X = Mutate(input.Move.X); break;
                case 5: output.Move.Y = Mutate(input.Move.Y); break;
            }
        }

        float Mutate(float input)
        {
            switch (Random.Next(2))
            {
                case 0: return input * (GetSignedExpRandom() + 1);
                default: return input + GetSignedExpRandom();
            }
        }

        float GetSignedExpRandom()
        {
            if (Random.Next(2) == 0)
                return GetExpRandom();
            else
                return -GetExpRandom();
        }

        float GetExpRandom()
        {
            return (float)(Math.Pow(2, Random.NextDouble()) - 1);
        }

        long MeasureDistance(MinutiaPairing pairing, Template probe, Template candidate, Transformation2D transformation)
        {
            long sum = 0;
            for (int i = 0; i < pairing.Count; ++i)
            {
                MinutiaPair pair = pairing.GetPair(i);
                Point candidatePoint = candidate.Minutiae[pair.Candidate].Position;
                Point probePoint = probe.Minutiae[pair.Probe].Position;
                Point expectedPoint = transformation.Apply(candidatePoint);
                sum += Calc.DistanceSq(probePoint, expectedPoint);
            }
            return sum;
        }
    }
}
