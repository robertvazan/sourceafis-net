using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Templates;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.Matching
{
    public sealed class ProbeIndex
    {
        public Template Template;
        public EdgeTable Edges;
        public EdgeHash EdgeHash;
    }
}
