// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

namespace SourceAFIS.Engine.Transparency
{
    class NoTransparency : FingerprintTransparency
    {
        public static readonly NoTransparency Instance = new NoTransparency();
        // Dispose it immediately, so that it does not hang around in thread-local variable.
        NoTransparency() => Dispose();
        public override bool Accepts(string key) => false;
    }
}
