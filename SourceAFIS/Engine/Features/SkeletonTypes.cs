// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS.Engine.Features
{
    static class SkeletonTypes
    {
        public static string Prefix(this SkeletonType type)
        {
            return type switch
            {
                SkeletonType.Ridges => "ridges-",
                SkeletonType.Valleys => "valleys-",
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };
        }
    }
}
