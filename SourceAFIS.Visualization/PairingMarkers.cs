using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;
using SourceAFIS.Matching.Minutia;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Visualization
{
    public static class PairingMarkers
    {
        static Point[] Circle = CircleDrawer.Draw(5);

        public static void DrawProbe(ColorF[,] output, MinutiaPairing pairing, Template probe)
        {
            for (int i = 0; i < pairing.Count; ++i)
            {
                MinutiaPair pair = pairing.GetPair(i);
                Point point = probe.Minutiae[pair.Probe].Position;
                CircleDrawer.Paint(Circle, point, output, ColorF.Red);
            }
        }

        public static void DrawCandidate(ColorF[,] output, MinutiaPairing pairing, Template candidate, Transformation2D transformation)
        {
            for (int i = 0; i < pairing.Count; ++i)
            {
                MinutiaPair pair = pairing.GetPair(i);
                Point point = transformation.Apply(candidate.Minutiae[pair.Candidate].Position);
                CircleDrawer.Paint(Circle, point, output, ColorF.Red);
            }
        }
    }
}
