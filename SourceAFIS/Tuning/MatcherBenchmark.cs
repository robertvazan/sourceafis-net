using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Matching;
using SourceAFIS.Tuning.Errors;
using SourceAFIS.Tuning.Reports;

namespace SourceAFIS.Tuning
{
    public sealed class MatcherBenchmark
    {
        public TestDatabase TestDatabase = new TestDatabase();
        public BulkMatcher Matcher = new BulkMatcher();

        public sealed class Statistics
        {
            public BenchmarkTimer Timer = new BenchmarkTimer();
            public int Count;
            public double Milliseconds;
            public float[][][][] ScoreTable;
        }

        public sealed class PrepareStats
        {
            public BenchmarkTimer Timer = new BenchmarkTimer();
            public int Count;
            public double Milliseconds;
        }

        public PrepareStats Prepares = new PrepareStats();
        public Statistics Matches = new Statistics();
        public Statistics NonMatches = new Statistics();
        public ScoreTable[] ScoreTables;

        public ROCCurve[] ROCs;
        public MultiFingerStatistics[] PerDatabaseErrors;
        public MultiFingerStatistics AverageErrors = new MultiFingerStatistics();

        public void Run()
        {
            ScoreTables = new ScoreTable[TestDatabase.Databases.Count];
            for (int databaseIndex = 0; databaseIndex < TestDatabase.Databases.Count; ++databaseIndex)
            {
                TestDatabase.Database database = TestDatabase.Databases[databaseIndex];
                ScoreTables[databaseIndex] = new ScoreTable();
                ScoreTables[databaseIndex].Initialize(database);
                for (int fingerIndex = 0; fingerIndex < database.Fingers.Count; ++fingerIndex)
                {
                    TestDatabase.Finger finger = database.Fingers[fingerIndex];
                    for (int viewIndex = 0; viewIndex < finger.Views.Count; ++viewIndex)
                    {
                        TestDatabase.View view = finger.Views[viewIndex];
                        RunPrepare(view.Template);

                        List<Template> matching = new List<Template>();
                        for (int candidateView = 0; candidateView < finger.Views.Count; ++candidateView)
                            if (candidateView != viewIndex)
                                matching.Add(finger.Views[candidateView].Template);
                        ScoreTables[databaseIndex].Table[fingerIndex][viewIndex].Matching = RunMatch(matching, Matches);

                        List<Template> nonmatching = new List<Template>();
                        for (int candidateFinger = 0; candidateFinger < database.Fingers.Count; ++candidateFinger)
                            if (candidateFinger != fingerIndex)
                                nonmatching.Add(database.Fingers[candidateFinger].Views[viewIndex].Template);
                        ScoreTables[databaseIndex].Table[fingerIndex][viewIndex].NonMatching = RunMatch(nonmatching, NonMatches);
                    }
                }
            }
            Summarize();
        }

        void RunPrepare(Template template)
        {
            Prepares.Timer.Start();
            Matcher.Prepare(template);
            Prepares.Timer.Stop();
            ++Prepares.Count;
        }

        float[] RunMatch(List<Template> templates, Statistics statistics)
        {
            statistics.Timer.Start();
            float[] scores = Matcher.Match(templates);
            statistics.Timer.Stop();
            statistics.Count += templates.Count;
            return scores;
        }

        void Summarize()
        {
            Prepares.Milliseconds = Prepares.Timer.Accumulated.TotalMilliseconds / Prepares.Count;
            Matches.Milliseconds = Matches.Timer.Accumulated.TotalMilliseconds / Matches.Count;
            NonMatches.Milliseconds = NonMatches.Timer.Accumulated.TotalMilliseconds / NonMatches.Count;

            ROCs = new ROCCurve[TestDatabase.Databases.Count];
            PerDatabaseErrors = new MultiFingerStatistics[TestDatabase.Databases.Count];
            for (int database = 0; database < TestDatabase.Databases.Count; ++database)
            {
                ROCs[database] = new ROCCurve();
                ROCs[database].Compute(ScoreTables[database]);

                PerDatabaseErrors[database] = new MultiFingerStatistics();
                PerDatabaseErrors[database].Compute(ScoreTables[database]);
            }
            AverageErrors.Average(new List<MultiFingerStatistics>(PerDatabaseErrors));
        }
    }
}
