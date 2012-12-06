package sourceafis.extraction.model;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import sourceafis.general.Calc;
import sourceafis.general.Point;

public final class SkeletonBuilderMinutia {
	public boolean Valid = true;
	Point Position;

	public Point getPosition() {
		return Position;
	}

	private List<Ridge> AllRidges = new ArrayList<Ridge>();

	public List<Ridge> getAllRidges() {
		return AllRidges;
	}

	private List<Ridge> ReadOnlyRidges;

	public List<Ridge> getRidges() {
		return ReadOnlyRidges;
	}

	public SkeletonBuilderMinutia(Point position) {
		Position = position;
		ReadOnlyRidges = Collections.unmodifiableList(AllRidges);
	}

	public void AttachStart(Ridge ridge) {
		if (!AllRidges.contains(ridge)) {
			AllRidges.add(ridge);
			ridge.setStart(this);
		}
	}

	public void DetachStart(Ridge ridge) {
		if (AllRidges.contains(ridge)) {
			AllRidges.remove(ridge);
			if (ridge.getStart() == this)
				ridge.setStart(null);
		}
	}

	@Override
	public boolean equals(Object other) {
		if (other == null || !(other instanceof SkeletonBuilderMinutia)) {
			return false;
		}
		SkeletonBuilderMinutia m = (SkeletonBuilderMinutia) other;
		return this.Valid == m.Valid
				//&& Calc.areEqual(this.AllRidges, m.AllRidges)
				&& Calc.areEqual(this.Position, m.Position);
	}
}