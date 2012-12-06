package sourceafis.general;

import java.io.Serializable;

@SuppressWarnings("serial")
public class Point implements Serializable {
	public int X;
	public int Y;

	public Point(int x, int y) {
		X = x;
		Y = y;
	}

	@Override
	public int hashCode() {
		//return X + Y;
		return (new Integer(X)).hashCode() + (new Integer(Y)).hashCode();

	}

	@Override
	public boolean equals(Object other) {
		return other instanceof Point && this.X == ((Point) other).X
				&& this.Y == ((Point) other).Y;
	}

	public static boolean isEqual(Point left, Point right) {
		return left.X == right.X && left.Y == right.Y;
	}

	public static boolean isNotEqual(Point left, Point right) {
		return left.X != right.X || left.Y != right.Y;
	}

	public static Point add(Point left, Size right) {
		return new Point(left.X + right.Width, left.Y + right.Height);
	}

	public static Point minus(Point left, Size right) {
		return new Point(left.X - right.Width, left.Y - right.Height);
	}

	public static java.awt.Point toPoint(Point point) {
		return new java.awt.Point(point.X, point.Y);
	}

	@Override
	public String toString() {
		return "[" + this.Y + ", " + this.X + "]";
	}
}
