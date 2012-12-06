package sourceafis.general;

public class PointF {

	public float X;
	public float Y;

	public PointF(float x, float y) {
		X = x;
		Y = y;
	}

	public static PointF toPointF(Point point) {
		return new PointF(point.X, point.Y);
	}

	public static PointF add(PointF left, SizeF right) {
		return new PointF(left.X + right.Width, left.Y + right.Height);
	}

	public static PointF multiply(float factor, PointF point) {
		return new PointF(factor * point.X, factor * point.Y);
	}

	public static java.awt.Point toPoint(PointF point) {
		java.awt.Point p = new java.awt.Point();
		p.setLocation(point.X, point.Y);
		return p;
	}

}
