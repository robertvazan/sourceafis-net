using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Visualization
{
    public static class ImageDiff
    {
        public static float[,] Diff(float[,] initial, float[,] updated)
        {
            float[,] output = new float[initial.GetLength(0), initial.GetLength(1)];
            for (int y = 0; y < initial.GetLength(0); ++y)
                for (int x = 0; x < initial.GetLength(1); ++x)
                    output[y, x] = updated[y, x] - initial[y, x];
            return output;
        }

        public static ColorF[,] Render(float[,] diff)
        {
            ColorF[,] output = new ColorF[diff.GetLength(0), diff.GetLength(1)];
            for (int y = 0; y < diff.GetLength(0); ++y)
                for (int x = 0; x < diff.GetLength(1); ++x)
                    if (diff[y, x] >= 0)
                        output[y, x] = new ColorF(0, 1, 0, diff[y, x]);
                    else
                        output[y, x] = new ColorF(1, 0, 0, -diff[y, x]);
            return output;
        }

        public static float[,] Normalize(float[,] diff, float maxFactor)
        {
            float max = 1f / maxFactor;
            for (int y = 0; y < diff.GetLength(0); ++y)
                for (int x = 0; x < diff.GetLength(1); ++x)
                    max = Math.Max(max, Math.Abs(diff[y, x]));
            float[,] output = new float[diff.GetLength(0), diff.GetLength(1)];
            for (int y = 0; y < diff.GetLength(0); ++y)
                for (int x = 0; x < diff.GetLength(1); ++x)
                    output[y, x] = diff[y, x] * (1f / max);
            return output;
        }

        public static float[,] Binarize(float[,] diff, float zeroThreshold, float saturation)
        {
            float[,] output = new float[diff.GetLength(0), diff.GetLength(1)];
            for (int y = 0; y < diff.GetLength(0); ++y)
                for (int x = 0; x < diff.GetLength(1); ++x)
                    if (diff[y, x] >= zeroThreshold)
                        output[y, x] = saturation;
                    else if (diff[y, x] <= -zeroThreshold)
                        output[y, x] = -saturation;
            return output;
        }
    }
}
