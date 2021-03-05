// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	public class FingerprintImage
	{
		internal DoubleMatrix Matrix;
		internal double Dpi;

		public FingerprintImage(int width, int height, byte[] pixels, FingerprintImageOptions options = null)
		{
			if (pixels == null)
				throw new ArgumentNullException(nameof(pixels));
			if (width <= 0 || height <= 0 || pixels.Length != width * height)
				throw new ArgumentOutOfRangeException();
			Matrix = new DoubleMatrix(width, height);
			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
					Matrix[x, y] = 1 - pixels[y * width + x] / 255.0;
			if (options == null)
				options = new FingerprintImageOptions();
			Dpi = options.Dpi;
		}
	}
}
