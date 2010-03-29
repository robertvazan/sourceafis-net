using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Tuning.Reports
{
    public sealed class DatabaseStatistics
    {
        public int TotalDatabases;
        public int TotalFingers;
        public int TotalViews;
        public int MatchingPairCount;
        public int NonMatchingPairCount;

        public void Collect(TestDatabase database)
        {
            TotalDatabases = database.Databases.Count;
            TotalFingers = database.GetFingerCount();
            TotalViews = database.GetFingerprintCount();
            MatchingPairCount = database.GetMatchingPairCount();
            NonMatchingPairCount = database.GetNonMatchingPairCount();
        }
    }
}
