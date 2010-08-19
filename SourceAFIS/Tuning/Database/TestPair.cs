using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.Tuning.Database
{
    public struct TestPair
    {
        public DatabaseIndex Probe;
        public DatabaseIndex Candidate;

        public bool IsMatching
        {
            get
            {
                bool sameFinger = Probe.Finger == Candidate.Finger;
                bool sameView = Probe.View == Candidate.View;
                if (sameFinger && !sameView)
                    return true;
                else if (!sameFinger && sameView)
                    return false;
                else
                    throw new ApplicationException("Inconsistent fingerprint pair");
            }
        }
        public bool IsNonMatching { get { return !IsMatching; } }

        public TestPair(DatabaseIndex probe, DatabaseIndex candidate)
        {
            Probe = probe;
            Candidate = candidate;
        }
    }
}
