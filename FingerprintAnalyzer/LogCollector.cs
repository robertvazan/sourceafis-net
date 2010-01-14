using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Extraction;
using SourceAFIS.Extraction.Model;
using SourceAFIS.General;
using SourceAFIS.Visualization;

namespace FingerprintAnalyzer
{
    sealed class LogCollector
    {
        public sealed class SkeletonData
        {
            public BinaryMap Binarized;
            public BinaryMap Thinned;
            public SkeletonBuilder RidgeTracer;
            public SkeletonBuilder DotRemover;
            public SkeletonBuilder PoreRemover;
            public SkeletonBuilder TailRemover;
            public SkeletonBuilder FragmentRemover;
            public SkeletonBuilder MinutiaMask;
        }

        public sealed class ExtractionData
        {
            public byte[,] InputImage;
            public BlockMap Blocks;
            public byte[,] BlockContrast;
            public BinaryMap AbsoluteContrast;
            public BinaryMap RelativeContrast;
            public BinaryMap LowContrastMajority;
            public BinaryMap SegmentationMask;
            public float[,] Equalized;
            public byte[,] Orientation;
            public float[,] SmoothedRidges;
            public float[,] OrthogonalSmoothing;
            public BinaryMap Binarized;
            public BinaryMap BinarySmoothingZeroes;
            public BinaryMap BinarySmoothingOnes;
            public BinaryMap RemovedCrosses;
            public BinaryMap InnerMask;
            public SkeletonData Ridges = new SkeletonData();
            public SkeletonData Valleys = new SkeletonData();
        }

        public ExtractionData Probe = new ExtractionData();

        Extractor Extractor = new Extractor();

        public LogCollector()
        {
            Logger.Resolver.Scan(Extractor, "Extractor");
            Logger.Filter = delegate(string path) { return true; };
        }

        public void Collect()
        {
            Extractor.Extract(Probe.InputImage, 500);
            Probe.Blocks = Logger.Retrieve<BlockMap>("Extractor.BlockMap");
            Probe.BlockContrast = Logger.Retrieve<byte[,]>("Extractor.Mask.Contrast");
            Probe.AbsoluteContrast = Logger.Retrieve<BinaryMap>("Extractor.Mask.AbsoluteContrast");
            Probe.RelativeContrast = Logger.Retrieve<BinaryMap>("Extractor.Mask.RelativeContrast");
            Probe.LowContrastMajority = Logger.Retrieve<BinaryMap>("Extractor.Mask.LowContrastMajority");
            Probe.SegmentationMask = Logger.Retrieve<BinaryMap>("Extractor.Mask");
            Probe.Equalized = Logger.Retrieve<float[,]>("Extractor.Equalizer");
            Probe.Orientation = Logger.Retrieve<byte[,]>("Extractor.Orientation");
            Probe.SmoothedRidges = Logger.Retrieve<float[,]>("Extractor.RidgeSmoother");
            Probe.OrthogonalSmoothing = Logger.Retrieve<float[,]>("Extractor.OrthogonalSmoother");
            Probe.Binarized = Logger.Retrieve<BinaryMap>("Extractor.Binarizer");
            Probe.BinarySmoothingZeroes = Logger.Retrieve<BinaryMap>("Extractor.BinarySmoother", 0);
            Probe.BinarySmoothingOnes = Logger.Retrieve<BinaryMap>("Extractor.BinarySmoother", 1);
            Probe.RemovedCrosses = Logger.Retrieve<BinaryMap>("Extractor.CrossRemover");
            Probe.InnerMask = Logger.Retrieve<BinaryMap>("Extractor.InnerMask");
            CollectSkeleton("[Ridges]", Probe.Ridges);
            CollectSkeleton("[Valleys]", Probe.Valleys);
            Logger.Clear();
        }

        void CollectSkeleton(string context, SkeletonData data)
        {
            data.Binarized = Logger.Retrieve<BinaryMap>("Extractor.Binarized" + context);
            data.Thinned = Logger.Retrieve<BinaryMap>("Extractor.Thinner" + context);
            data.RidgeTracer = Logger.Retrieve<SkeletonBuilder>("Extractor.RidgeTracer" + context);
            data.DotRemover = Logger.Retrieve<SkeletonBuilder>("Extractor.DotRemover" + context);
            data.PoreRemover = Logger.Retrieve<SkeletonBuilder>("Extractor.PoreRemover" + context);
            data.TailRemover = Logger.Retrieve<SkeletonBuilder>("Extractor.TailRemover" + context);
            data.FragmentRemover = Logger.Retrieve<SkeletonBuilder>("Extractor.FragmentRemover" + context);
            data.MinutiaMask = Logger.Retrieve<SkeletonBuilder>("Extractor.MinutiaMask" + context);
        }
    }
}
