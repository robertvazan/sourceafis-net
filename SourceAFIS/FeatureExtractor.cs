// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using System.Linq;

namespace SourceAFIS
{
	static class FeatureExtractor
	{
		public static MutableTemplate Extract(DoubleMatrix raw, double dpi)
		{
			var template = new MutableTemplate();
			// https://sourceafis.machinezoo.com/transparency/decoded-image
			FingerprintTransparency.Current.Log("decoded-image", raw);
			if (Math.Abs(dpi - 500) > Parameters.DpiTolerance)
				raw = ScaleImage(raw, dpi);
			// https://sourceafis.machinezoo.com/transparency/scaled-image
			FingerprintTransparency.Current.Log("scaled-image", raw);
			template.Size = raw.Size;
			var blocks = new BlockMap(raw.Width, raw.Height, Parameters.BlockSize);
			// https://sourceafis.machinezoo.com/transparency/blocks
			FingerprintTransparency.Current.Log("blocks", blocks);
			var histogram = Histogram(blocks, raw);
			var smoothHistogram = SmoothHistogram(blocks, histogram);
			var mask = Mask(blocks, histogram);
			var equalized = Equalize(blocks, raw, smoothHistogram, mask);
			var orientation = OrientationMap(equalized, mask, blocks);
			var smoothedLines = OrientedLines(Parameters.ParallelSmoothingResolution, Parameters.ParallelSmoothingRadius, Parameters.ParallelSmoothingStep);
			var smoothed = SmoothRidges(equalized, orientation, mask, blocks, 0, smoothedLines);
			// https://sourceafis.machinezoo.com/transparency/parallel-smoothing
			FingerprintTransparency.Current.Log("parallel-smoothing", smoothed);
			var orthogonalLines = OrientedLines(Parameters.OrthogonalSmoothingResolution, Parameters.OrthogonalSmoothingRadius, Parameters.OrthogonalSmoothingStep);
			var orthogonal = SmoothRidges(smoothed, orientation, mask, blocks, Math.PI, orthogonalLines);
			// https://sourceafis.machinezoo.com/transparency/orthogonal-smoothing
			FingerprintTransparency.Current.Log("orthogonal-smoothing", orthogonal);
			var binary = Binarize(smoothed, orthogonal, mask, blocks);
			var pixelMask = FillBlocks(mask, blocks);
			CleanupBinarized(binary, pixelMask);
			// https://sourceafis.machinezoo.com/transparency/pixel-mask
			FingerprintTransparency.Current.Log("pixel-mask", pixelMask);
			var inverted = Invert(binary, pixelMask);
			var innerMask = InnerMask(pixelMask);
			var ridges = new Skeleton(binary, SkeletonType.Ridges);
			var valleys = new Skeleton(inverted, SkeletonType.Valleys);
			template.Minutiae = new List<MutableMinutia>();
			CollectMinutiae(template.Minutiae, ridges, MinutiaType.Ending);
			CollectMinutiae(template.Minutiae, valleys, MinutiaType.Bifurcation);
			// https://sourceafis.machinezoo.com/transparency/skeleton-minutiae
			FingerprintTransparency.Current.Log("skeleton-minutiae", template);
			MaskMinutiae(template.Minutiae, innerMask);
			// https://sourceafis.machinezoo.com/transparency/inner-minutiae
			FingerprintTransparency.Current.Log("inner-minutiae", template);
			// Workaround for a bug in CBOR serializer that throws exceptions when it encounters empty array.
			if (template.Minutiae.Count == 0)
				template.Minutiae.Add(new MutableMinutia(new IntPoint(raw.Width / 2, raw.Height / 2), 0, MinutiaType.Ending));
			RemoveMinutiaClouds(template.Minutiae);
			// https://sourceafis.machinezoo.com/transparency/removed-minutia-clouds
			FingerprintTransparency.Current.Log("removed-minutia-clouds", template);
			template.Minutiae = LimitTemplateSize(template.Minutiae);
			// https://sourceafis.machinezoo.com/transparency/top-minutiae
			FingerprintTransparency.Current.Log("top-minutiae", template);
			return template;
		}
		static DoubleMatrix ScaleImage(DoubleMatrix input, double dpi)
		{
			return ScaleImage(input, Doubles.RoundToInt(500.0 / dpi * input.Width), Doubles.RoundToInt(500.0 / dpi * input.Height));
		}
		static DoubleMatrix ScaleImage(DoubleMatrix input, int newWidth, int newHeight)
		{
			var output = new DoubleMatrix(newWidth, newHeight);
			double scaleX = newWidth / (double)input.Width;
			double scaleY = newHeight / (double)input.Height;
			double descaleX = 1 / scaleX;
			double descaleY = 1 / scaleY;
			for (int y = 0; y < newHeight; ++y)
			{
				double y1 = y * descaleY;
				double y2 = y1 + descaleY;
				int y1i = (int)y1;
				int y2i = Math.Min((int)Math.Ceiling(y2), input.Height);
				for (int x = 0; x < newWidth; ++x)
				{
					double x1 = x * descaleX;
					double x2 = x1 + descaleX;
					int x1i = (int)x1;
					int x2i = Math.Min((int)Math.Ceiling(x2), input.Width);
					double sum = 0;
					for (int oy = y1i; oy < y2i; ++oy)
					{
						var ry = Math.Min(oy + 1, y2) - Math.Max(oy, y1);
						for (int ox = x1i; ox < x2i; ++ox)
						{
							var rx = Math.Min(ox + 1, x2) - Math.Max(ox, x1);
							sum += rx * ry * input[ox, oy];
						}
					}
					output[x, y] = sum * (scaleX * scaleY);
				}
			}
			return output;
		}
		static HistogramCube Histogram(BlockMap blocks, DoubleMatrix image)
		{
			var histogram = new HistogramCube(blocks.Primary.Blocks, Parameters.HistogramDepth);
			foreach (var block in blocks.Primary.Blocks.Iterate())
			{
				var area = blocks.Primary.Block(block);
				for (int y = area.Top; y < area.Bottom; ++y)
				{
					for (int x = area.Left; x < area.Right; ++x)
					{
						int depth = (int)(image[x, y] * histogram.Bins);
						histogram.Increment(block, histogram.Constrain(depth));
					}
				}
			}
			// https://sourceafis.machinezoo.com/transparency/histogram
			FingerprintTransparency.Current.Log("histogram", histogram);
			return histogram;
		}
		static HistogramCube SmoothHistogram(BlockMap blocks, HistogramCube input)
		{
			var blocksAround = new IntPoint[] { new IntPoint(0, 0), new IntPoint(-1, 0), new IntPoint(0, -1), new IntPoint(-1, -1) };
			var output = new HistogramCube(blocks.Secondary.Blocks, input.Bins);
			foreach (var corner in blocks.Secondary.Blocks.Iterate())
			{
				foreach (var relative in blocksAround)
				{
					var block = corner + relative;
					if (blocks.Primary.Blocks.Contains(block))
					{
						for (int i = 0; i < input.Bins; ++i)
							output.Add(corner, i, input[block, i]);
					}
				}
			}
			// https://sourceafis.machinezoo.com/transparency/smoothed-histogram
			FingerprintTransparency.Current.Log("smoothed-histogram", output);
			return output;
		}
		static BooleanMatrix Mask(BlockMap blocks, HistogramCube histogram)
		{
			var contrast = ClipContrast(blocks, histogram);
			var mask = FilterAbsoluteContrast(contrast);
			mask.Merge(FilterRelativeContrast(contrast, blocks));
			// https://sourceafis.machinezoo.com/transparency/combined-mask
			FingerprintTransparency.Current.Log("combined-mask", mask);
			mask.Merge(FilterBlockErrors(mask));
			mask.Merge(FilterBlockErrors(mask));
			mask.Invert();
			mask.Merge(FilterBlockErrors(mask));
			mask.Merge(FilterBlockErrors(mask));
			mask.Merge(Vote(mask, null, Parameters.MaskVoteRadius, Parameters.MaskVoteMajority, Parameters.MaskVoteBorderDistance));
			// https://sourceafis.machinezoo.com/transparency/filtered-mask
			FingerprintTransparency.Current.Log("filtered-mask", mask);
			return mask;
		}
		static DoubleMatrix ClipContrast(BlockMap blocks, HistogramCube histogram)
		{
			var result = new DoubleMatrix(blocks.Primary.Blocks);
			foreach (var block in blocks.Primary.Blocks.Iterate())
			{
				int volume = histogram.Sum(block);
				int clipLimit = Doubles.RoundToInt(volume * Parameters.ClippedContrast);
				int accumulator = 0;
				int lowerBound = histogram.Bins - 1;
				for (int i = 0; i < histogram.Bins; ++i)
				{
					accumulator += histogram[block, i];
					if (accumulator > clipLimit)
					{
						lowerBound = i;
						break;
					}
				}
				accumulator = 0;
				int upperBound = 0;
				for (int i = histogram.Bins - 1; i >= 0; --i)
				{
					accumulator += histogram[block, i];
					if (accumulator > clipLimit)
					{
						upperBound = i;
						break;
					}
				}
				result[block] = (upperBound - lowerBound) * (1.0 / (histogram.Bins - 1));
			}
			// https://sourceafis.machinezoo.com/transparency/contrast
			FingerprintTransparency.Current.Log("contrast", result);
			return result;
		}
		static BooleanMatrix FilterAbsoluteContrast(DoubleMatrix contrast)
		{
			var result = new BooleanMatrix(contrast.Size);
			foreach (var block in contrast.Size.Iterate())
				if (contrast[block] < Parameters.MinAbsoluteContrast)
					result[block] = true;
			// https://sourceafis.machinezoo.com/transparency/absolute-contrast-mask
			FingerprintTransparency.Current.Log("absolute-contrast-mask", result);
			return result;
		}
		static BooleanMatrix FilterRelativeContrast(DoubleMatrix contrast, BlockMap blocks)
		{
			var sortedContrast = new List<double>();
			foreach (var block in contrast.Size.Iterate())
				sortedContrast.Add(contrast[block]);
			sortedContrast.Sort();
			sortedContrast.Reverse();
			int pixelsPerBlock = blocks.Pixels.Area / blocks.Primary.Blocks.Area;
			int sampleCount = Math.Min(sortedContrast.Count, Parameters.RelativeContrastSample / pixelsPerBlock);
			int consideredBlocks = Math.Max(Doubles.RoundToInt(sampleCount * Parameters.RelativeContrastPercentile), 1);
			double averageContrast = 0;
			for (int i = 0; i < consideredBlocks; ++i)
				averageContrast += sortedContrast[i];
			averageContrast /= consideredBlocks;
			var limit = averageContrast * Parameters.MinRelativeContrast;
			var result = new BooleanMatrix(blocks.Primary.Blocks);
			foreach (var block in blocks.Primary.Blocks.Iterate())
				if (contrast[block] < limit)
					result[block] = true;
			// https://sourceafis.machinezoo.com/transparency/relative-contrast-mask
			FingerprintTransparency.Current.Log("relative-contrast-mask", result);
			return result;
		}
		static BooleanMatrix Vote(BooleanMatrix input, BooleanMatrix mask, int radius, double majority, int borderDistance) {
			var size = input.Size;
			var rect = new IntRect(borderDistance, borderDistance, size.X - 2 * borderDistance, size.Y - 2 * borderDistance);
			int[] thresholds = new int[Integers.Sq(2 * radius + 1) + 1];
			for (int i = 0; i < thresholds.Length; ++i)
				thresholds[i] = (int)Math.Ceiling(majority * i);
			var counts = new IntMatrix(size);
			var output = new BooleanMatrix(size);
			for (int y = rect.Top; y < rect.Bottom; ++y)
			{
				int superTop = y - radius - 1;
				int superBottom = y + radius;
				int yMin = Math.Max(0, y - radius);
				int yMax = Math.Min(size.Y - 1, y + radius);
				int yRange = yMax - yMin + 1;
				for (int x = rect.Left; x < rect.Right; ++x)
					if (mask == null || mask[x, y])
					{
						int left = x > 0 ? counts[x - 1, y] : 0;
						int top = y > 0 ? counts[x, y - 1] : 0;
						int diagonal = x > 0 && y > 0 ? counts[x - 1, y - 1] : 0;
						int xMin = Math.Max(0, x - radius);
						int xMax = Math.Min(size.X - 1, x + radius);
						int ones;
						if (left > 0 && top > 0 && diagonal > 0)
						{
							ones = top + left - diagonal - 1;
							int superLeft = x - radius - 1;
							int superRight = x + radius;
							if (superLeft >= 0 && superTop >= 0 && input[superLeft, superTop])
								++ones;
							if (superLeft >= 0 && superBottom < size.Y && input[superLeft, superBottom])
								--ones;
							if (superRight < size.X && superTop >= 0 && input[superRight, superTop])
								--ones;
							if (superRight < size.X && superBottom < size.Y && input[superRight, superBottom])
								++ones;
						}
						else
						{
							ones = 0;
							for (int ny = yMin; ny <= yMax; ++ny)
								for (int nx = xMin; nx <= xMax; ++nx)
									if (input[nx, ny])
										++ones;
						}
						counts[x, y] = ones + 1;
						if (ones >= thresholds[yRange * (xMax - xMin + 1)])
							output[x, y] = true;
					}
			}
			return output;
		}
		static BooleanMatrix FilterBlockErrors(BooleanMatrix input)
		{
			return Vote(input, null, Parameters.BlockErrorsVoteRadius, Parameters.BlockErrorsVoteMajority, Parameters.BlockErrorsVoteBorderDistance);
		}
		static DoubleMatrix Equalize(BlockMap blocks, DoubleMatrix image, HistogramCube histogram, BooleanMatrix blockMask)
		{
			const double rangeMin = -1;
			const double rangeMax = 1;
			const double rangeSize = rangeMax - rangeMin;
			double widthMax = rangeSize / histogram.Bins * Parameters.MaxEqualizationScaling;
			double widthMin = rangeSize / histogram.Bins * Parameters.MinEqualizationScaling;
			var limitedMin = new double[histogram.Bins];
			var limitedMax = new double[histogram.Bins];
			var dequantized = new double[histogram.Bins];
			for (int i = 0; i < histogram.Bins; ++i)
			{
				limitedMin[i] = Math.Max(i * widthMin + rangeMin, rangeMax - (histogram.Bins - 1 - i) * widthMax);
				limitedMax[i] = Math.Min(i * widthMax + rangeMin, rangeMax - (histogram.Bins - 1 - i) * widthMin);
				dequantized[i] = i / (double)(histogram.Bins - 1);
			}
			var mappings = new Dictionary<IntPoint, double[]>();
			foreach (var corner in blocks.Secondary.Blocks.Iterate())
			{
				double[] mapping = new double[histogram.Bins];
				mappings[corner] = mapping;
				if (blockMask.Get(corner, false)
					|| blockMask.Get(corner.X - 1, corner.Y, false)
					|| blockMask.Get(corner.X, corner.Y - 1, false)
					|| blockMask.Get(corner.X - 1, corner.Y - 1, false))
				{
					double step = rangeSize / histogram.Sum(corner);
					double top = rangeMin;
					for (int i = 0; i < histogram.Bins; ++i)
					{
						double band = histogram[corner, i] * step;
						double equalized = top + dequantized[i] * band;
						top += band;
						if (equalized < limitedMin[i])
							equalized = limitedMin[i];
						if (equalized > limitedMax[i])
							equalized = limitedMax[i];
						mapping[i] = equalized;
					}
				}
			}
			var result = new DoubleMatrix(blocks.Pixels);
			foreach (var block in blocks.Primary.Blocks.Iterate())
			{
				var area = blocks.Primary.Block(block);
				if (blockMask[block])
				{
					var topleft = mappings[block];
					var topright = mappings[new IntPoint(block.X + 1, block.Y)];
					var bottomleft = mappings[new IntPoint(block.X, block.Y + 1)];
					var bottomright = mappings[new IntPoint(block.X + 1, block.Y + 1)];
					for (int y = area.Top; y < area.Bottom; ++y)
						for (int x = area.Left; x < area.Right; ++x)
						{
							int depth = histogram.Constrain((int)(image[x, y] * histogram.Bins));
							double rx = (x - area.X + 0.5) / area.Width;
							double ry = (y - area.Y + 0.5) / area.Height;
							result[x, y] = Doubles.Interpolate(bottomleft[depth], bottomright[depth], topleft[depth], topright[depth], rx, ry);
						}
				}
				else
				{
					for (int y = area.Top; y < area.Bottom; ++y)
						for (int x = area.Left; x < area.Right; ++x)
							result[x, y] = -1;
				}
			}
			// https://sourceafis.machinezoo.com/transparency/equalized-image
			FingerprintTransparency.Current.Log("equalized-image", result);
			return result;
		}
		static DoubleMatrix OrientationMap(DoubleMatrix image, BooleanMatrix mask, BlockMap blocks)
		{
			var accumulated = PixelwiseOrientation(image, mask, blocks);
			var byBlock = BlockOrientations(accumulated, blocks, mask);
			var smooth = SmoothOrientation(byBlock, mask);
			return OrientationAngles(smooth, mask);
		}
		class ConsideredOrientation
		{
			public IntPoint Offset;
			public DoublePoint Orientation;
		}
		class OrientationRandom {
			const int Prime = 1610612741;
			const int Bits = 30;
			const int Mask = (1 << Bits) - 1;
			const double Scaling = 1.0 / (1 << Bits);
			long state = unchecked(Prime * Prime * Prime);
			public double Next() {
				state *= Prime;
				return ((state & Mask) + 0.5) * Scaling;
			}
		}
		static ConsideredOrientation[][] PlanOrientations()
		{
			var random = new OrientationRandom();
			var splits = new ConsideredOrientation[Parameters.OrientationSplit][];
			for (int i = 0; i < Parameters.OrientationSplit; ++i)
			{
				var orientations = splits[i] = new ConsideredOrientation[Parameters.OrientationsChecked];
				for (int j = 0; j < Parameters.OrientationsChecked; ++j)
				{
					var sample = orientations[j] = new ConsideredOrientation();
					while (true)
					{
						double angle = random.Next() * Math.PI;
						double distance = Doubles.InterpolateExponential(Parameters.MinOrientationRadius, Parameters.MaxOrientationRadius, random.Next());
						sample.Offset = (distance * DoubleAngle.ToVector(angle)).Round();
						if (sample.Offset == IntPoint.Zero)
							continue;
						if (sample.Offset.Y < 0)
							continue;
						bool duplicate = false;
						for (int jj = 0; jj < j; ++jj)
							if (orientations[jj].Offset == sample.Offset)
								duplicate = true;
						if (duplicate)
							continue;
						break;
					}
					sample.Orientation = DoubleAngle.ToVector(DoubleAngle.Add(DoubleAngle.ToOrientation(DoubleAngle.Atan((DoublePoint)sample.Offset)), Math.PI));
				}
			}
			return splits;
		}
		static DoublePointMatrix PixelwiseOrientation(DoubleMatrix input, BooleanMatrix mask, BlockMap blocks)
		{
			var neighbors = PlanOrientations();
			var orientation = new DoublePointMatrix(input.Size);
			for (int blockY = 0; blockY < blocks.Primary.Blocks.Y; ++blockY)
			{
				var maskRange = MaskRange(mask, blockY);
				if (maskRange.Length > 0)
				{
					var validXRange = new IntRange(
						blocks.Primary.Block(maskRange.Start, blockY).Left,
						blocks.Primary.Block(maskRange.End - 1, blockY).Right);
					for (int y = blocks.Primary.Block(0, blockY).Top; y < blocks.Primary.Block(0, blockY).Bottom; ++y)
					{
						foreach (var neighbor in neighbors[y % neighbors.Length])
						{
							int radius = Math.Max(Math.Abs(neighbor.Offset.X), Math.Abs(neighbor.Offset.Y));
							if (y - radius >= 0 && y + radius < input.Height)
							{
								var xRange = new IntRange(Math.Max(radius, validXRange.Start), Math.Min(input.Width - radius, validXRange.End));
								for (int x = xRange.Start; x < xRange.End; ++x)
								{
									double before = input[x - neighbor.Offset.X, y - neighbor.Offset.Y];
									double at = input[x, y];
									double after = input[x + neighbor.Offset.X, y + neighbor.Offset.Y];
									double strength = at - Math.Max(before, after);
									if (strength > 0)
										orientation.Add(x, y, strength * neighbor.Orientation);
								}
							}
						}
					}
				}
			}
			// https://sourceafis.machinezoo.com/transparency/pixelwise-orientation
			FingerprintTransparency.Current.Log("pixelwise-orientation", orientation);
			return orientation;
		}
		static IntRange MaskRange(BooleanMatrix mask, int y)
		{
			int first = -1;
			int last = -1;
			for (int x = 0; x < mask.Width; ++x)
				if (mask[x, y])
				{
					last = x;
					if (first < 0)
						first = x;
				}
			if (first >= 0)
				return new IntRange(first, last + 1);
			else
				return IntRange.Zero;
		}
		static DoublePointMatrix BlockOrientations(DoublePointMatrix orientation, BlockMap blocks, BooleanMatrix mask)
		{
			var sums = new DoublePointMatrix(blocks.Primary.Blocks);
			foreach (var block in blocks.Primary.Blocks.Iterate())
			{
				if (mask[block])
				{
					var area = blocks.Primary.Block(block);
					for (int y = area.Top; y < area.Bottom; ++y)
						for (int x = area.Left; x < area.Right; ++x)
							sums.Add(block, orientation[x, y]);
				}
			}
			// https://sourceafis.machinezoo.com/transparency/block-orientation
			FingerprintTransparency.Current.Log("block-orientation", sums);
			return sums;
		}
		static DoublePointMatrix SmoothOrientation(DoublePointMatrix orientation, BooleanMatrix mask)
		{
			var size = mask.Size;
			var smoothed = new DoublePointMatrix(size);
			foreach (var block in size.Iterate())
				if (mask[block])
				{
					var neighbors = IntRect.Around(block, Parameters.OrientationSmoothingRadius).Intersect(new IntRect(size));
					for (int ny = neighbors.Top; ny < neighbors.Bottom; ++ny)
						for (int nx = neighbors.Left; nx < neighbors.Right; ++nx)
							if (mask[nx, ny])
								smoothed.Add(block, orientation[nx, ny]);
				}
			// https://sourceafis.machinezoo.com/transparency/smoothed-orientation
			FingerprintTransparency.Current.Log("smoothed-orientation", smoothed);
			return smoothed;
		}
		static DoubleMatrix OrientationAngles(DoublePointMatrix vectors, BooleanMatrix mask)
		{
			var size = mask.Size;
			var angles = new DoubleMatrix(size);
			foreach (var block in size.Iterate())
				if (mask[block])
					angles[block] = DoubleAngle.Atan(vectors[block]);
			return angles;
		}
		static IntPoint[][] OrientedLines(int resolution, int radius, double step)
		{
			var result = new IntPoint[resolution][];
			for (int orientationIndex = 0; orientationIndex < resolution; ++orientationIndex)
			{
				var line = new List<IntPoint>();
				line.Add(IntPoint.Zero);
				var direction = DoubleAngle.ToVector(DoubleAngle.FromOrientation(DoubleAngle.BucketCenter(orientationIndex, resolution)));
				for (double r = radius; r >= 0.5; r /= step)
				{
					var sample = (r * direction).Round();
					if (!line.Contains(sample))
					{
						line.Add(sample);
						line.Add(-sample);
					}
				}
				result[orientationIndex] = line.ToArray();
			}
			return result;
		}
		static DoubleMatrix SmoothRidges(DoubleMatrix input, DoubleMatrix orientation, BooleanMatrix mask, BlockMap blocks, double angle, IntPoint[][] lines)
		{
			var output = new DoubleMatrix(input.Size);
			foreach (var block in blocks.Primary.Blocks.Iterate())
			{
				if (mask[block])
				{
					var line = lines[DoubleAngle.Quantize(DoubleAngle.Add(orientation[block], angle), lines.Length)];
					foreach (var linePoint in line)
					{
						var target = blocks.Primary.Block(block);
						var source = target.Move(linePoint).Intersect(new IntRect(blocks.Pixels));
						target = source.Move(-linePoint);
						for (int y = target.Top; y < target.Bottom; ++y)
							for (int x = target.Left; x < target.Right; ++x)
								output.Add(x, y, input[x + linePoint.X, y + linePoint.Y]);
					}
					var blockArea = blocks.Primary.Block(block);
					for (int y = blockArea.Top; y < blockArea.Bottom; ++y)
						for (int x = blockArea.Left; x < blockArea.Right; ++x)
							output.Multiply(x, y, 1.0 / line.Length);
				}
			}
			return output;
		}
		static BooleanMatrix Binarize(DoubleMatrix input, DoubleMatrix baseline, BooleanMatrix mask, BlockMap blocks)
		{
			var size = input.Size;
			var binarized = new BooleanMatrix(size);
			foreach (var block in blocks.Primary.Blocks.Iterate())
			{
				if (mask[block])
				{
					var rect = blocks.Primary.Block(block);
					for (int y = rect.Top; y < rect.Bottom; ++y)
						for (int x = rect.Left; x < rect.Right; ++x)
							if (input[x, y] - baseline[x, y] > 0)
								binarized[x, y] = true;
				}
			}
			// https://sourceafis.machinezoo.com/transparency/binarized-image
			FingerprintTransparency.Current.Log("binarized-image", binarized);
			return binarized;
		}
		static void CleanupBinarized(BooleanMatrix binary, BooleanMatrix mask)
		{
			var size = binary.Size;
			var inverted = new BooleanMatrix(binary);
			inverted.Invert();
			var islands = Vote(inverted, mask, Parameters.BinarizedVoteRadius, Parameters.BinarizedVoteMajority, Parameters.BinarizedVoteBorderDistance);
			var holes = Vote(binary, mask, Parameters.BinarizedVoteRadius, Parameters.BinarizedVoteMajority, Parameters.BinarizedVoteBorderDistance);
			for (int y = 0; y < size.Y; ++y)
				for (int x = 0; x < size.X; ++x)
					binary[x, y] = binary[x, y] && !islands[x, y] || holes[x, y];
			RemoveCrosses(binary);
			// https://sourceafis.machinezoo.com/transparency/filtered-binary-image
			FingerprintTransparency.Current.Log("filtered-binary-image", binary);
		}
		static void RemoveCrosses(BooleanMatrix input)
		{
			var size = input.Size;
			bool any = true;
			while (any)
			{
				any = false;
				for (int y = 0; y < size.Y - 1; ++y)
					for (int x = 0; x < size.X - 1; ++x)
						if (input[x, y] && input[x + 1, y + 1] && !input[x, y + 1] && !input[x + 1, y] || input[x, y + 1] && input[x + 1, y] && !input[x, y] && !input[x + 1, y + 1])
						{
							input[x, y] = false;
							input[x, y + 1] = false;
							input[x + 1, y] = false;
							input[x + 1, y + 1] = false;
							any = true;
						}
			}
		}
		static BooleanMatrix FillBlocks(BooleanMatrix mask, BlockMap blocks)
		{
			var pixelized = new BooleanMatrix(blocks.Pixels);
			foreach (var block in blocks.Primary.Blocks.Iterate())
				if (mask[block])
					foreach (var pixel in blocks.Primary.Block(block).Iterate())
						pixelized[pixel] = true;
			return pixelized;
		}
		static BooleanMatrix Invert(BooleanMatrix binary, BooleanMatrix mask) {
			var size = binary.Size;
			var inverted = new BooleanMatrix(size);
			for (int y = 0; y < size.Y; ++y)
				for (int x = 0; x < size.X; ++x)
					inverted[x, y] = !binary[x, y] && mask[x, y];
			return inverted;
		}
		static BooleanMatrix InnerMask(BooleanMatrix outer)
		{
			var size = outer.Size;
			var inner = new BooleanMatrix(size);
			for (int y = 1; y < size.Y - 1; ++y)
				for (int x = 1; x < size.X - 1; ++x)
					inner[x, y] = outer[x, y];
			if (Parameters.InnerMaskBorderDistance >= 1)
				inner = ShrinkMask(inner, 1);
			int total = 1;
			for (int step = 1; total + step <= Parameters.InnerMaskBorderDistance; step *= 2)
			{
				inner = ShrinkMask(inner, step);
				total += step;
			}
			if (total < Parameters.InnerMaskBorderDistance)
				inner = ShrinkMask(inner, Parameters.InnerMaskBorderDistance - total);
			// https://sourceafis.machinezoo.com/transparency/inner-mask
			FingerprintTransparency.Current.Log("inner-mask", inner);
			return inner;
		}
		static BooleanMatrix ShrinkMask(BooleanMatrix mask, int amount)
		{
			var size = mask.Size;
			var shrunk = new BooleanMatrix(size);
			for (int y = amount; y < size.Y - amount; ++y)
				for (int x = amount; x < size.X - amount; ++x)
					shrunk[x, y] = mask[x, y - amount] && mask[x, y + amount] && mask[x - amount, y] && mask[x + amount, y];
			return shrunk;
		}
		static void CollectMinutiae(List<MutableMinutia> minutiae, Skeleton skeleton, MinutiaType type)
		{
			foreach (var sminutia in skeleton.Minutiae)
				if (sminutia.Ridges.Count == 1)
					minutiae.Add(new MutableMinutia(sminutia.Position, sminutia.Ridges[0].Direction(), type));
		}
		static void MaskMinutiae(List<MutableMinutia> minutiae, BooleanMatrix mask)
		{
			minutiae.RemoveAll(minutia =>
			{
				var arrow = (-Parameters.MaskDisplacement * DoubleAngle.ToVector(minutia.Direction)).Round();
				return !mask.Get(minutia.Position + arrow, false);
			});
		}
		static void RemoveMinutiaClouds(List<MutableMinutia> minutiae)
		{
			var radiusSq = Integers.Sq(Parameters.MinutiaCloudRadius);
			var removed = new HashSet<MutableMinutia>(minutiae.Where(minutia => Parameters.MaxCloudSize < minutiae.Where(neighbor => (neighbor.Position - minutia.Position).LengthSq <= radiusSq).Count() - 1));
			minutiae.RemoveAll(minutia => removed.Contains(minutia));
		}
		static List<MutableMinutia> LimitTemplateSize(List<MutableMinutia> minutiae)
		{
			if (minutiae.Count <= Parameters.MaxMinutiae)
				return minutiae;
			return
				(from minutia in minutiae
				 let radiusSq = (from neighbor in minutiae
								 let distanceSq = (minutia.Position - neighbor.Position).LengthSq
								 orderby distanceSq
								 select distanceSq).Skip(Parameters.SortByNeighbor).First()
				 orderby radiusSq descending
				 select minutia).Take(Parameters.MaxMinutiae).ToList();
		}
	}
}
