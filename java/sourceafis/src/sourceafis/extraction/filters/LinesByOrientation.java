/**
 * @author Veaceslav Dubenco
 * @since 17.10.2012
 */
package sourceafis.extraction.filters;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

import sourceafis.general.Angle;
import sourceafis.general.Calc;
import sourceafis.general.Point;
import sourceafis.general.PointF;
import sourceafis.meta.DpiAdjusted;
import sourceafis.meta.Parameter;

/**
 * 
 */
public final class LinesByOrientation {
	@Parameter(lower = 4, upper = 128)
	public int AngularResolution = 32;
	@DpiAdjusted
	@Parameter(upper = 50)
	public int Radius = 7;
	@Parameter(lower = 1.1, upper = 4)
	public float StepFactor = 1.5f;

	public Point[][] Construct() {
		Point[][] result = new Point[AngularResolution][];
		for (int orientationIndex = 0; orientationIndex < AngularResolution; ++orientationIndex) {
			List<Point> line = new ArrayList<Point>();
			line.add(new Point(0, 0));
			PointF direction = Angle.ToVector(Angle.ByBucketCenter(
					orientationIndex, 2 * AngularResolution));
			for (float radius = Radius; radius >= 0.5f; radius /= StepFactor) {
				Point point = Calc.Round(Calc.Multiply(radius, direction));
				if (!line.contains(point)) {
					line.add(point);
					line.add(Calc.Negate(point));
				}
			}
			// line.Sort(Calc.CompareYX);
			Collections.sort(line, new Comparator<Point>() {
				@Override
				public int compare(Point p1, Point p2) {
					return Calc.CompareYX(p1, p2);
				}
			});
			Point[] arr = new Point[line.size()];
			result[orientationIndex] = line.toArray(arr);
		}
		return result;
	}
}
