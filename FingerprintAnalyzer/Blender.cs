using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SourceAFIS.General;
using SourceAFIS.Visualization;

namespace FingerprintAnalyzer
{
    sealed class Blender
    {
        public LogCollector Logs;

        public Options Options = new Options();

        public Bitmap OutputImage;

        readonly ColorF TransparentRed = new ColorF(1, 0, 0, 0.25f);
        readonly ColorF TransparentGreen = new ColorF(0, 1, 0, 0.25f);
        readonly ColorF LightFog = new ColorF(0.9f, 0.9f, 0.9f, 0.9f);

        public void Blend()
        {
            ColorF[,] output = new ColorF[Logs.Probe.InputImage.GetLength(0), Logs.Probe.InputImage.GetLength(1)];
            for (int y = 0; y < output.GetLength(0); ++y)
                for (int x = 0; x < output.GetLength(1); ++x)
                    output[y, x] = ColorF.White;

            LogCollector.SkeletonData skeletonData;
            if (Options.Probe.SkeletonType == SkeletonType.Ridges)
                skeletonData = Logs.Probe.Ridges;
            else
                skeletonData = Logs.Probe.Valleys;

            if (Options.Probe.EnableImageDisplay)
            {
                LayerType displayLayerType = Options.Probe.DisplayLayer;
                float[,] displayLayer = GlobalContrast.GetNormalized(GetLayer(displayLayerType, Logs.Probe, skeletonData));
                output = PixelFormat.ToColorF(GlobalContrast.GetNormalized(GrayscaleInverter.GetInverted(displayLayer)));

                LayerType compareLayerType = displayLayerType;
                if (Options.Probe.CompareWith != QuickCompare.None)
                {
                    if (Options.Probe.CompareWith == QuickCompare.OtherLayer)
                        compareLayerType = Options.Probe.CompareWithLayer;
                    else
                    {
                        int compareLayerIndex;
                        if (Options.Probe.CompareWith == QuickCompare.Next)
                            compareLayerIndex = (int)displayLayerType + 1;
                        else
                            compareLayerIndex = (int)displayLayerType - 1;
                        if (Enum.IsDefined(typeof(LayerType), compareLayerIndex))
                            compareLayerType = (LayerType)Enum.Parse(typeof(LayerType), compareLayerIndex.ToString());
                    }
                }

                if (compareLayerType != displayLayerType)
                {
                    float[,] compareLayer = GlobalContrast.GetNormalized(GetLayer(compareLayerType, Logs.Probe, skeletonData));
                    float[,] diff;
                    if ((int)compareLayerType < (int)displayLayerType)
                        diff = ImageDiff.Diff(compareLayer, displayLayer);
                    else
                        diff = ImageDiff.Diff(displayLayer, compareLayer);
                    if (Options.Probe.DiffType == DiffType.Normalized)
                        diff = ImageDiff.Normalize(diff, 10);
                    if (Options.Probe.DiffType == DiffType.Fog)
                        diff = ImageDiff.Binarize(diff, 0.05f, 0.5f);
                    if (Options.Probe.DiffType == DiffType.Binary)
                        diff = ImageDiff.Binarize(diff, 0.05f, 1);
                    ColorF[,] diffLayer = ImageDiff.Render(diff);
                    AlphaLayering.Layer(output, diffLayer);
                }
            }

            LayerBlocks(Options.Probe.Contrast, output, PixelFormat.ToFloat(Logs.Probe.BlockContrast));
            LayerMask(Options.Probe.AbsoluteContrast, output, Logs.Probe.AbsoluteContrast, TransparentRed);
            LayerMask(Options.Probe.RelativeContrast, output, Logs.Probe.RelativeContrast, TransparentRed);
            LayerMask(Options.Probe.LowContrastMajority, output, Logs.Probe.LowContrastMajority, TransparentRed);

            if (Options.Probe.Orientation)
            {
                BinaryMap markers = OrientationMarkers.Draw(Logs.Probe.Orientation, Logs.Probe.Blocks, Logs.Probe.SegmentationMask);
                AlphaLayering.Layer(output, ScalarColoring.Mask(markers, ColorF.Transparent, ColorF.Red));
            }

            if (Options.Probe.MinutiaCollector)
                TemplateDrawer.Draw(output, Logs.Probe.MinutiaCollector);

            BinaryMap mask = null;
            if (Options.Probe.Mask == MaskType.Segmentation)
                mask = BlockFiller.FillBlocks(Logs.Probe.SegmentationMask.GetInverted(), Logs.Probe.Blocks);
            if (Options.Probe.Mask == MaskType.Inner)
                mask = Logs.Probe.InnerMask.GetInverted();
            if (mask != null)
                AlphaLayering.Layer(output, ScalarColoring.Mask(mask, ColorF.Transparent, LightFog));

            OutputImage = ImageIO.CreateBitmap(PixelFormat.ToColorB(output));
        }

        float[,] GetLayer(LayerType type, LogCollector.ExtractionData data, LogCollector.SkeletonData skeleton)
        {
            switch (type)
            {
                case LayerType.OriginalImage: return GrayscaleInverter.GetInverted(PixelFormat.ToFloat(data.InputImage));
                case LayerType.Equalized: return data.Equalized;
                case LayerType.SmoothedRidges: return data.SmoothedRidges;
                case LayerType.OrthogonalSmoothing: return data.OrthogonalSmoothing;
                case LayerType.Binarized: return PixelFormat.ToFloat(data.Binarized);
                case LayerType.BinarySmoothing: return PixelFormat.ToFloat(data.BinarySmoothing);
                case LayerType.RemovedCrosses: return PixelFormat.ToFloat(data.RemovedCrosses);
                case LayerType.Thinned: return PixelFormat.ToFloat(skeleton.Thinned);
                case LayerType.RidgeTracer: return PixelFormat.ToFloat(SkeletonDrawer.Draw(skeleton.RidgeTracer, data.Binarized.Size));
                case LayerType.DotRemover: return PixelFormat.ToFloat(SkeletonDrawer.Draw(skeleton.DotRemover, data.Binarized.Size));
                case LayerType.PoreRemover: return PixelFormat.ToFloat(SkeletonDrawer.Draw(skeleton.PoreRemover, data.Binarized.Size));
                case LayerType.TailRemover: return PixelFormat.ToFloat(SkeletonDrawer.Draw(skeleton.TailRemover, data.Binarized.Size));
                case LayerType.FragmentRemover: return PixelFormat.ToFloat(SkeletonDrawer.Draw(skeleton.FragmentRemover, data.Binarized.Size));
                case LayerType.MinutiaMask: return PixelFormat.ToFloat(SkeletonDrawer.Draw(skeleton.MinutiaMask, data.Binarized.Size));
                case LayerType.BranchMinutiaRemover: return PixelFormat.ToFloat(SkeletonDrawer.Draw(skeleton.BranchMinutiaRemover, data.Binarized.Size));
                default: throw new Exception();
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
