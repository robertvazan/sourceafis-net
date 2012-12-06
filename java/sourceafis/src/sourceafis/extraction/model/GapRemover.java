/**
 * @author Veaceslav Dubenco
 * @since 20.10.2012
 */
package sourceafis.extraction.model;

import sourceafis.general.Angle;
import sourceafis.general.BinaryMap;
import sourceafis.general.Calc;
import sourceafis.general.DetailLogger;
import sourceafis.general.Point;
import sourceafis.general.PriorityQueueF;
import sourceafis.meta.DpiAdjusted;
import sourceafis.meta.Nested;
import sourceafis.meta.Parameter;

/**
 * 
 */
public class GapRemover implements ISkeletonFilter {

	@DpiAdjusted
	@Parameter(lower = 0, upper = 20)
	public int RuptureSize = 5;
	@DpiAdjusted
	@Parameter(lower = 0, upper = 100)
	public int GapSize = 20;
	@Parameter(upper = Angle.B90)
	public byte GapAngle = 32;
	@DpiAdjusted
	@Parameter(lower = 3, upper = 40)
	public int AngleSampleOffset = 22;
	@DpiAdjusted
	@Parameter(upper = 100)
	public int ToleratedOverlapLength = 2;
	@DpiAdjusted
	@Parameter(upper = 20)
	public int MinEndingLength = 7;

	@Nested
	public KnotRemover KnotRemover = new KnotRemover();
	@Nested
	public SkeletonShadow SkeletonShadow = new SkeletonShadow();

	public DetailLogger.Hook Logger = DetailLogger.off;

	class Gap {
		public SkeletonBuilderMinutia End1;
		public SkeletonBuilderMinutia End2;
	}

	@Override
	public void Filter(SkeletonBuilder skeleton) {
		PriorityQueueF<Gap> queue = new PriorityQueueF<Gap>();
		for (SkeletonBuilderMinutia end1 : skeleton.getMinutiae())
			if (end1.getRidges().size() == 1
					&& end1.getRidges().get(0).getPoints().size() >= MinEndingLength)
				for (SkeletonBuilderMinutia end2 : skeleton.getMinutiae())
					if (end2 != end1
							&& end2.getRidges().size() == 1
							&& end1.getRidges().get(0).getEnd() != end2
							&& end2.getRidges().get(0).getPoints().size() >= MinEndingLength
							&& IsWithinLimits(end1, end2)) {
						Gap gap = new Gap();
						gap.End1 = end1;
						gap.End2 = end2;
						queue.enqueue(
								Calc.DistanceSq(end1.getPosition(),
										end2.getPosition()), gap);
					}

		BinaryMap shadow = SkeletonShadow.Draw(skeleton);
		while (queue.size() > 0) {
			Gap gap = queue.dequeue();
			if (gap.End1.getRidges().size() == 1
					&& gap.End2.getRidges().size() == 1) {
				Point[] line = Calc.ConstructLine(gap.End1.getPosition(),
						gap.End2.getPosition());
				if (!IsOverlapping(line, shadow))
					AddRidge(skeleton, shadow, gap, line);
			}
		}

		KnotRemover.Filter(skeleton);
		Logger.log(skeleton);
	}

	boolean IsWithinLimits(SkeletonBuilderMinutia end1,
			SkeletonBuilderMinutia end2) {
		int distanceSq = Calc
				.DistanceSq(end1.getPosition(), end2.getPosition());
		if (distanceSq <= Calc.Sq(RuptureSize))
			return true;
		if (distanceSq > Calc.Sq(GapSize))
			return false;

		byte gapDirection = Angle.AtanB(end1.getPosition(), end2.getPosition());
		byte direction1 = Angle.AtanB(end1.getPosition(), GetAngleSample(end1));
		if (Angle.Distance(direction1, Angle.Opposite(gapDirection)) > GapAngle)
			return false;
		byte direction2 = Angle.AtanB(end2.getPosition(), GetAngleSample(end2));
		if (Angle.Distance(direction2, gapDirection) > GapAngle)
			return false;
		return true;
	}

	Point GetAngleSample(SkeletonBuilderMinutia minutia) {
		Ridge ridge = minutia.getRidges().get(0);
		if (AngleSampleOffset < ridge.getPoints().size())
			return ridge.getPoints().get(AngleSampleOffset);
		else
			return ridge.getEnd().getPosition();
	}

	boolean IsOverlapping(Point[] line, BinaryMap shadow) {
		for (int i = ToleratedOverlapLength; i < line.length
				- ToleratedOverlapLength; ++i)
			if (shadow.GetBit(line[i]))
				return true;
		return false;
	}

	void AddRidge(SkeletonBuilder skeleton, BinaryMap shadow, Gap gap,
			Point[] line) {
		Ridge ridge = new Ridge();
		for (Point point : line)
			ridge.getPoints().add(point);
		ridge.setStart(gap.End1);
		ridge.setEnd(gap.End2);
		for (Point point : line)
			shadow.SetBitOne(point);
	}
}
