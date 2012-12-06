/**
 * @author Veaceslav Dubenco
 * @since 20.10.2012
 */
package sourceafis.extraction.model;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import sourceafis.general.AssertException;
import sourceafis.general.BinaryMap;
import sourceafis.general.Calc;
import sourceafis.general.DetailLogger;
import sourceafis.general.Neighborhood;
import sourceafis.general.Point;

/**
 * 
 */
public final class RidgeTracer {
	public DetailLogger.Hook Logger = DetailLogger.off;

	static boolean[] IsMinutia = ConstructPixelClassifier();

	static boolean[] ConstructPixelClassifier() {
		boolean[] result = new boolean[256];
		for (int mask = 0; mask < 256; ++mask) {
			int count = Calc.CountBits(mask);
			result[mask] = (count == 1 || count > 2);
		}
		return result;
	}

	List<Point> FindMinutiae(BinaryMap binary) {
		List<Point> result = new ArrayList<Point>();
		for (int y = 0; y < binary.getHeight(); ++y)
			for (int x = 0; x < binary.getWidth(); ++x)
				if (binary.GetBit(x, y)
						&& IsMinutia[binary.GetNeighborhood(x, y)])
					result.add(new Point(x, y));
		return result;
	}

	Map<Point, List<Point>> LinkNeighboringMinutiae(List<Point> minutiae) {
		Map<Point, List<Point>> linking = new LinkedHashMap<Point, List<Point>>();
		for (Point minutiaPos : minutiae) {
			List<Point> ownLinks = null;
			for (Point neighborRelative : Neighborhood.CornerNeighbors) {
				Point neighborPos = Calc.Add(minutiaPos, neighborRelative);
				if (linking.containsKey(neighborPos)) {
					List<Point> neighborLinks = linking.get(neighborPos);
					if (neighborLinks != ownLinks) {
						if (ownLinks != null) {
							neighborLinks.addAll(ownLinks);
							for (Point mergedPos : ownLinks)
								linking.put(mergedPos, neighborLinks);
						}
						ownLinks = neighborLinks;
					}
				}
			}
			if (ownLinks == null)
				ownLinks = new ArrayList<Point>();
			ownLinks.add(minutiaPos);
			linking.put(minutiaPos, ownLinks);
		}
		return linking;
	}

	Map<Point, SkeletonBuilderMinutia> ComputeMinutiaCenters(
			Map<Point, List<Point>> linking, SkeletonBuilder skeleton) {
		Map<Point, SkeletonBuilderMinutia> centers = new LinkedHashMap<Point, SkeletonBuilderMinutia>();
		for (Point currentPos : linking.keySet()) {
			List<Point> linkedMinutiae = linking.get(currentPos);
			Point primaryPos = linkedMinutiae.get(0);
			if (!centers.containsKey(primaryPos)) {
				Point sum = new Point(0, 0);
				for (Point linkedPos : linkedMinutiae)
					sum = Calc.Add(sum, linkedPos);
				Point center = new Point(sum.X / linkedMinutiae.size(), sum.Y
						/ linkedMinutiae.size());
				SkeletonBuilderMinutia minutia = new SkeletonBuilderMinutia(
						center);
				skeleton.AddMinutia(minutia);
				centers.put(primaryPos, minutia);
			}
			centers.put(currentPos, centers.get(primaryPos));
		}
		return centers;
	}

	void TraceRidges(BinaryMap binary,
			Map<Point, SkeletonBuilderMinutia> minutiaePoints) {
		Map<Point, Ridge> leads = new LinkedHashMap<Point, Ridge>();
		for (Point minutiaPoint : minutiaePoints.keySet()) {
			for (Point startRelative : Neighborhood.CornerNeighbors) {
				Point start = Calc.Add(minutiaPoint, startRelative);
				if (binary.GetBitSafe(start, false)
						&& !minutiaePoints.containsKey(start)
						&& !leads.containsKey(start)) {
					Ridge ridge = new Ridge();
					ridge.Points.add(minutiaPoint);
					ridge.Points.add(start);
					Point previous = minutiaPoint;
					Point current = start;
					do {
						Point next = new Point(0, 0);
						for (Point nextRelative : Neighborhood.CornerNeighbors) {
							next = Calc.Add(current, nextRelative);
							if (binary.GetBitSafe(next, false)
									&& Point.isNotEqual(next, previous))
								break;
						}
						AssertException.Check(next != null, "");
						AssertException.Check(
								Point.isNotEqual(next, new Point(0, 0)), "");
						previous = current;
						current = next;
						ridge.Points.add(current);
					} while (!minutiaePoints.containsKey(current));
					Point end = current;

					ridge.setStart(minutiaePoints.get(minutiaPoint));
					ridge.setEnd(minutiaePoints.get(end));
					leads.put(ridge.Points.get(1), ridge);
					leads.put(ridge.Reversed.Points.get(1), ridge);
				}
			}
		}
	}

	public void FixLinkingGaps(SkeletonBuilder skeleton) {
		for (SkeletonBuilderMinutia minutia : skeleton.getMinutiae()) {
			for (Ridge ridge : minutia.getRidges()) {
				if (ridge.Points.get(0) != minutia.Position) {
					Point[] filling = Calc.ConstructLine(ridge.Points.get(0),
							minutia.Position);
					for (int i = 1; i < filling.length; ++i)
						ridge.Reversed.Points.add(filling[i]);
				}
			}
		}
	}

	public void Trace(BinaryMap binary, SkeletonBuilder skeleton) {
		List<Point> minutiaPoints = FindMinutiae(binary);
		Map<Point, List<Point>> linking = LinkNeighboringMinutiae(minutiaPoints);
		Map<Point, SkeletonBuilderMinutia> minutiaMap = ComputeMinutiaCenters(
				linking, skeleton);
		TraceRidges(binary, minutiaMap);
		FixLinkingGaps(skeleton);
		Logger.log(skeleton);
	}
}
