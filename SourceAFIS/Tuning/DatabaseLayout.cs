﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS.Tuning
{
    public abstract class DatabaseLayout
    {
        public abstract int FingerCount { get; }
        public abstract int ViewCount { get; }
        
        public int FpCount { get { return FingerCount * ViewCount; } }

        public IEnumerable<DatabaseIndex> GetConsequentViews(DatabaseIndex probe)
        {
            return from offset in Enumerable.Range(1, ViewCount - 1)
                   select new DatabaseIndex(probe.Finger, (probe.View + offset) % ViewCount);
        }

        public IEnumerable<TestPair> GetMatchingPairs(DatabaseIndex probe)
        {
            return from candidate in GetConsequentViews(probe)
                   select new TestPair(probe, candidate);
        }

        public IEnumerable<DatabaseIndex> GetConsequentFingers(DatabaseIndex probe)
        {
            return from offset in Enumerable.Range(1, FingerCount - 1)
                   select new DatabaseIndex((probe.Finger + offset) % FingerCount, probe.View);
        }

        public IEnumerable<TestPair> GetNonMatchingPairs(DatabaseIndex probe)
        {
            return from candidate in GetConsequentFingers(probe)
                   select new TestPair(probe, candidate);
        }

        public IEnumerable<TestPair> GetAllPairs(DatabaseIndex probe)
        {
            return GetMatchingPairs(probe).Concat(GetNonMatchingPairs(probe));
        }

        public IEnumerable<DatabaseIndex> AllIndexes
        {
            get
            {
                return from finger in Enumerable.Range(0, FingerCount)
                       from view in Enumerable.Range(0, ViewCount)
                       select new DatabaseIndex(finger, view);
            }
        }

        public IEnumerable<TestPair> MatchingPairs
        {
            get
            {
                return from probe in AllIndexes
                       from pair in GetMatchingPairs(probe)
                       select pair;
            }
        }

        public IEnumerable<TestPair> NonMatchingPairs
        {
            get
            {
                return from probe in AllIndexes
                       from pair in GetNonMatchingPairs(probe)
                       select pair;
            }
        }

        public IEnumerable<TestPair> AllPairs { get { return MatchingPairs.Concat(NonMatchingPairs); } }
    }
}