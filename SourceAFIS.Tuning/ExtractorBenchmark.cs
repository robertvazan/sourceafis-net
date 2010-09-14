using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using SourceAFIS.General;
using SourceAFIS.Extraction;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Visualization;
using SourceAFIS.Tuning.Reports;
using SourceAFIS.Tuning.Database;

namespace SourceAFIS.Tuning
{
    public sealed class ExtractorBenchmark
    {
        public DatabaseCollection Database = new DatabaseCollection();
        public Extractor Extractor = new Extractor();
        public float Timeout = 300;

        public ExtractorReport Run()
        {
            ExtractorReport report = new ExtractorReport();
            report.Templates = Database.Clone();

            int count = 0;
            SerializedFormat serializedFormat = new SerializedFormat();
            CompactFormat compactFormat = new CompactFormat();

            Stopwatch timer = new Stopwatch();
            timer.Start();

            foreach (TestDatabase database in report.Templates.Databases)
            {
                foreach (DatabaseIndex index in database.AllIndexes)
                {
                    byte[,] grayscale = ImageIO.Load(database[index].FilePath);
                    TemplateBuilder builder = Extractor.Extract(grayscale, 500);
                    Template template = serializedFormat.Export(builder);
                    database[index].Template = template;

                    report.MinutiaCount += template.Minutiae.Length;
                    report.TemplateSize += compactFormat.Export(builder).Length;
                    ++count;

                    if (timer.Elapsed.TotalSeconds > Timeout)
                        throw new TimeoutException("Timeout in extractor");
                }
            }

            timer.Stop();
            report.Time = (float)(timer.Elapsed.TotalSeconds / count);

            report.MinutiaCount /= count;
            report.TemplateSize /= count;
            return report;
        }
    }
}
