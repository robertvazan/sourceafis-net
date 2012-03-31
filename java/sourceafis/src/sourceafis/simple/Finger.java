package sourceafis.simple;

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
 * fingerprints with {@link #ANY} positions are compatible as well, of course.
 * </p>
 * <p>
 * All enum values have numeric codes like in C# or C++. Documentation for each enum value notes
 * its numeric code. The numeric code can be read by calling {@link #toByte} method on the enum value.
 * Numeric code can be converted to enum value by calling {@link #valueOf(int)}.
 * </p>
 * <ul>
 * <li>0 - {@link #ANY}
 * <li>1 - {@link #RIGHT_THUMB}
 * <li>2 - {@link #LEFT_THUMB}
 * <li>3 - {@link #RIGHT_INDEX}
 * <li>4 - {@link #LEFT_INDEX}
 * <li>5 - {@link #RIGHT_MIDDLE}
 * <li>6 - {@link #LEFT_MIDDLE}
 * <li>7 - {@link #RIGHT_RING}
 * <li>8 - {@link #LEFT_RING}
 * <li>9 - {@link #RIGHT_LITTLE}
 * <li>10 - {@link #LEFT_LITTLE}
 * </ul>
 * @see Fingerprint#setFinger Fingerprint.setFinger 
 */
public enum Finger
{
    /**
     * Unspecified finger position (0).
     */
    ANY(0), 
    /**
     * Thumb finger on the right hand (1).
     */
    RIGHT_THUMB(1), 
    /**
     * Thumb finger on the left hand (2).
     */
    LEFT_THUMB(2), 
    /**
     * Index finger on the right hand (3).
     */
    RIGHT_INDEX(3),  
    /**
     * Index finger on the left hand (4).
     */
    LEFT_INDEX(4),  
    /**
     *  Middle finger on the right hand (5).
     */
    RIGHT_MIDDLE(5),  
    /**
     *  Middle finger on the left hand (6).
     */
    LEFT_MIDDLE(6), 
    /**
     * Ring finger on the right hand (7).
     */
    RIGHT_RING(7), 
    /**
     * Ring finger on the left hand (8).
     */
    LEFT_RING(8),
    /**
     * Little finger on the right hand (9).
     */
    RIGHT_LITTLE(9),
    /**
     * Little finger on the left hand (10).
     */
    LEFT_LITTLE(10); 
    
    private final byte value;
    private static final Map<Byte,Finger> lookup = new HashMap<Byte,Finger>();

    static {
        for(Finger s : Finger.values())
             lookup.put(s.toByte(), s);
    }

    Finger(int value) {
		this.value = (byte) value;
	}

    /**
     * Converts enum value to its numeric code.
     * @return numeric code associated with this enum value
     */
	public byte toByte() {
		return value;
	}
	
	/**
	 * Converts numeric code to the corresponding enum value.
	 * @param code the numeric code to be converted
	 * @return enum value corresponding to the numeric code
	 * @throws IllegalArgumentException if the argument doesn't correspond to any enum value
	 */
	public static Finger valueOf(int code) { 
        if (code < 0 || code > 10)
        	throw new IllegalArgumentException();
        return lookup.get((byte)code);
   }
}
