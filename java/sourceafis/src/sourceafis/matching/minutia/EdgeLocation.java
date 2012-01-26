package sourceafis.matching.minutia;
/*
 * Added newly
 */
public class EdgeLocation {

	public short reference;
	public short neighbor;

	public EdgeLocation(int reference, int neighbor) {
		this.reference = (short) reference;
		this.neighbor = (short) neighbor;
	}
}
