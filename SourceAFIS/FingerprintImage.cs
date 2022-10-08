// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SourceAFIS.Engine.Primitives;

// TODO: Research options for WSQ support.
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
    /// </remarks>
    /// <seealso cref="FingerprintImageOptions" />
    /// <seealso cref="FingerprintTemplate" />
    public class FingerprintImage
    {
        internal DoubleMatrix Matrix;
        internal double Dpi;

        /// <summary>Reads raw grayscale fingerprint image from byte array.</summary>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <param name="pixels">Image pixels ordered from top-left to bottom-right in horizontal rows.</param>
        /// <param name="options">Additional information about the image or <c>null</c> for default options.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="pixels" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="width" /> or <paramref name="height" /> is not positive
        /// or when <paramref name="pixels" /> length is not <c>width * height</c>.
        /// </exception>
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
        /// <seealso cref="FingerprintImage(byte[],FingerprintImageOptions)" />
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

        /// <summary>
        /// Decodes fingerprint image in standard format.
        /// </summary>
        /// <param name="image">Fingerprint image in one of the supported formats.</param>
        /// <param name="options">Additional information about the image or null for default options.</param>
        /// <exception cref="ArgumentNullException">Thrown when image is null.</exception>
        /// <exception cref="ArgumentException">Thrown when image cannot be decoded.</exception>
        /// <remarks>
        /// <para>
        /// The image must contain black fingerprint on white background
        /// in resolution specified by setting <see cref="FingerprintImageOptions.Dpi" />.
        /// </para>
        /// <para>
        /// The image may be in any format commonly used to store fingerprint images, including PNG, JPEG, BMP, and TIFF.
        /// SourceAFIS will try to decode the image using <a href="https://sixlabors.com/products/imagesharp/">ImageSharp</a> library.
        /// Note that ImageSharp might not support all versions and variations of the mentioned formats.
        /// </para>
        /// </remarks>
        /// <seealso cref="FingerprintImage(int,int,byte[],FingerprintImageOptions)" />
        public FingerprintImage(byte[] image, FingerprintImageOptions options = null)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            try
            {
                using (var decoded = Image.Load<Argb32>(image))
                {
                    Matrix = new DoubleMatrix(decoded.Width, decoded.Height);
                    decoded.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < Matrix.Height; ++y)
                        {
                            var span = accessor.GetRowSpan(y);
                            for (int x = 0; x < Matrix.Width; ++x)
                            {
                                var pixel = span[x];
                                var color = pixel.R + pixel.G + pixel.B;
                                Matrix[x, y] = 1 - color * (1.0 / (3.0 * 255.0));
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Failed to decode fingerprint image.", ex);
            }
            if (options == null)
                options = new FingerprintImageOptions();
            Dpi = options.Dpi;
        }
    }
}
