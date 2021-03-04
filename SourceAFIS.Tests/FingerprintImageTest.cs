// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using NUnit.Framework;
using SourceAFIS;

namespace SourceAFIS.Tests
{
	public class FingerprintImageTest
	{
		static FingerprintImage Decode(byte[] bytes)
		{
			using (var stream = new MemoryStream(bytes))
			{
				using (var image = Image.FromStream(stream))
				{
					using (var bitmap = new Bitmap(image))
					{
						var grayscale = new byte[bitmap.Width * bitmap.Height];
						var locked = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
						try
						{
							var pixels = new byte[locked.Stride * locked.Height];
							Marshal.Copy(locked.Scan0, pixels, 0, pixels.Length);
							for (int y = 0; y < bitmap.Height; ++y)
							{
								for (int x = 0; x < bitmap.Width; ++x)
								{
									int sum = 0;
									for (int c = 0; c < 3; ++c)
										sum += pixels[y * locked.Stride + x * 3 + c];
									grayscale[y * bitmap.Width + x] = (byte)(sum / 3);
								}
							}
						}
						finally
						{
							bitmap.UnlockBits(locked);
						}
						return FingerprintImage.Grayscale(bitmap.Width, bitmap.Height, grayscale);
					}
				}
			}
		}

		public static FingerprintImage Probe() { return Decode(TestResources.Probe()); }
		public static FingerprintImage Matching() { return Decode(TestResources.Matching()); }
		public static FingerprintImage Nonmatching() { return Decode(TestResources.Nonmatching()); }
		public static FingerprintImage ProbeGray() { return FingerprintImage.Grayscale(332, 533, TestResources.ProbeGray()); }
		public static FingerprintImage MatchingGray() { return FingerprintImage.Grayscale(320, 407, TestResources.MatchingGray()); }
		public static FingerprintImage NonmatchingGray() { return FingerprintImage.Grayscale(333, 435, TestResources.NonmatchingGray()); }

		[Test]
		public void DecodeGray()
		{
			double score = new FingerprintMatcher(new FingerprintTemplate(ProbeGray()))
				.Match(new FingerprintTemplate(MatchingGray()));
			Assert.That(score, Is.GreaterThan(40));
		}
	}
}
