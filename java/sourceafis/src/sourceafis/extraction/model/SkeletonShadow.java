/**
 * @author Veaceslav Dubenco
 * @since 20.10.2012
 */
package sourceafis.extraction.model;

import sourceafis.general.BinaryMap;
import sourceafis.general.Point;
import sourceafis.general.RectangleC;
import sourceafis.general.Size;

/**
 * 
 */
public final class SkeletonShadow {
	public Size GetSize(SkeletonBuilder skeleton) {
		RectangleC rect = new RectangleC(0, 0, 1, 1);
		for (SkeletonBuilderMinutia minutia : skeleton.getMinutiae()) {
			rect.Include(minutia.getPosition());
			for (Ridge ridge : minutia.getRidges()) {
				if (ridge.getStart().getPosition().Y <= ridge.getEnd()
						.getPosition().Y)
					for (Point point : ridge.getPoints())
						rect.Include(point);
			}
		}
		return rect.getSize();
	}

	public BinaryMap Draw(SkeletonBuilder skeleton) {
		BinaryMap binary = new BinaryMap(GetSize(skeleton));
		Draw(skeleton, binary);
		return binary;
	}

	public static void Draw(SkeletonBuilder skeleton, BinaryMap binary) {
		for (SkeletonBuilderMinutia minutia : skeleton.getMinutiae()) {
			binary.SetBitOne(minutia.getPosition());
			for (Ridge ridge : minutia.getRidges())
				if (ridge.getStart().getPosition().Y <= ridge.getEnd()
						.getPosition().Y)
					for (Point point : ridge.getPoints())
						if (point != null)
							binary.SetBitOne(point);
		}
	}
}
