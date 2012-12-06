/**
 * @author Veaceslav Dubenco
 * @since 10.10.2012
 */
package sourceafis.general;

/**
 * 
 */
public class Neighborhood {
	public static final Point[] EdgeNeighbors = new Point[] { new Point(0, -1),
			new Point(-1, 0), new Point(1, 0), new Point(0, 1) };

	public static final Point[] CornerNeighbors = new Point[] {
			new Point(-1, -1), new Point(0, -1), new Point(1, -1),
			new Point(-1, 0), new Point(1, 0), new Point(-1, 1),
			new Point(0, 1), new Point(1, 1) };

}
