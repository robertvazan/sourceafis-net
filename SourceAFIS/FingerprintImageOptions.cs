// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	/// <summary>Additional information about fingerprint image.</summary>
    /// <remarks>
    /// <c>FingerprintImageOptions</c> can be passed to <see cref="T:FingerprintImage" /> constructor
    /// to provide additional information about fingerprint image that supplements raw pixel data.
	/// Since SourceAFIS algorithm is not scale-invariant, all images should have DPI configured explicitly by setting <see cref="P:Dpi" />.
    /// </remarks>
    /// <seealso cref="T:FingerprintImage" />
	public class FingerprintImageOptions
	{
		double DpiValue = 500;

		/// <summary>Gets or sets image DPI.</summary>
		/// <value>Image resolution in DPI (dots per inch). Allowed range is 20 to 20,000 DPI. Default is 500 DPI.</value>
		/// <remarks>
		/// Check your fingerprint reader specification for correct DPI value.
		/// </remarks>
		/// <exception cref="T:ArgumentOutOfRangeException">Thrown when DPI is lower than 20 or higher than 20,000.</exception>
		public double Dpi
		{
			get { return DpiValue; }
			set
			{
				if (value < 20 || value > 20_000)
					throw new ArgumentOutOfRangeException();
				DpiValue = value;
			}
		}
	}
}
