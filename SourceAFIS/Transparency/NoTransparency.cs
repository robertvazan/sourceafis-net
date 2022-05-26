// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS.Transparency
{
    class NoTransparency : FingerprintTransparency
    {
        public static readonly NoTransparency Instance = new NoTransparency();
        // Dispose it immediately, so that it does not hang around in thread-local variable.
        NoTransparency() { Dispose(); }
        public override bool Accepts(string key) { return false; }
    }
}
