/**
 * @author Veaceslav Dubenco
 * @since 19.10.2012
 */
package sourceafis.extraction.model;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import sourceafis.general.Point;

/**
 * 
 */
public final class SkeletonBuilder {
	List<SkeletonBuilderMinutia> AllMinutiae = new ArrayList<SkeletonBuilderMinutia>();

	public Iterable<SkeletonBuilderMinutia> getMinutiae() {
		return AllMinutiae;
	}

	public void AddMinutia(SkeletonBuilderMinutia minutia) {
		AllMinutiae.add(minutia);
	}

	public void RemoveMinutia(SkeletonBuilderMinutia minutia) {
		AllMinutiae.remove(minutia);
	}

	@Override
	public Object clone() {
		SkeletonBuilder clone = new SkeletonBuilder();

		Map<SkeletonBuilderMinutia, SkeletonBuilderMinutia> minutiaClones = new HashMap<SkeletonBuilderMinutia, SkeletonBuilderMinutia>();
		for (SkeletonBuilderMinutia minutia : AllMinutiae) {
			SkeletonBuilderMinutia minutiaClone = new SkeletonBuilderMinutia(minutia.Position);
			minutiaClone.Valid = minutia.Valid;
			clone.AddMinutia(minutiaClone);
			minutiaClones.put(minutia, minutiaClone);
		}

		Map<Ridge, Ridge> ridgeClones = new HashMap<Ridge, Ridge>();
		for (SkeletonBuilderMinutia minutia : AllMinutiae) {
			for (Ridge ridge : minutia.getRidges()) {
				if (!ridgeClones.containsKey(ridge)) {
					Ridge ridgeClone = new Ridge();
					ridgeClone.setStart(minutiaClones.get(ridge.getStart()));
					ridgeClone.setEnd(minutiaClones.get(ridge.getEnd()));

					for (Point point : ridge.Points)
						ridgeClone.Points.add(point);
					ridgeClones.put(ridge, ridgeClone);
					ridgeClones.put(ridge.Reversed, ridgeClone.Reversed);
				}
			}
		}

		return clone;
	}
}
