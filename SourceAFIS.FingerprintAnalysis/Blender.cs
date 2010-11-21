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
                BlendImage
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
                default: return new float[data.Binarized.Height, data.Binarized.Width];
            }
        }
    }
}
