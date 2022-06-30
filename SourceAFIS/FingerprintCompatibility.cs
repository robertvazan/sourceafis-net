// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
    /// <summary>Collection of methods helping with template compatibility.</summary>
    public static class FingerprintCompatibility
    {
        /// <summary>Version of the currently running SourceAFIS.</summary>
        /// <value>Semantic version in a three-part 1.2.3 format.</value>
        /// <remarks>
        /// This is useful during upgrades when the application has to deal
        /// with possible template incompatibility between versions.
        /// Versions of different language ports of SourceAFIS are not kept in sync.
        /// </remarks>
        public static string Version => typeof(FingerprintCompatibility).Assembly.GetName().Version.ToString(3);
    }
}
