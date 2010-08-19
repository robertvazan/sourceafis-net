using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Matching;
using SourceAFIS.Tuning.Errors;
using SourceAFIS.Tuning.Reports;
using SourceAFIS.Tuning.Database;

namespace SourceAFIS.Tuning
{
    public sealed class MatcherBenchmark
    {
        public DatabaseCollection TestDatabase = new DatabaseCollection();
        public Matcher Matcher = new Matcher();
        public float Timeout = 300;

        public MatcherReport Run()
        {
            Matcher.Initialize();

            MatcherReport report = new MatcherReport();
            report.SetDatabaseCount(TestDatabase.Databases.Count);

            Stopwatch prepareTimer = new Stopwatch();
            Stopwatch matchingTimer = new Stopwatch();
            Stopwatch nonmatchingTimer = new Stopwatch();

            for (int databaseIndex = 0; databaseIndex < TestDatabase.Databases.Count; ++databaseIndex)
            {
                TestDatabase database = TestDatabase.Databases[databaseIndex];
                report.ScoreTables[databaseIndex].Initialize(database.Fingers.Count, database.ViewCount);
                foreach (DatabaseIndex probe in database.AllIndexes)
                {
                    RunPrepare(database[probe].Template, prepareTimer);
                    CollectMatches(database, database.GetMatchingPairs(probe), report.ScoreTables[databaseIndex], matchingTimer);
                    CollectMatches(database, database.GetNonMatchingPairs(probe), report.ScoreTables[databaseIndex], nonmatchingTimer);

                    if (prepareTimer.Elapsed.TotalSeconds + matchingTimer.Elapsed.TotalSeconds +
                        nonmatchingTimer.Elapsed.TotalSeconds > Timeout)
                    {
                        throw new TimeoutException("Timeout in matcher");
                    }
                }
            }

            report.Time.Prepare = (float)prepareTimer.Elapsed.TotalSeconds / TestDatabase.FpCount;
            report.Time.Matching = (float)matchingTimer.Elapsed.TotalSeconds / TestDatabase.GetMatchingPairCount();
            report.Time.NonMatching = (float)nonmatchingTimer.Elapsed.TotalSeconds / TestDatabase.GetNonMatchingPairCount();

            report.ComputeStatistics();

            return report;
        }

        void RunPrepare(Template template, Stopwatch timer)
        {
            timer.Start();
            Matcher.Prepare(template);
            timer.Stop();
        }

        void CollectMatches(TestDatabase database, IEnumerable<TestPair> pairSource, ScoreTable table, Stopwatch timer)
        {
            var pairs = pairSource.ToList();
            var templates = from pair in pairs
                            select database[pair.Candidate].Template;
            var scores = RunMatch(templates.ToList(), timer);
            foreach (int i in Enumerable.Range(0, scores.Length))
                table[pairs[i]] = scores[i];
        }

        float[] RunMatch(List<Template> templates, Stopwatch timer)
        {
            timer.Start();
            float[] scores = Matcher.Match(templates);
            timer.Stop();
            return scores;
        }
    }
}
