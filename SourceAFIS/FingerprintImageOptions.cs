// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	public class FingerprintImageOptions
	{
		double DpiValue = 500;

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
