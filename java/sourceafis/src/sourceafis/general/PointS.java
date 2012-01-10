package sourceafis.general;
 
 
    public class PointS
    {
        public short X;
        public short Y;

        public PointS(Point point)
        {
            X = (short)point.X;
            Y = (short)point.Y;
        }

        public Point ToPoint()
        {
            return new Point(X, Y);
        }

        public static Point toPoint(PointS point)
        {
            return new Point(point.X, point.Y);
        }
    }

