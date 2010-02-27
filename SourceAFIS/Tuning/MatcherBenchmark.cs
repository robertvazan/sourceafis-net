using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Matching;

namespace SourceAFIS.Tuning
{
    public sealed class MatcherBenchmark
    {
        public TestDatabase TestDatabase = new TestDatabase();
        public Matcher Matcher = new Matcher();

        public sealed class Statistics
        {
            public BenchmarkTimer Timer = new BenchmarkTimer();
            public int Count;
            public double Milliseconds;
            public List<float> Scores = new List<float>();
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

        public void Run()
        {
            foreach (TestDatabase.Database database in TestDatabase.Databases)
            {
                for (int fingerIndex = 0; fingerIndex < database.Fingers.Count; ++fingerIndex)
                {
                    TestDatabase.Finger finger = database.Fingers[fingerIndex];
                    for (int viewIndex = 0; viewIndex < finger.Views.Count; ++viewIndex)
                    {
                        TestDatabase.View view = finger.Views[viewIndex];
                        RunPrepare(view.Template);

                        for (int candidateView = 0; candidateView < finger.Views.Count; ++candidateView)
                            if (candidateView != viewIndex)
                                RunMatch(finger.Views[candidateView].Template, Matches);

                        for (int candidateFinger = 0; candidateFinger < database.Fingers.Count; ++candidateFinger)
                            if (candidateFinger != fingerIndex)
                                RunMatch(database.Fingers[candidateFinger].Views[viewIndex].Template, NonMatches);
                    }
                }
            }
            Summarize();
        }

        void RunPrepare(Template template)
        {
            Prepares.Timer.Start();
            Matcher.SelectProbe(Matcher.CreateIndex(template));
            Prepares.Timer.Stop();
            ++Prepares.Count;
        }

        void RunMatch(Template template, Statistics statistics)
        {
            statistics.Timer.Start();
            float score = Matcher.Match(template);
            statistics.Timer.Stop();
            ++statistics.Count;
            statistics.Scores.Add(score);
        }

        void Summarize()
        {
            Prepares.Milliseconds = Prepares.Timer.Accumulated.TotalMilliseconds / Prepares.Count;
            Matches.Milliseconds = Matches.Timer.Accumulated.TotalMilliseconds / Matches.Count;
            NonMatches.Milliseconds = NonMatches.Timer.Accumulated.TotalMilliseconds / NonMatches.Count;
        }
    }
}
