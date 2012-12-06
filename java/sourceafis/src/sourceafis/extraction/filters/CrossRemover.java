/**
 * @author Veaceslav Dubenco
 * @since 11.10.2012
 */
package sourceafis.extraction.filters;

import sourceafis.general.BinaryMap;
import sourceafis.general.DetailLogger;
import sourceafis.general.Point;
import sourceafis.general.RectangleC;

/**
 * 
 */
public class CrossRemover {
	public DetailLogger.Hook Logger = DetailLogger.off;

	public void Remove(BinaryMap input) {
		BinaryMap sw2ne = new BinaryMap(input.getSize());
		BinaryMap se2nw = new BinaryMap(input.getSize());
		BinaryMap positions = new BinaryMap(input.getSize());
		BinaryMap squares = new BinaryMap(input.getSize());

		while (true) {
			sw2ne.Copy(
					input,
					new RectangleC(0, 0, input.getWidth() - 1, input
							.getHeight() - 1), new Point(0, 0));
			sw2ne.And(
					input,
					new RectangleC(1, 1, input.getWidth() - 1, input
							.getHeight() - 1), new Point(0, 0));
			sw2ne.AndNot(input, new RectangleC(0, 1, input.getWidth() - 1,
					input.getHeight() - 1), new Point(0, 0));
			sw2ne.AndNot(input, new RectangleC(1, 0, input.getWidth() - 1,
					input.getHeight() - 1), new Point(0, 0));

			se2nw.Copy(
					input,
					new RectangleC(0, 1, input.getWidth() - 1, input
							.getHeight() - 1), new Point(0, 0));
			se2nw.And(
					input,
					new RectangleC(1, 0, input.getWidth() - 1, input
							.getHeight() - 1), new Point(0, 0));
			se2nw.AndNot(input, new RectangleC(0, 0, input.getWidth() - 1,
					input.getHeight() - 1), new Point(0, 0));
			se2nw.AndNot(input, new RectangleC(1, 1, input.getWidth() - 1,
					input.getHeight() - 1), new Point(0, 0));

			positions.Copy(sw2ne);
			positions.Or(se2nw);
			if (positions.IsEmpty())
				break;

			squares.Copy(positions);
			squares.Or(positions, new RectangleC(0, 0,
					positions.getWidth() - 1, positions.getHeight() - 1),
					new Point(1, 0));
			squares.Or(positions, new RectangleC(0, 0,
					positions.getWidth() - 1, positions.getHeight() - 1),
					new Point(0, 1));
			squares.Or(positions, new RectangleC(0, 0,
					positions.getWidth() - 1, positions.getHeight() - 1),
					new Point(1, 1));

			input.AndNot(squares);
		}
		Logger.log(input);
	}
}
