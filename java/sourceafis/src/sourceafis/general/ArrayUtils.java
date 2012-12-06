/**
 * @author Veaceslav Dubenco
 * @since 10.10.2012
 */
package sourceafis.general;

/**
 * 
 */
public class ArrayUtils {
	public static void copyArray(Object[] source, int sourceStartIdx,
			Object[] dest, int destStartIdx, int count) {
		for (int i = 0; i < count; i++) {
			dest[destStartIdx + i] = source[sourceStartIdx + i];
		}
	}

	public static void copyArray(Object[] source, Object[] dest,
			int destStartIdx) {
		copyArray(source, 0, dest, destStartIdx, source.length);
	}

	public static void reverse(Object[] array, int startIdx, int count) {
		int endIdx = startIdx + count - 1;
		for (int i = 0; i < count / 2; i++) {
			Object tmp = array[startIdx + i];
			array[startIdx + i] = array[endIdx - i];
			array[endIdx - i] = tmp;
		}
	}

}
