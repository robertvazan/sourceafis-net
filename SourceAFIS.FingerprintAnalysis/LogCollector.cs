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
                Extractor.Extract(data.InputImage, 500);
            data.SetProperty("Blocks", Logger.Retrieve("Extractor.BlockMap"));
            data.SetProperty("BlockContrast", Logger.Retrieve("Extractor.Mask.Contrast"));
            data.SetProperty("AbsoluteContrast", Logger.Retrieve("Extractor.Mask.AbsoluteContrast"));
            data.SetProperty("RelativeContrast", Logger.Retrieve("Extractor.Mask.RelativeContrast"));
            data.SetProperty("LowContrastMajority", Logger.Retrieve("Extractor.Mask.LowContrastMajority"));
            data.SetProperty("SegmentationMask", Logger.Retrieve("Extractor.Mask"));
            data.SetProperty("Equalized", Logger.Retrieve("Extractor.Equalizer"));
            data.SetProperty("Orientation", Logger.Retrieve("Extractor.Orientation"));
            data.SetProperty("SmoothedRidges", Logger.Retrieve("Extractor.RidgeSmoother"));
            data.SetProperty("OrthogonalSmoothing", Logger.Retrieve("Extractor.OrthogonalSmoother"));
            data.SetProperty("Binarized", Logger.Retrieve("Extractor.Binarizer"));
            data.SetProperty("BinarySmoothing", Logger.Retrieve("Extractor.BinarySmoothingResult"));
            data.SetProperty("RemovedCrosses", Logger.Retrieve("Extractor.CrossRemover"));
            data.SetProperty("InnerMask", Logger.Retrieve("Extractor.InnerMask"));
            CollectSkeleton("[Ridges]", data.Ridges);
            CollectSkeleton("[Valleys]", data.Valleys);
            data.SetProperty("MinutiaCollector", Logger.Retrieve("Extractor.MinutiaCollector"));
            Logger.Clear();
        }

        void CollectSkeleton(string context, SkeletonData data)
        {
            data.SetProperty("Binarized", Logger.Retrieve("Extractor.Binarized" + context));
            data.SetProperty("Thinned", Logger.Retrieve("Extractor.Thinner" + context));
            data.SetProperty("RidgeTracer", Logger.Retrieve("Extractor.RidgeTracer" + context));
            data.SetProperty("DotRemover", Logger.Retrieve("Extractor.DotRemover" + context));
            data.SetProperty("PoreRemover", Logger.Retrieve("Extractor.PoreRemover" + context));
            data.SetProperty("GapRemover", Logger.Retrieve("Extractor.GapRemover" + context));
            data.SetProperty("TailRemover", Logger.Retrieve("Extractor.TailRemover" + context));
            data.SetProperty("FragmentRemover", Logger.Retrieve("Extractor.FragmentRemover" + context));
            data.SetProperty("MinutiaMask", Logger.Retrieve("Extractor.MinutiaMask" + context));
            data.SetProperty("BranchMinutiaRemover", Logger.Retrieve("Extractor.BranchMinutiaRemover" + context));
        }

        void CollectMatching()
        {
            if (Probe.InputImage != null && Candidate.InputImage != null)
            {
                ParallelMatcher.PreparedProbe prepared = Matcher.Prepare(Probe.Template);
                Matcher.Match(prepared, new Template[] { Candidate.Template });
            }
            Match.SetProperty("Score", Logger.Retrieve("Matcher.MinutiaMatcher.Score"));
            Match.SetProperty("Root", Logger.Retrieve("Matcher.MinutiaMatcher.Root"));
            Match.SetProperty("Pairing", Logger.Retrieve("Matcher.MinutiaMatcher.Pairing"));
            Logger.Clear();
        }
    }
}
