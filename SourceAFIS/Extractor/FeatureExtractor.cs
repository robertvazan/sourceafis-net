// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Configuration;
using SourceAFIS.Extractor.Minutiae;
using SourceAFIS.Extractor.Skeletons;
using SourceAFIS.Features;
using SourceAFIS.Primitives;
using SourceAFIS.Templates;

namespace SourceAFIS.Extractor
{
    static class FeatureExtractor
    {
        public static MutableTemplate Extract(DoubleMatrix raw, double dpi)
        {
            var template = new MutableTemplate();
            // https://sourceafis.machinezoo.com/transparency/decoded-image
            FingerprintTransparency.Current.Log("decoded-image", raw);
            raw = ImageResizer.Resize(raw, dpi);
            // https://sourceafis.machinezoo.com/transparency/scaled-image
            FingerprintTransparency.Current.Log("scaled-image", raw);
            template.Size = raw.Size;
            var blocks = new BlockMap(raw.Width, raw.Height, Parameters.BlockSize);
            // https://sourceafis.machinezoo.com/transparency/blocks
            FingerprintTransparency.Current.Log("blocks", blocks);
            var histogram = LocalHistograms.Create(blocks, raw);
            var smoothHistogram = LocalHistograms.Smooth(blocks, histogram);
            var mask = SegmentationMask.Compute(blocks, histogram);
            var equalized = ImageEqualization.Equalize(blocks, raw, smoothHistogram, mask);
            var orientation = BlockOrientations.Compute(equalized, mask, blocks);
            var smoothed = OrientedSmoothing.Parallel(equalized, orientation, mask, blocks);
            var orthogonal = OrientedSmoothing.Orthogonal(smoothed, orientation, mask, blocks);
            var binary = BinarizedImage.Binarize(smoothed, orthogonal, mask, blocks);
            var pixelMask = SegmentationMask.Pixelwise(mask, blocks);
            BinarizedImage.Cleanup(binary, pixelMask);
            // https://sourceafis.machinezoo.com/transparency/pixel-mask
            FingerprintTransparency.Current.Log("pixel-mask", pixelMask);
            var inverted = BinarizedImage.Invert(binary, pixelMask);
            var innerMask = SegmentationMask.Inner(pixelMask);
            var ridges = SkeletonGraphs.Create(binary, SkeletonType.Ridges);
            var valleys = SkeletonGraphs.Create(inverted, SkeletonType.Valleys);
            template.Minutiae = MinutiaCollector.Collect(ridges, valleys);
            // https://sourceafis.machinezoo.com/transparency/skeleton-minutiae
            FingerprintTransparency.Current.Log("skeleton-minutiae", template);
            InnerMinutiaeFilter.Apply(template.Minutiae, innerMask);
            // https://sourceafis.machinezoo.com/transparency/inner-minutiae
            FingerprintTransparency.Current.Log("inner-minutiae", template);
            MinutiaCloudFilter.Apply(template.Minutiae);
            // https://sourceafis.machinezoo.com/transparency/removed-minutia-clouds
            FingerprintTransparency.Current.Log("removed-minutia-clouds", template);
            template.Minutiae = TopMinutiaeFilter.Apply(template.Minutiae);
            // https://sourceafis.machinezoo.com/transparency/top-minutiae
            FingerprintTransparency.Current.Log("top-minutiae", template);
            return template;
        }
    }
}
