package sourceafis.templates;
/*
 * Added Convert to be able to serialize as byte  
 */
public enum MinutiaType // : byte
{
	Ending(0), Bifurcation(1), Other(2);
	private final byte value;

	MinutiaType(int value) {
		this.value = (byte) value;
	}
	public byte convert() {
		return value;
	}

}
