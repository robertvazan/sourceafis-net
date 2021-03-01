// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	public class FingerprintImage
	{
		internal double Dpi;
		internal DoubleMatrix Matrix;

		FingerprintImage(DoubleMatrix matrix, FingerprintImageOptions options)
		{
			Matrix = matrix;
			Dpi = options.Dpi;
		}

		public static FingerprintImage Grayscale(int width, int height, byte[] pixels, FingerprintImageOptions options)
		{
			if (pixels == null)
				throw new ArgumentNullException(nameof(pixels));
			if (width <= 0 || height <= 0 || pixels.Length != width * height)
				throw new ArgumentOutOfRangeException();
			var matrix = new DoubleMatrix(width, height);
			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
					matrix[x, y] = 1 - pixels[y * width + x] / 255.0;
			return new FingerprintImage(matrix, options);
		}
		public static FingerprintImage Grayscale(int width, int height, byte[] pixels) { return Grayscale(width, height, pixels, new FingerprintImageOptions()); }
	}
}
