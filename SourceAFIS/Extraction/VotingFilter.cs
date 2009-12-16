using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;

namespace SourceAFIS.Extraction
{
    public sealed class VotingFilter
    {
        public int Radius = 1;
        public float Majority = 0.51f;

        public BinaryMap Filter(BinaryMap input)
        {
            BinaryMap output = new BinaryMap(input.Size);
            for (int y = 0; y < output.Height; ++y)
                for (int x = 0; x < output.Width; ++x)
                {
                    Range xRange = new Range(Math.Max(x - Radius, 0), Math.Min(x + Radius, output.Width - 1));
                    Range yRange = new Range(Math.Max(y - Radius, 0), Math.Min(y + Radius, output.Height - 1));
                    int ones = 0;
                    for (int ny = yRange.Begin; ny <= yRange.End; ++ny)
                        for (int nx = xRange.Begin; nx <= xRange.End; ++nx)
                            if (input.GetBit(nx, ny))
                                ++ones;

                    double voteWeight = 1.0 / (xRange.Length * yRange.Length);
                    if (ones * voteWeight >= Majority)
                        output.SetBitOne(x, y);
                }
            Logger.Log(this, output);
            return output;
        }
    }
}
