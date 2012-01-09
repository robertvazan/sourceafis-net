using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SourceAFIS.Tuning.Database;

namespace SourceAFIS.Tuning.Reports
{
    public sealed class DatabaseStatistics
    {
        public int TotalDatabases;
        public int TotalFingers;
        public int TotalViews;
        public int MatchingPairCount;
        public int NonMatchingPairCount;

        public void Collect(DatabaseCollection database)
        {
            TotalDatabases = database.Databases.Count;
            TotalFingers = database.FingerCount;
            TotalViews = database.FpCount;
            MatchingPairCount = database.GetMatchingPairCount();
            NonMatchingPairCount = database.GetNonMatchingPairCount();
        }
    }
}
