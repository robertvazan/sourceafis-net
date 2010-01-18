using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Extraction;
using SourceAFIS.Extraction.Model;
using SourceAFIS.Extraction.Templates;
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
            public TemplateBuilder MinutiaCollector;
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
            Collect(Probe);
        }

        public void Collect(ExtractionData data)
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
                data.BinarySmoothingZeroes = Logger.Retrieve<BinaryMap>("Extractor.BinarySmoother", 0);
                data.BinarySmoothingOnes = Logger.Retrieve<BinaryMap>("Extractor.BinarySmoother", 1);
                data.RemovedCrosses = Logger.Retrieve<BinaryMap>("Extractor.CrossRemover");
                data.InnerMask = Logger.Retrieve<BinaryMap>("Extractor.InnerMask");
                CollectSkeleton("[Ridges]", data.Ridges);
                CollectSkeleton("[Valleys]", data.Valleys);
                data.MinutiaCollector = Logger.Retrieve<TemplateBuilder>("Extractor.MinutiaCollector");
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
            data.TailRemover = Logger.Retrieve<SkeletonBuilder>("Extractor.TailRemover" + context);
            data.FragmentRemover = Logger.Retrieve<SkeletonBuilder>("Extractor.FragmentRemover" + context);
            data.MinutiaMask = Logger.Retrieve<SkeletonBuilder>("Extractor.MinutiaMask" + context);
        }
    }
}
