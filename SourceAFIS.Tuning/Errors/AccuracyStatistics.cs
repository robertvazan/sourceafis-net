using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SourceAFIS.General;
using SourceAFIS.Tuning.Reports;
using SourceAFIS.Visualization;

namespace SourceAFIS.Tuning.Errors
{
    public sealed class AccuracyStatistics
    {
        public sealed class PerDatabaseInfo
        {
            [XmlIgnore]
            public ScoreTable CombinedScores;
            [XmlIgnore]
            public ROCCurve ROC = new ROCCurve();
            public ErrorRange Range = new ErrorRange();
            public float Scalar;
            public float Separation;

            PerDatabaseInfo() { }

            public PerDatabaseInfo(ScoreTable table, AccuracyMeasure measure)
            {
                CombinedScores = table.GetMultiFingerTable(measure.MultiFingerPolicy);
                ROC.Compute(CombinedScores);
                Range.Compute(ROC, measure.ErrorPolicyFunction);
                Scalar = measure.ScalarMeasure.Measure(Range.Rate);
                Separation = measure.Separation.Measure(CombinedScores);
            }

            public void Save(string folder)
            {
                Directory.CreateDirectory(folder);

                using (FileStream stream = File.Open(Path.Combine(folder, "CombinedScores.xml"), FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ScoreTable));
                    serializer.Serialize(stream, CombinedScores);
                }

                using (FileStream stream = File.Open(Path.Combine(folder, "ROC.xml"), FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ROCCurve));
                    serializer.Serialize(stream, ROC);
                }

                WpfIO.Save(WpfIO.GetBitmapSource(PixelFormat.ToByte(new ROCGraph().Draw(ROC))), Path.Combine(folder, "ROC.png"));
            }
        }

        public string Name;
        public PerDatabaseInfo[] PerDatabase;
        public float AverageError;
        public float Separation;
        [XmlIgnore]
        public TopErrors TopErrors;

        AccuracyStatistics() { }

        public AccuracyStatistics(ScoreTable[] tables, AccuracyMeasure measure)
        {
            Name = measure.Name;
            PerDatabase = (from table in tables.AsParallel().AsOrdered()
                           select new PerDatabaseInfo(table, measure)).ToArray();
            AverageError = PerDatabase.Average(db => db.Scalar);
            Separation = PerDatabase.Average(db => db.Separation);
            TopErrors = new TopErrors(tables);
        }

        public void Save(string folder, bool perDatabase)
        {
            Directory.CreateDirectory(folder);

            using (FileStream stream = File.Open(Path.Combine(folder, "Accuracy.xml"), FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AccuracyStatistics));
                serializer.Serialize(stream, this);
            }

            using (FileStream stream = File.Open(Path.Combine(folder, "TopErrors.xml"), FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TopErrors));
                serializer.Serialize(stream, TopErrors);
            }

            if (perDatabase)
                Parallel.For(0, PerDatabase.Length,
                    (i) => { PerDatabase[i].Save(Path.Combine(folder, String.Format("Database{0}", i + 1))); });
        }
    }
}
