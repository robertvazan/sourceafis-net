// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	/// <summary>Pixels and metadata of the fingerprint image.</summary>
	/// <remarks>
	/// <para>
	/// This class captures all fingerprint information that is available prior to construction of <see cref="FingerprintTemplate" />.
	/// It consists of pixel data and additional information in <see cref="FingerprintImageOptions" />.
	/// Since SourceAFIS algorithm is not scale-invariant, all images should have DPI
	/// configured explicitly by setting <see cref="FingerprintImageOptions.Dpi" />.
	/// </para>
	/// <para>
	/// Application should start fingerprint processing by constructing an instance of <c>FingerprintImage</c>
	/// and then passing it to <see cref="FingerprintTemplate(FingerprintImage)" /> constructor.
	/// </para>
	/// <para>
	/// Only raw grayscale images are currently supported.
	/// </para>
	/// </remarks>
	/// <seealso cref="FingerprintImageOptions" />
	/// <seealso cref="FingerprintTemplate" />
	public class FingerprintImage
	{
		internal DoubleMatrix Matrix;
		internal double Dpi;

		/// <summary>Reads raw grayscale fingerprint image from byte array.</summary>
		/// <remarks>
		/// <para>
		/// The image must contain black fingerprint on white background
		/// in resolution specified by <see cref="FingerprintImageOptions.Dpi" />.
		/// </para>
		/// <para>
		/// Pixels are represented as 8-bit unsigned bytes with 0 meaning black and 255 meaning white.
		/// Pixels in <paramref name="pixels" /> array are ordered from top-left to bottom-right in horizontal rows.
		/// Size of <paramref name="pixels" /> must be equal to <c>width * height</c>.
		/// </para>
		/// </remarks>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <param name="pixels">Image pixels ordered from top-left to bottom-right in horizontal rows.</param>
        /// <param name="options">Additional information about the image or <c>null</c> for default options.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="pixels" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="width" /> or <paramref name="height" /> is not positive
		/// or when <paramref name="pixels" /> length is not <c>width * height</c>.
		/// </exception>
		/// <seealso cref="FingerprintTemplate(byte[])" />
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
