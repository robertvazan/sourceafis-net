/**
 * @author Veaceslav Dubenco
 * @since 10.10.2012
 */
package sourceafis.general;

/**
 * 
 */
public class RangeF {
	public float Begin;
	public float End;

	public float getLength() {
		return End - Begin;
	}

	public RangeF(float begin, float end) {
		Begin = begin;
		End = end;
	}

	public float GetFraction(float value) {
		return (value - Begin) / getLength();
	}

	public float Interpolate(float fraction) {
		return Calc.Interpolate(Begin, End, fraction);
	}

}
