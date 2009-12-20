using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;

namespace SourceAFIS.Extraction
{
    public sealed class Thinner
    {
        public int MaxIterations = 15;

        static readonly bool[] IsRemovable = ConstructRemovable();

        static bool[] ConstructRemovable()
        {
            bool[] removable = new bool[256];
            for (uint mask = 0; mask < 256; ++mask)
            {
                bool TL = (mask & 1) != 0;
                bool TC = (mask & 2) != 0;
                bool TR = (mask & 4) != 0;
                bool CL = (mask & 8) != 0;
                bool CR = (mask & 16) != 0;
                bool BL = (mask & 32) != 0;
                bool BC = (mask & 64) != 0;
                bool BR = (mask & 128) != 0;

                int count = Calc.CountBits(mask);

                bool diagonal = !TC && !CL && TL || !CL && !BC && BL || !BC && !CR && BR || !CR && !TC && TR;
                bool horizontal = !TC && !BC && (TR || CR || BR) && (TL || CL || BL);
                bool vertical = !CL && !CR && (TL || TC || TR) && (BL || BC || BR);
                bool end = (count == 1);

                removable[mask] = !diagonal && !horizontal && !vertical && !end;
            }
            return removable;
        }

        public BinaryMap Thin(BinaryMap input)
        {
            BinaryMap intermediate = new BinaryMap(input.Size);
            intermediate.Copy(input, new RectangleC(1, 1, input.Width - 2, input.Height - 2), new Point(1, 1));

            BinaryMap border = new BinaryMap(input.Size);
            BinaryMap skeleton = new BinaryMap(input.Size);
            bool removedAnything = true;
            for (int i = 0; i < MaxIterations && removedAnything; ++i)
            {
                border.Clear();
                border.OrNot(intermediate, new RectangleC(1, 0, border.Width - 1, border.Height), new Point(0, 0));
                border.OrNot(intermediate, new RectangleC(0, 0, border.Width - 1, border.Height), new Point(1, 0));
                border.OrNot(intermediate, new RectangleC(0, 1, border.Width, border.Height - 1), new Point(0, 0));
                border.OrNot(intermediate, new RectangleC(0, 0, border.Width, border.Height - 1), new Point(0, 1));
                border.And(intermediate);
                border.AndNot(skeleton);

                removedAnything = false;
                for (int y = 1; y < input.Height - 1; ++y)
                    for (int xw = 0; xw < input.WordWidth; ++xw)
                        if (border.IsWordNonZero(xw, y))
                            for (int x = xw << BinaryMap.WordShift; x < (xw << BinaryMap.WordShift) + BinaryMap.WordSize; ++x)
                                if (x > 0 && x < input.Width - 1 && border.GetBit(x, y))
                                {
                                    if (IsRemovable[intermediate.GetNeighborhood(x, y)])
                                    {
                                        removedAnything = true;
                                        intermediate.SetBitZero(x, y);
                                    }
                                    else
                                        skeleton.SetBitOne(x, y);
                                }
            }

            Logger.Log(this, skeleton);
            return skeleton;
        }
    }
}
