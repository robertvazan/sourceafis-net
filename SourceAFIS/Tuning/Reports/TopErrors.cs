using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Linq;
using SourceAFIS.General;
using SourceAFIS.Tuning.Errors;
using SourceAFIS.Tuning.Database;

namespace SourceAFIS.Tuning.Reports
{
    public sealed class TopErrors
    {
        public const int TopErrorCount = 20;

        public struct TopPair
        {
            public TestPair Pair;
            [XmlAttribute]
            public float Score;
        }
        
        public struct PerDatabase
        {
            public TopPair[] TopFalseAccepts;
            public TopPair[] TopFalseRejects;
        }

        public PerDatabase[] Databases;

        TopErrors() { }

        public TopErrors(ScoreTable[] tables)
        {
            Databases = (from table in tables
                         select new PerDatabase
                         {
                             TopFalseAccepts = CollectFalseAccepts(table).ToArray(),
                             TopFalseRejects = CollectFalseRejects(table).ToArray()
                         }).ToArray();
        }

        public static IEnumerable<TopPair> CollectFalseAccepts(ScoreTable table)
        {
            return (from pair in table.NonMatchingPairs
                    orderby table[pair] descending
                    select new TopPair { Pair = pair, Score = table[pair] }).Take(TopErrorCount);
        }

        public static IEnumerable<TopPair> CollectFalseRejects(ScoreTable table)
        {
            return (from pair in table.MatchingPairs
                    orderby table[pair]
                    select new TopPair { Pair = pair, Score = table[pair] }).Take(TopErrorCount);
        }
    }
}
