using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using SourceAFIS.Extraction;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Visualization;
using SourceAFIS.Tuning.Reports;

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
            SerializedFormat templateFormat = new SerializedFormat();

            Stopwatch timer = new Stopwatch();
            timer.Start();

            foreach (TestDatabase.View view in report.Templates.AllViews)
            {
                ColorB[,] image = ImageIO.Load(view.FilePath);
                byte[,] grayscale = PixelFormat.ToByte(image);
                TemplateBuilder builder = Extractor.Extract(grayscale, 500);
                view.Template = templateFormat.Export(builder);

                report.MinutiaCount += view.Template.Minutiae.Length;
                report.TemplateSize += templateFormat.Serialize(view.Template).Length;
                ++count;

                if (timer.Elapsed.TotalSeconds > Timeout)
                    throw new TimeoutException("Timeout in extractor");
            }

            timer.Stop();
            report.Time = (float)(timer.Elapsed.TotalSeconds / count);

            report.MinutiaCount /= count;
            report.TemplateSize /= count;
            return report;
        }
    }
}
