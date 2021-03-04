// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace SourceAFIS.Cmd
{
	class SampleFingerprint
	{
		public readonly SampleDataset Dataset;
		public readonly int Id;
		public SampleFingerprint(SampleDataset dataset, int id)
		{
			Dataset = dataset;
			Id = id;
		}
		public String Name { get { return Dataset.Layout.Name(Id); } }
		public byte[] Load() { return File.ReadAllBytes(Path.Combine(SampleDownload.Location(Dataset.Name), Dataset.Layout.Filename(Id))); }
		public FingerprintImage Decode()
		{
			using (var stream = new MemoryStream(Load()))
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
						return FingerprintImage.Grayscale(bitmap.Width, bitmap.Height, grayscale, new FingerprintImageOptions() { Dpi = Dataset.Dpi });
					}
				}
			}
		}
	}
}
