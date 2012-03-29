package sourceafis.simple;

import java.util.EnumSet;
import java.util.HashMap;
import java.util.Map;

import sourceafis.general.AssertException;

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
 *   This feature is optional. It can be disabled by using finger position {@link #ANY} 
 *   which is default value of {@link Fingerprint#setFinger finger} property for new {@link Fingerprint} objects.
 * </p>
 * 
 * <p>
 * A compatible fingerprint pair consists of two fingerprints with the same
 * finger position, e.g. {@link #RIGHT_THUMB} matches only other {@link #RIGHT_THUMB}. Alternatively,
 * compatible fingerprint pair can be also formed if one of the fingerprints
 * has {@link #ANY} finger position, e.g. {@link #ANY} can be matched against all other finger
 * positions and all other finger positions can be matched against {@link #ANY}. Two
 * fingerprints with{@link #ANY} positions are compatible as well, of course.
 * </p>
 * @see Fingerprint#setFinger Fingerprint.setFinger 
 */
public enum Finger
{
    /**
     * Unspecified finger position.
     */
    ANY(0), 
    /**
     * Thumb finger on the right hand.
     */
    RIGHT_THUMB(1), 
    /**
     * Thumb finger on the left hand.
     */
    LEFT_THUMB(2), 
    /**
     * Index finger on the right hand.
     */
    RIGHT_INDEX(3),  
    /**
     * Index finger on the left hand.
     */
    LEFT_INDEX(4),  
    /**
     *  Middle finger on the right hand.
     */
    RIGHT_MIDDLE(5),  
    /**
     *  Middle finger on the left hand.
     */
    LEFT_MIDDLE(6), 
    /**
     * Ring finger on the right hand.
     */
    RIGHT_RING(7), 
    /**
     * Ring finger on the left hand.
     */
    LEFT_RING(8),
    /**
     * Little finger on the right hand.
     */
    RIGHT_LITTLE(9),
    /**
     * Little finger on the left hand.
     */
    LEFT_LITTLE(10); 
    private final byte value;

    Finger(int value) {
		this.value = (byte) value;
	}
    private static final Map<Byte,Finger> lookup 
    = new HashMap<Byte,Finger>();

    static {
    for(Finger s : EnumSet.allOf(Finger.class))
         lookup.put(s.toByte(), s);
    }

	public byte toByte() {
		return value;
	}
	public static Finger valueOf(int code) { 
        AssertException.Check(code >= 0 && code <= 10, "Unknown finger position.");
        return lookup.get((byte)code);
   }

}
