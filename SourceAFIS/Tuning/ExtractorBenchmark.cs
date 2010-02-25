using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Extraction;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Visualization;

namespace SourceAFIS.Tuning
{
    public sealed class ExtractorBenchmark
    {
        public TestDatabase Database = new TestDatabase();
        public Extractor Extractor = new Extractor();
        public int MaxTotalSeconds = 300;

        public struct Statistics
        {
            public double Seconds;
            public int Minutiae;
            public int TemplateBytes;
        }

        public bool Timeout;
        public int Count;
        public Statistics Totals;
        public Statistics Average;

        public void Run()
        {
            Totals = new Statistics();
            Count = 0;
            SerializedFormat templateFormat = new SerializedFormat();

            BenchmarkTimer timer = new BenchmarkTimer();
            timer.Start();
            Timeout = false;
            
            foreach (TestDatabase.View view in Database.AllViews)
            {
                ColorB[,] image = ImageIO.Load(view.Path);
                byte[,] grayscale = PixelFormat.ToByte(image);
                TemplateBuilder builder = Extractor.Extract(grayscale, 500);
                view.Template = templateFormat.Export(builder);

                Totals.Minutiae += view.Template.Minutiae.Length;
                Totals.TemplateBytes += templateFormat.Serialize(view.Template).Length;
                ++Count;

                timer.Update();
                if (timer.Elapsed.TotalSeconds > MaxTotalSeconds)
                {
                    Timeout = true;
                    break;
                }
            }

            timer.Stop();
            Totals.Seconds = timer.TotalTime.TotalSeconds;

            Average.Seconds = Totals.Seconds / Count;
            Average.Minutiae = Totals.Minutiae / Count;
            Average.TemplateBytes = Totals.TemplateBytes / Count;
        }
    }
}
