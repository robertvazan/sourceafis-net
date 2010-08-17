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
                report.ScoreTables[databaseIndex].Initialize(database);
                for (int fingerIndex = 0; fingerIndex < database.Fingers.Count; ++fingerIndex)
                {
                    TestDatabase.Finger finger = database.Fingers[fingerIndex];
                    for (int viewIndex = 0; viewIndex < finger.Views.Count; ++viewIndex)
                    {
                        TestDatabase.View view = finger.Views[viewIndex];
                        RunPrepare(view.Template, prepareTimer);

                        var matching = (from view2 in finger.Views
                                        where view2 != view
                                        select view2.Template).ToList();
                        report.ScoreTables[databaseIndex].Table[fingerIndex][viewIndex].Matching = RunMatch(matching, matchingTimer);

                        var nonmatching = (from finger2 in database.Fingers
                                           where finger2 != finger
                                           select finger2.Views[viewIndex].Template).ToList();
                        report.ScoreTables[databaseIndex].Table[fingerIndex][viewIndex].NonMatching = RunMatch(nonmatching, nonmatchingTimer);

                        if (prepareTimer.Elapsed.TotalSeconds + matchingTimer.Elapsed.TotalSeconds +
                            nonmatchingTimer.Elapsed.TotalSeconds > Timeout)
                        {
                            throw new TimeoutException("Timeout in matcher");
                        }
                    }
                }
            }

            report.Time.Prepare = (float)prepareTimer.Elapsed.TotalSeconds / TestDatabase.AllViews.Count();
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

        float[] RunMatch(List<Template> templates, Stopwatch timer)
        {
            timer.Start();
            float[] scores = Matcher.Match(templates);
            timer.Stop();
            return scores;
        }
    }
}
