using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Filters
{
    public sealed class VotingFilter
    {
        readonly int Radius;
        readonly float Majority;
        readonly int BorderDistance;

        public VotingFilter(int radius = 1, float majority = 0.51f, int borderDist = 0)
        {
            Radius = radius;
            Majority = majority;
            BorderDistance = borderDist;
        }

        public BinaryMap Filter(BinaryMap input)
        {
            RectangleC rect = new RectangleC(new Point(BorderDistance, BorderDistance),
                new Size(input.Width - 2 * BorderDistance, input.Height - 2 * BorderDistance));
            BinaryMap output = new BinaryMap(input.Size);
            for (int y = rect.RangeY.Begin; y < rect.RangeY.End; ++y)
            {
                for (int x = rect.Left; x < rect.Right; ++x)
                {
                    RectangleC neighborhood = new RectangleC(
                        new Point(Math.Max(x - Radius, 0), Math.Max(y - Radius, 0)),
                        new Point(Math.Min(x + Radius + 1, output.Width), Math.Min(y + Radius + 1, output.Height)));

                    int ones = 0;
                    for (int ny = neighborhood.Bottom; ny < neighborhood.Top; ++ny)
                        for (int nx = neighborhood.Left; nx < neighborhood.Right; ++nx)
                            if (input.GetBit(nx, ny))
                                ++ones;

                    double voteWeight = 1.0 / neighborhood.TotalArea;
                    if (ones * voteWeight >= Majority)
                        output.SetBitOne(x, y);
                }
            }
            return output;
        }
    }
}
