using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Matching;

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

        public double[] PerDatabaseEER;
        public double EER;

        public void Run()
        {
            Matches.ScoreTable = InitScoreTable();
            NonMatches.ScoreTable = InitScoreTable();
            for (int databaseIndex = 0; databaseIndex < TestDatabase.Databases.Count; ++databaseIndex)
            {
                TestDatabase.Database database = TestDatabase.Databases[databaseIndex];
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
                        Matches.ScoreTable[databaseIndex][fingerIndex][viewIndex] = RunMatch(matching, Matches);

                        List<Template> nonmatching = new List<Template>();
                        for (int candidateFinger = 0; candidateFinger < database.Fingers.Count; ++candidateFinger)
                            if (candidateFinger != fingerIndex)
                                nonmatching.Add(database.Fingers[candidateFinger].Views[viewIndex].Template);
                        NonMatches.ScoreTable[databaseIndex][fingerIndex][viewIndex] = RunMatch(nonmatching, NonMatches);
                    }
                }
            }
            Summarize();
        }

        float[][][][] InitScoreTable()
        {
            float[][][][] result = new float[TestDatabase.Databases.Count][][][];
            for (int database = 0; database < result.Length; ++database)
            {
                result[database] = new float[TestDatabase.Databases[database].Fingers.Count][][];
                for (int finger = 0; finger < result[database].Length; ++finger)
                    result[database][finger] = new float[TestDatabase.Databases[database].Fingers[finger].Views.Count][];
            }
            return result;
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
            SummarizeScores();
        }

        struct ScoreRecord
        {
            public float Score;
            public int Matches;
            public int NonMatches;

            public ScoreRecord(float score, bool match)
            {
                Score = score;
                Matches = match ? 1 : 0;
                NonMatches = match ? 0 : 1;
            }
        }

        void SummarizeScores()
        {
            PerDatabaseEER = new double[TestDatabase.Databases.Count];
            for (int database = 0; database < TestDatabase.Databases.Count; ++database)
            {
                List<ScoreRecord> mixed = new List<ScoreRecord>();
                int totalMatching = 0;
                foreach (float[][] finger in Matches.ScoreTable[database])
                    foreach (float[] view in finger)
                    {
                        foreach (float score in view)
                            mixed.Add(new ScoreRecord(score, true));
                        totalMatching += view.Length;
                    }
                int totalNonMatching = 0;
                foreach (float[][] finger in NonMatches.ScoreTable[database])
                    foreach (float[] view in finger)
                    {
                        foreach (float score in view)
                            mixed.Add(new ScoreRecord(score, false));
                        totalNonMatching += view.Length;
                    }
                mixed.Sort(delegate(ScoreRecord left, ScoreRecord right) { return Calc.Compare(left.Score, right.Score); });

                List<ScoreRecord> compact = new List<ScoreRecord>();
                ScoreRecord last = mixed[0];
                for (int i = 1; i < mixed.Count; ++i)
                {
                    if (mixed[i].Score > last.Score)
                    {
                        compact.Add(last);
                        last = mixed[i];
                    }
                    else
                    {
                        last.Matches += mixed[i].Matches;
                        last.NonMatches += mixed[i].NonMatches;
                    }
                }
                compact.Add(last);

                PerDatabaseEER[database] = 0.5;
                int matchesSoFar = 0;
                int nonmatchesSoFar = 0;
                foreach (ScoreRecord record in compact)
                {
                    matchesSoFar += record.Matches;
                    nonmatchesSoFar += record.NonMatches;
                    int falseMatches = totalNonMatching - nonmatchesSoFar;
                    int falseNonMatches = matchesSoFar;
                    double FAR = falseMatches / (double)totalNonMatching;
                    double FRR = falseNonMatches / (double)totalMatching;
                    double PerThresholdEER = (FAR + FRR) / 2;
                    if (PerThresholdEER < PerDatabaseEER[database])
                        PerDatabaseEER[database] = PerThresholdEER;
                }
            }

            EER = 0;
            foreach (double eer in PerDatabaseEER)
                EER += eer;
            EER /= PerDatabaseEER.Length;
        }
    }
}
