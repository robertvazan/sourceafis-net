using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using SourceAFIS.Tuning.Errors;
using SourceAFIS.Visualization;

namespace SourceAFIS.Tuning.Reports
{
    public sealed class MatcherReport
    {
        public MatcherTimings Time = new MatcherTimings();

        public ScoreTable[] ScoreTables;
        public ROCCurve[] ROCs;
        public MultiFingerStatistics[] PerDatabaseErrors;
        public MultiFingerStatistics AverageErrors = new MultiFingerStatistics();

        public void SetDatabaseCount(int count)
        {
            ScoreTables = new ScoreTable[count];
            ROCs = new ROCCurve[count];
            PerDatabaseErrors = new MultiFingerStatistics[count];

            for (int i = 0; i < count; ++i)
            {
                ScoreTables[i] = new ScoreTable();
                ROCs[i] = new ROCCurve();
                PerDatabaseErrors[i] = new MultiFingerStatistics();
            }
        }

        public void Save(string folder)
        {
            Directory.CreateDirectory(folder);

            using (FileStream stream = File.OpenWrite(Path.Combine(folder, "Time.xml")))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MatcherTimings));
                serializer.Serialize(stream, Time);
            }

            using (FileStream stream = File.OpenWrite(Path.Combine(folder, "Errors.xml")))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MultiFingerStatistics));
                serializer.Serialize(stream, AverageErrors);
            }

            for (int i = 0; i < ScoreTables.Length; ++i)
                SaveDatabase(Path.Combine(folder, String.Format("Database{0}", i + 1)), i);
        }

        void SaveDatabase(string folder, int index)
        {
            Directory.CreateDirectory(folder);

            using (FileStream stream = File.OpenWrite(Path.Combine(folder, "Errors.xml")))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MultiFingerStatistics));
                serializer.Serialize(stream, PerDatabaseErrors[index]);
            }

            using (FileStream stream = File.OpenWrite(Path.Combine(folder, "ScoreTable.xml")))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ScoreTable));
                serializer.Serialize(stream, ScoreTables[index]);
            }

            using (FileStream stream = File.OpenWrite(Path.Combine(folder, "ROC.xml")))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ROCCurve));
                serializer.Serialize(stream, ROCs[index]);
            }

            ImageIO.CreateBitmap(PixelFormat.ToColorB(new ROCGraph().Draw(ROCs[index]))).Save(Path.Combine(folder, "ROC.png"));
        }
    }
}
