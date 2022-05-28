// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
    /// <summary>Collection of methods helping with template compatibility.</summary>
    public static class FingerprintCompatibility
    {
        /// <summary>Gets version of the currently running SourceAFIS.</summary>
        /// <value>SourceAFIS version in a three-part 1.2.3 format.</value>
        /// <remarks>
        /// This is useful during upgrades when the application has to deal
        /// with possible template incompatibility between versions.
        /// </remarks>
        public static String Version => typeof(FingerprintCompatibility).Assembly.GetName().Version.ToString(3);
    }
}
