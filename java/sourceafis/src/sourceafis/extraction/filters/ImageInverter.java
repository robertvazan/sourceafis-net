/**
 * @author Veaceslav Dubenco
 * @since 17.10.2012
 */
package sourceafis.extraction.filters;

/**
 * 
 */
public final class ImageInverter {
	public static byte[][] GetInverted(byte[][] image) {
		byte[][] result = new byte[image.length][image[0].length];
		for (int y = 0; y < image.length; ++y)
			for (int x = 0; x < image[0].length; ++x)
				result[y][x] = (byte) (255 - (image[y][x] & 0xFF));
		return result;
	}
}
