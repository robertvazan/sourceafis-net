using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.FingerprintAnalysis
{
    public class ExtractionData
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
        public BinaryMap BinarySmoothing;
        public BinaryMap RemovedCrosses;
        public BinaryMap InnerMask;
        public SkeletonData Ridges = new SkeletonData();
        public SkeletonData Valleys = new SkeletonData();
        public TemplateBuilder MinutiaCollector;
        public Template Template;
    }
}
