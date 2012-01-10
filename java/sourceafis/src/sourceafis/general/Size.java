package sourceafis.general;

public class Size {
	public int Width;
	public int Height;

	public Size(int width, int height) {
		Width = width;
		Height = height;
	}

	public Size(Point point) {
		Width = point.X;
		Height = point.Y;
	}

	public boolean equals(Object other) {

		return other instanceof Size && this == other;

		/*
		 * if(!(other instanceof Size)) return false; Size s=(Size)other;
		 * 
		 * return this.Width == s.Width && this.Height == s.Height;
		 */
	}

	public int hashCode() {

		return Width ^ Height;
	}

	public static boolean isEqual(Size left, Size right) {
		return left.Width == right.Width && left.Height == right.Height;
	}

	public static boolean isNotEqual(Size left, Size right) {
		return !isEqual(left, right);
	}

}
