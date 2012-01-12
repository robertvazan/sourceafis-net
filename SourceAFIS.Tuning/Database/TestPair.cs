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

        public bool IsMatching { get { return Probe.Finger == Candidate.Finger; } }
        public bool IsNonMatching { get { return !IsMatching; } }

        public TestPair(DatabaseIndex probe, DatabaseIndex candidate)
        {
            Probe = probe;
            Candidate = candidate;
        }
    }
}
