using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Matching
{
    public interface IRootSelector
    {
        void SetProbe(Template probe);
        void SetCandidate(Template candidate);
        IEnumerable<MinutiaPair> GetRoots();
    }
}
