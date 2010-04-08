using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching.Minutia
{
    public interface IRootSelector
    {
        IEnumerable<MinutiaPair> GetRoots(Template probe, Template candidate);
    }
}
