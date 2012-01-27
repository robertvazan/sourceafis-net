package sourceafis.simple;

import java.util.EnumSet;
import java.util.HashMap;
import java.util.Map;

/**
 * Finger position on hand.
 * 
 * <p>
 * Finger position is used to speed up matching by skipping fingerprint pairs
 * that cannot match due to incompatible position. SourceAFIS will return zero
 * similarity score for incompatible fingerprint pairs.
 * </p>
 * 
 * <p>
 *   This feature is optional. It can be disabled by using finger position <see cref="Any"/>
 *   which is default value of <see cref="Fingerprint.Finger"/> for new <see cref="Fingerprint"/> objects.
 * </p>
 * 
 * <p>
 * A compatible fingerprint pair consists of two fingerprints with the same
 * finger position, e.g. <see cref="RightThumb"/> matches only other <see cref="RightThumb"/>. Alternatively,
 * compatible fingerprint pair can be also formed if one of the fingerprints
 * has <see cref="Any"/> finger position, e.g. <see cref="Any"/> can be matched against all other finger
 * positions and all other finger positions can be matched against <see cref="Any"/>. Two
 * fingerprints with <see cref="Any"/> positions are compatible as well, of course.
 * </p>
 *  <seealso cref="Fingerprint.Finger"/>
 * 
 */
public enum Finger
{

    ANY(0),//Unspecified finger position
    RIGHT_THUMB(1), // Thumb finger on the right hand.
    LEFT_THUMB(2), // Thumb finger on the left hand.
    RIGHT_INDEX(3), //Index finger on the right hand.
    LEFT_INDEX(4), // Index finger on the left hand.
    RIGHT_MIDDLE(5), // Middle finger on the right hand.
    LEFT_MIDDLE(6), // Middle finger on the left hand.
    RIGHT_RING(7),//Ring finger on the right hand.
    LEFT_RING(8),//Ring finger on the left hand.
    RIGHT_LITTLE(9),//Little finger on the right hand.
    LEFT_LITTLE(10); //Little finger on the left hand.
    private final byte value;

    Finger(int value) {
		this.value = (byte) value;
	}
    private static final Map<Byte,Finger> lookup 
    = new HashMap<Byte,Finger>();

    static {
    for(Finger s : EnumSet.allOf(Finger.class))
         lookup.put(s.getCode(), s);
    }

	public byte getCode() {
		return value;
	}
	public static Finger get(byte code) { 
        return lookup.get(code); 
   }

}
