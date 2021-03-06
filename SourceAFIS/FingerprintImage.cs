// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	/// <summary>Pixels and metadata of the fingerprint image.</summary>
	/// <remarks>
	/// <para>
	/// This class captures all fingerprint information that is available prior to construction of <see cref="T:FingerprintTemplate" />.
	/// It consists of pixel data and additional information in <see cref="T:FingerprintImageOptions" />.
	/// </para>
	/// <para>
	/// Application should start fingerprint processing by constructing an instance of <c>FingerprintImage</c>
	/// and then passing it to <see cref="T:FingerprintTemplate" /> constructor.
	/// </para>
	/// <para>
	/// Currently only raw grayscale images are supported.
	/// </para>
	/// </remarks>
	/// <seealso cref="T:FingerprintTemplate" />
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
