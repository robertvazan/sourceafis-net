package sourceafis.general;

public class SizeF {
	public float Width;
	public float Height;

	public SizeF(float width, float height) {
		Width = width;
		Height = height;
	}

	public SizeF(PointF point) {
		Width = point.X;
		Height = point.Y;
	}
}