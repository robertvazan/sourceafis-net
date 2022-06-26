// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Collections.Generic;
using SourceAFIS.Engine.Features;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Templates
{
    record FeatureTemplate(IntPoint Size, List<Minutia> Minutiae)
    {
    }
}
