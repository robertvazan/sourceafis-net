using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SourceAFIS.Tuning.Errors;

namespace SourceAFIS.Tuning.Reports
{
    public sealed class MatcherReport
    {
        public MatcherTimings Time = new MatcherTimings();

        public ScoreTable[] ScoreTables;
        public AccuracyStatistics[] Accuracy;

        public void SetDatabaseCount(int count)
        {
            ScoreTables = new ScoreTable[count];
            for (int i = 0; i < count; ++i)
                ScoreTables[i] = new ScoreTable();
        }

        public void ComputeStatistics()
        {
            Accuracy = (from measure in AccuracyMeasure.AccuracyLandscape.AsParallel().AsOrdered()
                        select new AccuracyStatistics(ScoreTables, measure)).ToArray();
        }

        public void Save(string folder)
        {
            Directory.CreateDirectory(folder);

            using (FileStream stream = File.Open(Path.Combine(folder, "MatcherTime.xml"), FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MatcherTimings));
                serializer.Serialize(stream, Time);
            }

            SaveScoreTables(Path.Combine(folder, "ScoreTable"));
            SaveAccuracy(Path.Combine(folder, "Accuracy"));
        }

        void SaveScoreTables(string folder)
        {
            Directory.CreateDirectory(folder);

            Parallel.For(0, ScoreTables.Length, delegate(int i)
            {
                using (FileStream stream = File.Open(Path.Combine(folder, String.Format("Database{0}.xml", i + 1)), FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ScoreTable));
                    serializer.Serialize(stream, ScoreTables[i]);
                }
            });
        }

        void SaveAccuracy(string folder)
        {
            Directory.CreateDirectory(folder);

            foreach(var accuracy in Accuracy)
                accuracy.Save(Path.Combine(folder, accuracy.Name), true);
        }
    }
}
