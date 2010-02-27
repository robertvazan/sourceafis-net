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
        public BulkMatcher Matcher = new BulkMatcher();

        public sealed class Statistics
        {
            public BenchmarkTimer Timer = new BenchmarkTimer();
            public int Count;
            public double Milliseconds;
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

                        List<Template> matching = new List<Template>();
                        for (int candidateView = 0; candidateView < finger.Views.Count; ++candidateView)
                            if (candidateView != viewIndex)
                                matching.Add(finger.Views[candidateView].Template);
                        RunMatch(matching, Matches);

                        List<Template> nonmatching = new List<Template>();
                        for (int candidateFinger = 0; candidateFinger < database.Fingers.Count; ++candidateFinger)
                            if (candidateFinger != fingerIndex)
                                nonmatching.Add(database.Fingers[candidateFinger].Views[viewIndex].Template);
                        RunMatch(nonmatching, NonMatches);
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

        void RunMatch(List<Template> templates, Statistics statistics)
        {
            statistics.Timer.Start();
            Matcher.Match(templates);
            statistics.Timer.Stop();
            statistics.Count += templates.Count;
        }

        void Summarize()
        {
            Prepares.Milliseconds = Prepares.Timer.Accumulated.TotalMilliseconds / Prepares.Count;
            Matches.Milliseconds = Matches.Timer.Accumulated.TotalMilliseconds / Matches.Count;
            NonMatches.Milliseconds = NonMatches.Timer.Accumulated.TotalMilliseconds / NonMatches.Count;
        }
    }
}
