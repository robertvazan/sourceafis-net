using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;

namespace SourceAFIS.Extraction
{
    public sealed class CrossRemover
    {
        public void Remove(BinaryMap binary)
        {
            BinaryMap diagonalNW = new BinaryMap(binary);
            diagonalNW.Xor(binary, new RectangleC(1, 1, binary.Width - 1, binary.Height - 1), new Point(0, 0));
            BinaryMap diagonalNE = new BinaryMap(binary);
            diagonalNE.Xor(binary, new RectangleC(0, 1, binary.Width - 1, binary.Height - 1), new Point(1, 0));
            BinaryMap diagonalOr = new BinaryMap(diagonalNW);
            diagonalOr.Or(diagonalNE, new RectangleC(1, 0, binary.Width - 1, binary.Height), new Point(0, 0));
            BinaryMap horizontalOr = new BinaryMap(binary);
            horizontalOr.Or(binary, new RectangleC(1, 0, binary.Width - 1, binary.Height), new Point(0, 0));
            BinaryMap bridges = new BinaryMap(horizontalOr);
            bridges.AndNot(diagonalOr);
            BinaryMap bridgeFill = new BinaryMap(bridges);
            bridgeFill.Or(bridges, new RectangleC(0, 0, binary.Width - 1, binary.Height), new Point(1, 0));
            bridgeFill.Or(bridges, new RectangleC(0, 0, binary.Width, binary.Height - 1), new Point(0, 1));
            bridgeFill.Or(bridges, new RectangleC(0, 0, binary.Width - 1, binary.Height - 1), new Point(1, 1));
            Logger.Log(this, bridgeFill);
            binary.Or(bridgeFill);
        }
    }
}
