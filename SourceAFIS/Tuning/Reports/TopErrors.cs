using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Linq;
using SourceAFIS.General;
using SourceAFIS.Tuning.Errors;

namespace SourceAFIS.Tuning.Reports
{
    public sealed class TopErrors
    {
        public const int TopErrorCount = 20;

        public struct Pair
        {
            public ScoreTable.Index Probe;
            public ScoreTable.Index Candidate;
            [XmlAttribute]
            public float Score;
        }
        
        public struct PerDatabase
        {
            public Pair[] TopFalseAccepts;
            public Pair[] TopFalseRejects;
        }

        public PerDatabase[] Databases;

        public TopErrors(ScoreTable[] tables)
        {
            Databases = (from table in tables
                         select new PerDatabase
                         {
                             TopFalseAccepts = CollectFalseAccepts(table),
                             TopFalseRejects = CollectFalseRejects(table)
                         }).ToArray();
        }

        public static Pair[] CollectFalseAccepts(ScoreTable table)
        {
            TopListF<Pair> top = new TopListF<Pair>(TopErrorCount);
            for (int finger = 0; finger < table.Table.Length; ++finger)
                for (int view = 0; view < table.Table[finger].Length; ++view)
                    for (int pair = 0; pair < table.Table[finger][view].NonMatching.Length; ++pair)
                    {
                        Pair item = new Pair();
                        item.Probe.Finger = finger;
                        item.Probe.View = view;
                        item.Candidate = item.Probe;
                        for (int i = 0; i <= pair; ++i)
                        {
                            do
                            {
                                item.Candidate.Finger = (item.Candidate.Finger + 1) % table.Table.Length;
                            } while (view >= table.Table[item.Candidate.Finger].Length);
                        }
                        item.Score = table.Table[finger][view].NonMatching[pair];
                        top.Add(-table.Table[finger][view].NonMatching[pair], item);
                    }
            return top.GetValues();
        }

        public static Pair[] CollectFalseRejects(ScoreTable table)
        {
            TopListF<Pair> top = new TopListF<Pair>(TopErrorCount);
            for (int finger = 0; finger < table.Table.Length; ++finger)
                for (int view = 0; view < table.Table[finger].Length; ++view)
                    for (int pair = 0; pair < table.Table[finger][view].Matching.Length; ++pair)
                    {
                        Pair item = new Pair();
                        item.Probe.Finger = finger;
                        item.Probe.View = view;
                        item.Candidate.Finger = finger;
                        item.Candidate.View = (view + pair + 1) % table.Table[finger].Length;
                        item.Score = table.Table[finger][view].Matching[pair];
                        top.Add(table.Table[finger][view].Matching[pair], item);
                    }
            return top.GetValues();
        }
    }
}
