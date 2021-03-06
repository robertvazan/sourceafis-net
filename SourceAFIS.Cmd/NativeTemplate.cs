// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
﻿using System;

namespace SourceAFIS.Cmd
{
	class NativeTemplate
	{
		public static byte[] Serialized(SampleFingerprint fp)
		{
			return PersistentCache.Get("templates", fp.Path + ".cbor", () =>
			{
				return new FingerprintTemplate(fp.Decode()).ToByteArray();
			});
		}
		public static FingerprintTemplate Of(SampleFingerprint fp) { return new FingerprintTemplate(Serialized(fp)); }
	}
}
