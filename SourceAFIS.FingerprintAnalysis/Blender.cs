using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;
using SourceAFIS.General;
using SourceAFIS.Visualization;
using SourceAFIS.Matching;

namespace SourceAFIS.FingerprintAnalysis
{
    sealed class Blender
    {
        public LogDecoder Logs;
        public ExtractionData ExtractionData;
        public MatchSideData MatchSide;

        public Options Options;

        public BitmapSource OutputImage;

        readonly ColorF TransparentRed = new ColorF(1, 0, 0, 0.25f);
        readonly ColorF TransparentGreen = new ColorF(0, 1, 0, 0.25f);
        readonly ColorF LightFog = new ColorF(0.9f, 0.9f, 0.9f, 0.9f);

        delegate ColorF[,] BlendLayer(ExtractionData data);

        public void Blend()
        {
            BlendLayer[] layers = new BlendLayer[]
            {
                BlendImage,
                BlendDiff,
                BlendMarkers,
                BlendMask
            };

            Size size = ExtractionData.InputImage != null
                ? new Size(ExtractionData.InputImage.GetLength(1), ExtractionData.InputImage.GetLength(0))
                : new Size(480, 640);

            ColorF[,] output = new ColorF[size.Height, size.Width];
            for (int y = 0; y < output.GetLength(0); ++y)
                for (int x = 0; x < output.GetLength(1); ++x)
                    output[y, x] = ColorF.White;

            if (ExtractionData.InputImage != null)
            {
                foreach (BlendLayer layer in layers)
                    AlphaLayering.Layer(output, layer(ExtractionData));
            }

            OutputImage = ImageSerialization.GetBitmapSource(PixelFormat.ToColorB(output));
        }

        ColorF[,] BlendImage(ExtractionData data)
        {
            SkeletonData skeletonData = GetSkeletonData(data);
            if (Options.EnableImageDisplay)
            {
                Layer displayLayerType = Options.DisplayLayer;
                float[,] displayLayer = GlobalContrast.GetNormalized(GetLayer(displayLayerType, data, skeletonData));
                return ScalarColoring.Interpolate(GlobalContrast.GetNormalized(displayLayer), ColorF.Transparent, ColorF.Black);
            }
            else
                return GetEmptyLayer(data);
        }

        ColorF[,] BlendDiff(ExtractionData data)
        {
            if (Options.EnableImageDisplay)
            {
                SkeletonData skeletonData = GetSkeletonData(data);
                Layer displayLayerType = Options.DisplayLayer;
                Layer compareLayerType = displayLayerType;
                if (Options.CompareWith != QuickCompareType.None)
                {
                    if (Options.CompareWith == QuickCompareType.OtherLayer)
                        compareLayerType = Options.CompareWithLayer;
                    else
                    {
                        int compareLayerIndex;
                        if (Options.CompareWith == QuickCompareType.Next)
                            compareLayerIndex = (int)displayLayerType + 1;
                        else
                            compareLayerIndex = (int)displayLayerType - 1;
                        if (Enum.IsDefined(typeof(Layer), compareLayerIndex))
                            compareLayerType = (Layer)Enum.Parse(typeof(Layer), compareLayerIndex.ToString());
                    }
                }

                if (compareLayerType != displayLayerType)
                {
                    float[,] displayLayer = GlobalContrast.GetNormalized(GetLayer(displayLayerType, data, skeletonData));
                    float[,] compareLayer = GlobalContrast.GetNormalized(GetLayer(compareLayerType, data, skeletonData));
                    float[,] diff;
                    if ((int)compareLayerType < (int)displayLayerType)
                        diff = ImageDiff.Diff(compareLayer, displayLayer);
                    else
                        diff = ImageDiff.Diff(displayLayer, compareLayer);
                    if (Options.DiffMode == DiffType.Normalized)
                        diff = ImageDiff.Normalize(diff, 10);
                    if (Options.DiffMode == DiffType.Fog)
                        diff = ImageDiff.Binarize(diff, 0.05f, 0.5f);
                    if (Options.DiffMode == DiffType.Binary)
                        diff = ImageDiff.Binarize(diff, 0.05f, 1);
                    return ImageDiff.Render(diff);
                }
                else
                    return GetEmptyLayer(data);
            }
            else
                return GetEmptyLayer(data);
        }

        ColorF[,] BlendMarkers(ExtractionData data)
        {
            ColorF[,] output = GetEmptyLayer(data);
            LayerBlocks(Options.Contrast, output, PixelFormat.ToFloat(data.BlockContrast));
            LayerMask(Options.AbsoluteContrast, output, data.AbsoluteContrast, TransparentRed);
            LayerMask(Options.RelativeContrast, output, data.RelativeContrast, TransparentRed);
            LayerMask(Options.LowContrastMajority, output, data.LowContrastMajority, TransparentRed);

            return output;
        }

        ColorF[,] BlendMask(ExtractionData data)
        {
            BinaryMap mask = null;
            if (Options.Mask == MaskType.Segmentation)
                mask = BlockFiller.FillBlocks(data.SegmentationMask.GetInverted(), data.Blocks);
            if (Options.Mask == MaskType.Inner)
                mask = data.InnerMask.GetInverted();
            if (mask != null)
                return ScalarColoring.Mask(mask, ColorF.Transparent, LightFog);
            else
                return GetEmptyLayer(data);
        }

        ColorF[,] GetEmptyLayer(ExtractionData data)
        {
            return new ColorF[data.InputImage.GetLength(0), data.InputImage.GetLength(1)];
        }

        SkeletonData GetSkeletonData(ExtractionData data)
        {
            if (Options.Skeleton == SkeletonType.Ridges)
                return data.Ridges;
            else
                return data.Valleys;
        }

        float[,] GetLayer(Layer type, ExtractionData data, SkeletonData skeleton)
        {
            switch (type)
            {
                case Layer.OriginalImage: return GrayscaleInverter.GetInverted(PixelFormat.ToFloat(data.InputImage));
                case Layer.Equalized: return data.Equalized;
                case Layer.SmoothedRidges: return data.SmoothedRidges;
                case Layer.OrthogonalSmoothing: return data.OrthogonalSmoothing;
                case Layer.Binarized: return PixelFormat.ToFloat(data.Binarized);
                case Layer.BinarySmoothing: return PixelFormat.ToFloat(data.BinarySmoothing);
                case Layer.RemovedCrosses: return PixelFormat.ToFloat(data.RemovedCrosses);
                case Layer.Thinned: return PixelFormat.ToFloat(skeleton.Thinned);
                case Layer.RidgeTracer: return PixelFormat.ToFloat(SkeletonDrawer.Draw(skeleton.RidgeTracer, data.Binarized.Size));
                case Layer.DotRemover: return PixelFormat.ToFloat(SkeletonDrawer.Draw(skeleton.DotRemover, data.Binarized.Size));
                case Layer.PoreRemover: return PixelFormat.ToFloat(SkeletonDrawer.Draw(skeleton.PoreRemover, data.Binarized.Size));
                case Layer.GapRemover: return PixelFormat.ToFloat(SkeletonDrawer.Draw(skeleton.GapRemover, data.Binarized.Size));
                case Layer.TailRemover: return PixelFormat.ToFloat(SkeletonDrawer.Draw(skeleton.TailRemover, data.Binarized.Size));
                case Layer.FragmentRemover: return PixelFormat.ToFloat(SkeletonDrawer.Draw(skeleton.FragmentRemover, data.Binarized.Size));
                case Layer.MinutiaMask: return PixelFormat.ToFloat(SkeletonDrawer.Draw(skeleton.MinutiaMask, data.Binarized.Size));
                case Layer.BranchMinutiaRemover: return PixelFormat.ToFloat(SkeletonDrawer.Draw(skeleton.BranchMinutiaRemover, data.Binarized.Size));
                default: throw new AssertException();
            }
        }

        void LayerMask(bool condition, ColorF[,] output, BinaryMap mask, ColorF color)
        {
            if (condition)
            {
                BinaryMap scaled = BlockFiller.FillBlocks(mask, Logs.Probe.Blocks);
                AlphaLayering.Layer(output, ScalarColoring.Mask(scaled, ColorF.Transparent, color));
            }
        }

        void LayerBlocks(bool condition, ColorF[,] output, float[,] data)
        {
            if (condition)
            {
                GlobalContrast.Normalize(data);
                float[,] scaled = BlockFiller.FillBlocks(data, Logs.Probe.Blocks);
                AlphaLayering.Layer(output, ScalarColoring.Interpolate(scaled, TransparentRed, TransparentGreen));
            }
        }
    }
}
