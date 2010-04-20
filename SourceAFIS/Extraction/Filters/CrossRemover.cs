using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;

namespace SourceAFIS.Extraction.Filters
{
    public sealed class CrossRemover
    {
        public DetailLogger.Hook Logger = DetailLogger.Null;

        public void Remove(BinaryMap input)
        {
            BinaryMap sw2ne = new BinaryMap(input.Size);
            BinaryMap se2nw = new BinaryMap(input.Size);
            BinaryMap positions = new BinaryMap(input.Size);
            BinaryMap squares = new BinaryMap(input.Size);

            while (true)
            {
                sw2ne.Copy(input, new RectangleC(0, 0, input.Width - 1, input.Height - 1), new Point());
                sw2ne.And(input, new RectangleC(1, 1, input.Width - 1, input.Height - 1), new Point());
                sw2ne.AndNot(input, new RectangleC(0, 1, input.Width - 1, input.Height - 1), new Point());
                sw2ne.AndNot(input, new RectangleC(1, 0, input.Width - 1, input.Height - 1), new Point());

                se2nw.Copy(input, new RectangleC(0, 1, input.Width - 1, input.Height - 1), new Point());
                se2nw.And(input, new RectangleC(1, 0, input.Width - 1, input.Height - 1), new Point());
                se2nw.AndNot(input, new RectangleC(0, 0, input.Width - 1, input.Height - 1), new Point());
                se2nw.AndNot(input, new RectangleC(1, 1, input.Width - 1, input.Height - 1), new Point());

                positions.Copy(sw2ne);
                positions.Or(se2nw);
                if (positions.IsEmpty())
                    break;

                squares.Copy(positions);
                squares.Or(positions, new RectangleC(0, 0, positions.Width - 1, positions.Height - 1), new Point(1, 0));
                squares.Or(positions, new RectangleC(0, 0, positions.Width - 1, positions.Height - 1), new Point(0, 1));
                squares.Or(positions, new RectangleC(0, 0, positions.Width - 1, positions.Height - 1), new Point(1, 1));

                input.AndNot(squares);
            }
            Logger.Log(input);
        }
    }
}
