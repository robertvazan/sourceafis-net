// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using SourceAFIS.Primitives;

namespace SourceAFIS.Matcher
{
    class MatcherThread
    {
        [ThreadStatic]
        static MatcherThread current;
        public readonly MinutiaPairPool Pool = new MinutiaPairPool();
        public readonly RootList Roots;
        public readonly PairingGraph Pairing;
        public PriorityQueue<MinutiaPair> Queue = new PriorityQueue<MinutiaPair>(Comparer<MinutiaPair>.Create((a, b) => a.Distance.CompareTo(b.Distance)));
        public ScoringData Score = new ScoringData();
        public static MatcherThread Current
        {
            get
            {
                if (current == null)
                    current = new MatcherThread();
                return current;
            }
        }
        public MatcherThread()
        {
            Roots = new RootList(Pool);
            Pairing = new PairingGraph(Pool);
        }
        public static void Kill() => current = null;
    }
}
