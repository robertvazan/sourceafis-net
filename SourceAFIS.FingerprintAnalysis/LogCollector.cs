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
                data.Blocks = Logger.Retrieve<BlockMap>("Extractor.BlockMap");
                data.BlockContrast = Logger.Retrieve<byte[,]>("Extractor.Mask.Contrast");
                data.AbsoluteContrast = Logger.Retrieve<BinaryMap>("Extractor.Mask.AbsoluteContrast");
                data.RelativeContrast = Logger.Retrieve<BinaryMap>("Extractor.Mask.RelativeContrast");
                data.LowContrastMajority = Logger.Retrieve<BinaryMap>("Extractor.Mask.LowContrastMajority");
                data.SegmentationMask = Logger.Retrieve<BinaryMap>("Extractor.Mask");
                data.Equalized = Logger.Retrieve<float[,]>("Extractor.Equalizer");
                data.Orientation = Logger.Retrieve<byte[,]>("Extractor.Orientation");
                data.SmoothedRidges = Logger.Retrieve<float[,]>("Extractor.RidgeSmoother");
                data.OrthogonalSmoothing = Logger.Retrieve<float[,]>("Extractor.OrthogonalSmoother");
                data.Binarized = Logger.Retrieve<BinaryMap>("Extractor.Binarizer");
                data.BinarySmoothing = Logger.Retrieve<BinaryMap>("Extractor.BinarySmoothingResult");
                data.RemovedCrosses = Logger.Retrieve<BinaryMap>("Extractor.CrossRemover");
                data.InnerMask = Logger.Retrieve<BinaryMap>("Extractor.InnerMask");
                CollectSkeleton("[Ridges]", data.Ridges);
                CollectSkeleton("[Valleys]", data.Valleys);
                data.MinutiaCollector = Logger.Retrieve<TemplateBuilder>("Extractor.MinutiaCollector");
                data.Template = new SerializedFormat().Export(data.MinutiaCollector);
                Logger.Clear();
            }
        }

        void CollectSkeleton(string context, SkeletonData data)
        {
            data.Binarized = Logger.Retrieve<BinaryMap>("Extractor.Binarized" + context);
            data.Thinned = Logger.Retrieve<BinaryMap>("Extractor.Thinner" + context);
            data.RidgeTracer = Logger.Retrieve<SkeletonBuilder>("Extractor.RidgeTracer" + context);
            data.DotRemover = Logger.Retrieve<SkeletonBuilder>("Extractor.DotRemover" + context);
            data.PoreRemover = Logger.Retrieve<SkeletonBuilder>("Extractor.PoreRemover" + context);
            data.GapRemover = Logger.Retrieve<SkeletonBuilder>("Extractor.GapRemover" + context);
            data.TailRemover = Logger.Retrieve<SkeletonBuilder>("Extractor.TailRemover" + context);
            data.FragmentRemover = Logger.Retrieve<SkeletonBuilder>("Extractor.FragmentRemover" + context);
            data.MinutiaMask = Logger.Retrieve<SkeletonBuilder>("Extractor.MinutiaMask" + context);
            data.BranchMinutiaRemover = Logger.Retrieve<SkeletonBuilder>("Extractor.BranchMinutiaRemover" + context);
        }

        void CollectMatching()
        {
            if (Probe.InputImage != null && Candidate.InputImage != null)
            {
                ParallelMatcher.PreparedProbe prepared = Matcher.Prepare(Probe.Template);
                Matcher.Match(prepared, new Template[] { Candidate.Template });
                Match.Score = Logger.Retrieve<float>("Matcher.MinutiaMatcher.Score");
                Match.AnyMatch = Match.Score > 0;
                if (Match.AnyMatch)
                {
                    Match.Root = Logger.Retrieve<MinutiaPair>("Matcher.MinutiaMatcher.Root");
                    Match.Pairing = Logger.Retrieve<MinutiaPairing>("Matcher.MinutiaMatcher.Pairing");
                }
                Logger.Clear();
            }
        }
    }
}
