using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;
using SourceAFIS.Extraction;
using SourceAFIS.Extraction.Model;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.General;
using SourceAFIS.Visualization;
using SourceAFIS.Matching;
using SourceAFIS.Matching.Minutia;

namespace SourceAFIS.FingerprintAnalysis
{
    public class LogCollector
    {
        public ExtractionData Probe = new ExtractionData();
        public ExtractionData Candidate = new ExtractionData();
        public MatchData Match = new MatchData();

        DetailLogger Logger = new DetailLogger();
        Extractor Extractor = new Extractor();
        ParallelMatcher Matcher = new ParallelMatcher();

        public LogCollector()
        {
            ObjectTree tree = new ObjectTree();
            tree.Scan(Extractor, "Extractor");
            tree.Scan(Matcher, "Matcher");
            Logger.Attach(tree);
        }

        public void Collect()
        {
            CollectExtraction(Probe);
            CollectExtraction(Candidate);
            CollectMatching();
        }

        public void CollectExtraction(ExtractionData data)
        {
            if (data.InputImage != null)
            {
                Extractor.Extract(data.InputImage, 500);
                data.SetProperty("Blocks", Logger.Retrieve<BlockMap>("Extractor.BlockMap"));
                data.SetProperty("BlockContrast", Logger.Retrieve<byte[,]>("Extractor.Mask.Contrast"));
                data.SetProperty("AbsoluteContrast", Logger.Retrieve<BinaryMap>("Extractor.Mask.AbsoluteContrast"));
                data.SetProperty("RelativeContrast", Logger.Retrieve<BinaryMap>("Extractor.Mask.RelativeContrast"));
                data.SetProperty("LowContrastMajority", Logger.Retrieve<BinaryMap>("Extractor.Mask.LowContrastMajority"));
                data.SetProperty("SegmentationMask", Logger.Retrieve<BinaryMap>("Extractor.Mask"));
                data.SetProperty("Equalized", Logger.Retrieve<float[,]>("Extractor.Equalizer"));
                data.SetProperty("Orientation", Logger.Retrieve<byte[,]>("Extractor.Orientation"));
                data.SetProperty("SmoothedRidges", Logger.Retrieve<float[,]>("Extractor.RidgeSmoother"));
                data.SetProperty("OrthogonalSmoothing", Logger.Retrieve<float[,]>("Extractor.OrthogonalSmoother"));
                data.SetProperty("Binarized", Logger.Retrieve<BinaryMap>("Extractor.Binarizer"));
                data.SetProperty("BinarySmoothing", Logger.Retrieve<BinaryMap>("Extractor.BinarySmoothingResult"));
                data.SetProperty("RemovedCrosses", Logger.Retrieve<BinaryMap>("Extractor.CrossRemover"));
                data.SetProperty("InnerMask", Logger.Retrieve<BinaryMap>("Extractor.InnerMask"));
                CollectSkeleton("[Ridges]", data.Ridges);
                CollectSkeleton("[Valleys]", data.Valleys);
                data.SetProperty("MinutiaCollector", Logger.Retrieve<TemplateBuilder>("Extractor.MinutiaCollector"));
                Logger.Clear();
            }
        }

        void CollectSkeleton(string context, SkeletonData data)
        {
            data.SetProperty("Binarized", Logger.Retrieve<BinaryMap>("Extractor.Binarized" + context));
            data.SetProperty("Thinned", Logger.Retrieve<BinaryMap>("Extractor.Thinner" + context));
            data.SetProperty("RidgeTracer", Logger.Retrieve<SkeletonBuilder>("Extractor.RidgeTracer" + context));
            data.SetProperty("DotRemover", Logger.Retrieve<SkeletonBuilder>("Extractor.DotRemover" + context));
            data.SetProperty("PoreRemover", Logger.Retrieve<SkeletonBuilder>("Extractor.PoreRemover" + context));
            data.SetProperty("GapRemover", Logger.Retrieve<SkeletonBuilder>("Extractor.GapRemover" + context));
            data.SetProperty("TailRemover", Logger.Retrieve<SkeletonBuilder>("Extractor.TailRemover" + context));
            data.SetProperty("FragmentRemover", Logger.Retrieve<SkeletonBuilder>("Extractor.FragmentRemover" + context));
            data.SetProperty("MinutiaMask", Logger.Retrieve<SkeletonBuilder>("Extractor.MinutiaMask" + context));
            data.SetProperty("BranchMinutiaRemover", Logger.Retrieve<SkeletonBuilder>("Extractor.BranchMinutiaRemover" + context));
        }

        void CollectMatching()
        {
            if (Probe.InputImage != null && Candidate.InputImage != null)
            {
                ParallelMatcher.PreparedProbe prepared = Matcher.Prepare(Probe.Template);
                Matcher.Match(prepared, new Template[] { Candidate.Template });
                Match.SetProperty("Score", Logger.Retrieve<float>("Matcher.MinutiaMatcher.Score"));
                if (Match.AnyMatch)
                {
                    Match.SetProperty("Root", Logger.Retrieve<MinutiaPair>("Matcher.MinutiaMatcher.Root"));
                    Match.SetProperty("Pairing", Logger.Retrieve<MinutiaPairing>("Matcher.MinutiaMatcher.Pairing"));
                }
                Logger.Clear();
            }
        }
    }
}
