// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
    /// <summary>Additional information about fingerprint image.</summary>
    /// <remarks>
    /// <c>FingerprintImageOptions</c> can be passed to <see cref="FingerprintImage" /> constructor
    /// to provide additional information about fingerprint image that supplements raw pixel data.
    /// Since SourceAFIS algorithm is not scale-invariant, all images should have DPI configured explicitly by setting <see cref="Dpi" />.
    /// </remarks>
    /// <seealso cref="FingerprintImage" />
    public class FingerprintImageOptions
    {
        double dpi = 500;

        /// <summary>Gets or sets image resolution.</summary>
        /// <value>Image resolution in DPI (dots per inch), usually around 500. Default DPI is 500.</value>
        /// <remarks>
        /// SourceAFIS algorithm is not scale-invariant. Fingerprints with incorrectly configured DPI may fail to match.
        /// Check your fingerprint reader specification for correct DPI value.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when DPI is non-positive, impossibly low, or impossibly high.</exception>
        public double Dpi
        {
            get => dpi;
            set
            {
                if (value < 20 || value > 20_000)
                    throw new ArgumentOutOfRangeException();
                dpi = value;
            }
        }
    }
}
