using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SourceAFIS.General;
using System.Xml.Linq;

namespace SourceAFIS
{
    public sealed class FingerprintTemplate
    {
        public List<FingerprintMinutia> Minutiae = new List<FingerprintMinutia>();
        internal NeighborEdge[][] EdgeTable;

        public FingerprintTemplate(byte[,] image)
        {
            const int blockSize = 15;

            image = InvertInput(image);
            var blocks = new BlockMap(new Point(image.GetLength(1), image.GetLength(0)), blockSize);

            var histogram = ComputeHistogram(blocks, image);
            var smoothHistogram = ComputeSmoothedHistogram(blocks, histogram);
            BitImage mask = ComputeMask(blocks, histogram);
            double[,] equalized = Equalize(blocks, image, smoothHistogram, mask);

            byte[,] orientation = ComputeOrientationMap(equalized, mask, blocks);
            double[,] smoothed = SmoothByOrientation(equalized, orientation, mask, blocks, 0, ConstructOrientedLines(step: 1.59));
            double[,] orthogonal = SmoothByOrientation(smoothed, orientation, mask, blocks, Angle.PIB,
                ConstructOrientedLines(resolution: 11, radius: 4, step: 1.11));

            BitImage binary = Binarize(smoothed, orthogonal, mask, blocks);
            binary.AndNot(FilterBinarized(binary.GetInverted()));
            binary.Or(FilterBinarized(binary));
            RemoveCrosses(binary);

            BitImage pixelMask = mask.FillBlocks(blocks);
            BitImage innerMask = ComputeInnerMask(pixelMask);

            BitImage inverted = binary.GetInverted();
            inverted.And(pixelMask);

            FingerprintSkeleton ridges = new FingerprintSkeleton(binary);
            FingerprintSkeleton valleys = new FingerprintSkeleton(inverted);

            CollectMinutiae(ridges, FingerprintMinutiaType.Ending);
            CollectMinutiae(valleys, FingerprintMinutiaType.Bifurcation);
            ApplyMask(innerMask);
            RemoveMinutiaClouds();
            LimitTemplateSize();
            ShuffleMinutiae();

            BuildEdgeTable();
        }

        public FingerprintTemplate(XElement xml)
        {
            Minutiae = (from minutia in xml.Elements("Minutia")
                        select new FingerprintMinutia()
                        {
                            Position = new Point((int)minutia.Attribute("X"), (int)minutia.Attribute("Y")),
                            Direction = (byte)(uint)minutia.Attribute("Direction"),
                            Type = (FingerprintMinutiaType)Enum.Parse(typeof(FingerprintMinutiaType), (string)minutia.Attribute("Type"), false)
                        }).ToList();
            BuildEdgeTable();
        }

        public XElement ToXml()
        {
            return new XElement("FingerprintTemplate",
                from minutia in Minutiae
                select new XElement("Minutia",
                    new XAttribute("X", minutia.Position.X),
                    new XAttribute("Y", minutia.Position.Y),
                    new XAttribute("Direction", minutia.Direction),
                    new XAttribute("Type", minutia.Type.ToString())));
        }

        public override string ToString() { return ToXml().ToString(); }

        static byte[,] InvertInput(byte[,] image)
        {
            var result = new byte[image.GetLength(0), image.GetLength(1)];
            for (int y = 0; y < image.GetLength(0); ++y)
                for (int x = 0; x < image.GetLength(1); ++x)
                    result[y, x] = (byte)(255 - image[image.GetLength(0) - y - 1, x]);
            return result;
        }
        
        static int[, ,] ComputeHistogram(BlockMap blocks, byte[,] image)
        {
            var histogram = new int[blocks.BlockCount.Y, blocks.BlockCount.X, 256];
            foreach (var block in blocks.AllBlocks)
            {
                var area = blocks.BlockAreas[block];
                for (int y = area.Bottom; y < area.Top; ++y)
                    for (int x = area.Left; x < area.Right; ++x)
                        ++histogram[block.Y, block.X, image[y, x]];
            }
            return histogram;
        }

        static int[, ,] ComputeSmoothedHistogram(BlockMap blocks, int[, ,] input)
        {
            var blocksAround = new Point[] { new Point(0, 0), new Point(-1, 0), new Point(0, -1), new Point(-1, -1) };
            var output = new int[blocks.CornerCount.Y, blocks.CornerCount.X, 256];
            foreach (var corner in blocks.AllCorners)
            {
                foreach (Point relative in blocksAround)
                {
                    var block = corner + relative;
                    if (blocks.AllBlocks.Contains(block))
                    {
                        for (int i = 0; i < 256; ++i)
                            output[corner.Y, corner.X, i] += input[block.Y, block.X, i];
                    }
                }
            }
            return output;
        }

        static BitImage ComputeMask(BlockMap blocks, int[, ,] histogram)
        {
            byte[,] contrast = ComputeClippedContrast(blocks, histogram);

            BitImage mask = new BitImage(ComputeAbsoluteContrast(contrast));
            mask.Or(ComputeRelativeContrast(contrast, blocks));
            mask.Or(ApplyVotingFilter(mask, radius: 9, majority: 0.86, borderDist: 7));

            mask.Or(FilterBlockErrors(mask));
            mask.Invert();
            mask.Or(FilterBlockErrors(mask));
            mask.Or(FilterBlockErrors(mask));
            mask.Or(ApplyVotingFilter(mask, radius: 7, borderDist: 4));

            return mask;
        }

        static BitImage FilterBlockErrors(BitImage input) { return ApplyVotingFilter(input, majority: 0.7, borderDist: 4); }
        static BitImage FilterBinarized(BitImage input) { return ApplyVotingFilter(input, radius: 2, majority: 0.61, borderDist: 17); }

        static byte[,] ComputeClippedContrast(BlockMap blocks, int[, ,] histogram)
        {
            const double clipFraction = 0.08;

            byte[,] result = new byte[blocks.BlockCount.Y, blocks.BlockCount.X];
            foreach (var block in blocks.AllBlocks)
            {
                int area = 0;
                for (int i = 0; i < 256; ++i)
                    area += histogram[block.Y, block.X, i];
                int clipLimit = Convert.ToInt32(area * clipFraction);

                int accumulator = 0;
                int lowerBound = 255;
                for (int i = 0; i < 256; ++i)
                {
                    accumulator += histogram[block.Y, block.X, i];
                    if (accumulator > clipLimit)
                    {
                        lowerBound = i;
                        break;
                    }
                }

                accumulator = 0;
                int upperBound = 0;
                for (int i = 255; i >= 0; --i)
                {
                    accumulator += histogram[block.Y, block.X, i];
                    if (accumulator > clipLimit)
                    {
                        upperBound = i;
                        break;
                    }
                }

                result[block.Y, block.X] = (byte)(upperBound - lowerBound);
            }
            return result;
        }

        static BitImage ComputeAbsoluteContrast(byte[,] contrast)
        {
            const int limit = 17;
            BitImage result = new BitImage(contrast.GetLength(1), contrast.GetLength(0));
            for (int y = 0; y < result.Height; ++y)
                for (int x = 0; x < result.Width; ++x)
                    if (contrast[y, x] < limit)
                        result.SetBitOne(x, y);
            return result;
        }

        static BitImage ComputeRelativeContrast(byte[,] contrast, BlockMap blocks)
        {
            const int sampleSize = 168568;
            const double sampleFraction = 0.49;
            const double relativeLimit = 0.34;

            List<byte> sortedContrast = new List<byte>();
            foreach (byte contrastItem in contrast)
                sortedContrast.Add(contrastItem);
            sortedContrast.Sort();
            sortedContrast.Reverse();

            int pixelsPerBlock = blocks.PixelCount.Area / blocks.AllBlocks.TotalArea;
            int sampleCount = Math.Min(sortedContrast.Count, sampleSize / pixelsPerBlock);
            int consideredBlocks = Math.Max(Convert.ToInt32(sampleCount * sampleFraction), 1);

            int averageContrast = 0;
            for (int i = 0; i < consideredBlocks; ++i)
                averageContrast += sortedContrast[i];
            averageContrast /= consideredBlocks;
            byte limit = Convert.ToByte(averageContrast * relativeLimit);

            BitImage result = new BitImage(blocks.BlockCount.X, blocks.BlockCount.Y);
            for (int y = 0; y < result.Height; ++y)
                for (int x = 0; x < result.Width; ++x)
                    if (contrast[y, x] < limit)
                        result.SetBitOne(x, y);
            return result;
        }

        static BitImage ApplyVotingFilter(BitImage input, int radius = 1, double majority = 0.51, int borderDist = 0)
        {
            Rectangle rect = new Rectangle(new Point(borderDist, borderDist),
                new Point(input.Width - 2 * borderDist, input.Height - 2 * borderDist));
            BitImage output = new BitImage(input.Size);
            for (int y = rect.RangeY.Begin; y < rect.RangeY.End; ++y)
            {
                for (int x = rect.Left; x < rect.Right; ++x)
                {
                    Rectangle neighborhood = Rectangle.Between(
                        new Point(Math.Max(x - radius, 0), Math.Max(y - radius, 0)),
                        new Point(Math.Min(x + radius + 1, output.Width), Math.Min(y + radius + 1, output.Height)));

                    int ones = 0;
                    for (int ny = neighborhood.Bottom; ny < neighborhood.Top; ++ny)
                        for (int nx = neighborhood.Left; nx < neighborhood.Right; ++nx)
                            if (input.GetBit(nx, ny))
                                ++ones;

                    double voteWeight = 1.0 / neighborhood.TotalArea;
                    if (ones * voteWeight >= majority)
                        output.SetBitOne(x, y);
                }
            }
            return output;
        }

        static double[,] Equalize(BlockMap blocks, byte[,] image, int[, ,] histogram, BitImage blockMask)
        {
            const double maxScaling = 3.99;
            const double minScaling = 0.25;

            const double rangeMin = -1;
            const double rangeMax = 1;
            const double rangeSize = rangeMax - rangeMin;

            const double widthMax = rangeSize / 256 * maxScaling;
            const double widthMin = rangeSize / 256 * minScaling;

            var limitedMin = new double[256];
            var limitedMax = new double[256];
            var toFloatTable = new double[256];
            for (int i = 0; i < 256; ++i)
            {
                limitedMin[i] = Math.Max(i * widthMin + rangeMin, rangeMax - (255 - i) * widthMax);
                limitedMax[i] = Math.Min(i * widthMax + rangeMin, rangeMax - (255 - i) * widthMin);
                toFloatTable[i] = i / 255;
            }

            var cornerMapping = new double[blocks.CornerCount.Y, blocks.CornerCount.X, 256];
            foreach (var corner in blocks.AllCorners)
            {
                if (blockMask.GetBitSafe(corner.X, corner.Y, false)
                    || blockMask.GetBitSafe(corner.X - 1, corner.Y, false)
                    || blockMask.GetBitSafe(corner.X, corner.Y - 1, false)
                    || blockMask.GetBitSafe(corner.X - 1, corner.Y - 1, false))
                {
                    int area = 0;
                    for (int i = 0; i < 256; ++i)
                        area += histogram[corner.Y, corner.X, i];
                    double widthWeigth = rangeSize / area;

                    double top = rangeMin;
                    for (int i = 0; i < 256; ++i)
                    {
                        double width = histogram[corner.Y, corner.X, i] * widthWeigth;
                        double equalized = top + toFloatTable[i] * width;
                        top += width;

                        double limited = equalized;
                        if (limited < limitedMin[i])
                            limited = limitedMin[i];
                        if (limited > limitedMax[i])
                            limited = limitedMax[i];
                        cornerMapping[corner.Y, corner.X, i] = limited;
                    }
                }
            }

            var result = new double[blocks.PixelCount.Y, blocks.PixelCount.X];
            foreach (var block in blocks.AllBlocks)
            {
                if (blockMask.GetBit(block))
                {
                    var area = blocks.BlockAreas[block];
                    for (int y = area.Bottom; y < area.Top; ++y)
                        for (int x = area.Left; x < area.Right; ++x)
                        {
                            byte pixel = image[y, x];

                            double bottomLeft = cornerMapping[block.Y, block.X, pixel];
                            double bottomRight = cornerMapping[block.Y, block.X + 1, pixel];
                            double topLeft = cornerMapping[block.Y + 1, block.X, pixel];
                            double topRight = cornerMapping[block.Y + 1, block.X + 1, pixel];

                            var fraction = area.GetFraction(new Point(x, y));
                            result[y, x] = MathEx.Interpolate(topLeft, topRight, bottomLeft, bottomRight, fraction);
                        }
                }
            }
            return result;
        }

        static byte[,] ComputeOrientationMap(double[,] image, BitImage mask, BlockMap blocks)
        {
            PointF[,] accumulated = ComputePixelwiseOrientation(image, mask, blocks);
            PointF[,] byBlock = AverageBlockOrientations(accumulated, blocks, mask);
            PointF[,] smooth = SmoothOutOrientationMap(byBlock, mask);
            return ConvertOrientationVectorsToAngles(smooth, mask);
        }

        class ConsideredOrientation
        {
            public Point CheckLocation;
            public PointF OrientationVector;
        }

        static PointF[,] ComputePixelwiseOrientation(double[,] input, BitImage mask, BlockMap blocks)
        {
            List<List<ConsideredOrientation>> neighbors = GetTestedOrientations();

            PointF[,] orientation = new PointF[input.GetLength(0), input.GetLength(1)];
            for (int blockY = 0; blockY < mask.Height; ++blockY)
            {
                Range validMaskRange = GetMaskLineRange(mask, blockY);
                if (validMaskRange.Length > 0)
                {
                    Range validXRange = new Range(blocks.BlockAreas[blockY, validMaskRange.Begin].Left,
                        blocks.BlockAreas[blockY, validMaskRange.End - 1].Right);
                    for (int y = blocks.BlockAreas[blockY, 0].Bottom; y < blocks.BlockAreas[blockY, 0].Top; ++y)
                    {
                        foreach (ConsideredOrientation neighbor in neighbors[y % neighbors.Count])
                        {
                            int radius = Math.Max(Math.Abs(neighbor.CheckLocation.X), Math.Abs(neighbor.CheckLocation.Y));
                            if (y - radius >= 0 && y + radius < input.GetLength(0))
                            {
                                Range xRange = new Range(Math.Max(radius, validXRange.Begin),
                                    Math.Min(input.GetLength(1) - radius, validXRange.End));
                                for (int x = xRange.Begin; x < xRange.End; ++x)
                                {
                                    double before = input[y - neighbor.CheckLocation.Y, x - neighbor.CheckLocation.X];
                                    double at = input[y, x];
                                    double after = input[y + neighbor.CheckLocation.Y, x + neighbor.CheckLocation.X];
                                    double strength = at - Math.Max(before, after);
                                    if (strength > 0)
                                        orientation[y, x] = orientation[y, x] + strength * neighbor.OrientationVector;
                                }
                            }
                        }
                    }
                }
            }
            return orientation;
        }

        static List<List<ConsideredOrientation>> GetTestedOrientations()
        {
            const double minHalfDistance = 2;
            const double maxHalfDistance = 6;
            const int orientationListSplit = 50;
            const int orientationsChecked = 20;

            Random random = new Random(0);
            List<List<ConsideredOrientation>> allSplits = new List<List<ConsideredOrientation>>();
            for (int i = 0; i < orientationListSplit; ++i)
            {
                List<ConsideredOrientation> orientations = new List<ConsideredOrientation>();
                for (int j = 0; j < orientationsChecked; ++j)
                {
                    ConsideredOrientation orientation = new ConsideredOrientation();
                    do
                    {
                        double angle = Angle.FromFraction(random.NextDouble() * 0.5);
                        double distance = MathEx.InterpolateExponential(minHalfDistance, maxHalfDistance, random.NextDouble());
                        orientation.CheckLocation = (distance * Angle.ToVector(angle)).Round();
                    } while (orientation.CheckLocation == new Point() || orientation.CheckLocation.Y < 0);
                    orientation.OrientationVector = Angle.ToVector(Angle.Add(Angle.ToOrientation(Angle.Atan(orientation.CheckLocation)), Math.PI));
                    if (!orientations.Any(info => info.CheckLocation == orientation.CheckLocation))
                        orientations.Add(orientation);
                }
                orientations.Sort((left, right) => MathEx.CompareYX(left.CheckLocation, right.CheckLocation));
                allSplits.Add(orientations);
            }
            return allSplits;
        }

        static Range GetMaskLineRange(BitImage mask, int y)
        {
            int first = -1;
            int last = -1;
            for (int x = 0; x < mask.Width; ++x)
                if (mask.GetBit(x, y))
                {
                    last = x;
                    if (first < 0)
                        first = x;
                }
            if (first >= 0)
                return new Range(first, last + 1);
            else
                return new Range();
        }

        static PointF[,] AverageBlockOrientations(PointF[,] orientation, BlockMap blocks, BitImage mask)
        {
            PointF[,] sums = new PointF[blocks.BlockCount.Y, blocks.BlockCount.X];
            foreach (var block in blocks.AllBlocks)
            {
                if (mask.GetBit(block))
                {
                    PointF sum = new PointF();
                    Rectangle area = blocks.BlockAreas[block];
                    for (int y = area.Bottom; y < area.Top; ++y)
                        for (int x = area.Left; x < area.Right; ++x)
                            sum += orientation[y, x];
                    sums[block.Y, block.X] = sum;
                }
            }
            return sums;
        }

        static PointF[,] SmoothOutOrientationMap(PointF[,] orientation, BitImage mask)
        {
            const int radius = 1;
            PointF[,] smoothed = new PointF[mask.Height, mask.Width];
            for (int y = 0; y < mask.Height; ++y)
                for (int x = 0; x < mask.Width; ++x)
                    if (mask.GetBit(x, y))
                    {
                        Rectangle neighbors = Rectangle.Between(
                            new Point(Math.Max(0, x - radius), Math.Max(0, y - radius)),
                            new Point(Math.Min(mask.Width, x + radius + 1), Math.Min(mask.Height, y + radius + 1)));
                        PointF sum = new PointF();
                        for (int ny = neighbors.Bottom; ny < neighbors.Top; ++ny)
                            for (int nx = neighbors.Left; nx < neighbors.Right; ++nx)
                                if (mask.GetBit(nx, ny))
                                    sum += orientation[ny, nx];
                        smoothed[y, x] = sum;
                    }
            return smoothed;
        }

        static byte[,] ConvertOrientationVectorsToAngles(PointF[,] vectors, BitImage mask)
        {
            byte[,] angles = new byte[mask.Height, mask.Width];
            for (int y = 0; y < mask.Height; ++y)
                for (int x = 0; x < mask.Width; ++x)
                    if (mask.GetBit(x, y))
                        angles[y, x] = Angle.ToByte(Angle.Atan(vectors[y, x]));
            return angles;
        }

        Point[][] ConstructOrientedLines(int resolution = 32, int radius = 7, double step = 1.5)
        {
            Point[][] result = new Point[resolution][];
            for (int orientationIndex = 0; orientationIndex < resolution; ++orientationIndex)
            {
                List<Point> line = new List<Point>();
                line.Add(new Point());
                PointF direction = Angle.ToVector(Angle.ByBucketCenter(orientationIndex, 2 * resolution));
                for (double r = radius; r >= 0.5; r /= step)
                {
                    Point point = (r * direction).Round();
                    if (!line.Contains(point))
                    {
                        line.Add(point);
                        line.Add(-point);
                    }
                }
                line.Sort(MathEx.CompareYX);
                result[orientationIndex] = line.ToArray();
            }
            return result;
        }

        static double[,] SmoothByOrientation(double[,] input, byte[,] orientation, BitImage mask, BlockMap blocks, byte angle, Point[][] lines)
        {
            double[,] output = new double[input.GetLength(0), input.GetLength(1)];
            foreach (var block in blocks.AllBlocks)
            {
                if (mask.GetBit(block))
                {
                    Point[] line = lines[Angle.Quantize(Angle.Add(orientation[block.Y, block.X], angle), lines.Length)];
                    foreach (Point linePoint in line)
                    {
                        Rectangle target = blocks.BlockAreas[block];
                        Rectangle source = target.GetShifted(linePoint);
                        source.Clip(new Rectangle(blocks.PixelCount));
                        target = source.GetShifted(-linePoint);
                        for (int y = target.Bottom; y < target.Top; ++y)
                            for (int x = target.Left; x < target.Right; ++x)
                                output[y, x] += input[y + linePoint.Y, x + linePoint.X];
                    }
                    Rectangle blockArea = blocks.BlockAreas[block];
                    for (int y = blockArea.Bottom; y < blockArea.Top; ++y)
                        for (int x = blockArea.Left; x < blockArea.Right; ++x)
                            output[y, x] *= 1 / line.Length;
                }
            }
            return output;
        }

        static BitImage Binarize(double[,] input, double[,] baseline, BitImage mask, BlockMap blocks)
        {
            BitImage binarized = new BitImage(input.GetLength(1), input.GetLength(0));
            for (int blockY = 0; blockY < blocks.AllBlocks.Height; ++blockY)
            {
                for (int blockX = 0; blockX < blocks.AllBlocks.Width; ++blockX)
                {
                    if (mask.GetBit(blockX, blockY))
                    {
                        Rectangle rect = blocks.BlockAreas[blockY, blockX];
                        for (int y = rect.Bottom; y < rect.Top; ++y)
                            for (int x = rect.Left; x < rect.Right; ++x)
                                if (input[y, x] - baseline[y, x] > 0)
                                    binarized.SetBitOne(x, y);
                    }
                }
            }
            return binarized;
        }

        static void RemoveCrosses(BitImage input)
        {
            BitImage sw2ne = new BitImage(input.Size);
            BitImage se2nw = new BitImage(input.Size);
            BitImage positions = new BitImage(input.Size);
            BitImage squares = new BitImage(input.Size);

            while (true)
            {
                sw2ne.Copy(input, new Rectangle(0, 0, input.Width - 1, input.Height - 1), new Point());
                sw2ne.And(input, new Rectangle(1, 1, input.Width - 1, input.Height - 1), new Point());
                sw2ne.AndNot(input, new Rectangle(0, 1, input.Width - 1, input.Height - 1), new Point());
                sw2ne.AndNot(input, new Rectangle(1, 0, input.Width - 1, input.Height - 1), new Point());

                se2nw.Copy(input, new Rectangle(0, 1, input.Width - 1, input.Height - 1), new Point());
                se2nw.And(input, new Rectangle(1, 0, input.Width - 1, input.Height - 1), new Point());
                se2nw.AndNot(input, new Rectangle(0, 0, input.Width - 1, input.Height - 1), new Point());
                se2nw.AndNot(input, new Rectangle(1, 1, input.Width - 1, input.Height - 1), new Point());

                positions.Copy(sw2ne);
                positions.Or(se2nw);
                if (positions.IsEmpty())
                    break;

                squares.Copy(positions);
                squares.Or(positions, new Rectangle(0, 0, positions.Width - 1, positions.Height - 1), new Point(1, 0));
                squares.Or(positions, new Rectangle(0, 0, positions.Width - 1, positions.Height - 1), new Point(0, 1));
                squares.Or(positions, new Rectangle(0, 0, positions.Width - 1, positions.Height - 1), new Point(1, 1));

                input.AndNot(squares);
            }
        }

        static BitImage ComputeInnerMask(BitImage outer)
        {
            const int minBorderDistance = 14;
            BitImage inner = new BitImage(outer.Size);
            inner.Copy(outer, new Rectangle(1, 1, outer.Width - 2, outer.Height - 2), new Point(1, 1));
            BitImage temporary = new BitImage(outer.Size);
            if (minBorderDistance >= 1)
                ShrinkMask(temporary, inner, 1);
            int total = 1;
            for (int step = 1; total + step <= minBorderDistance; step *= 2)
            {
                ShrinkMask(temporary, inner, step);
                total += step;
            }
            if (total < minBorderDistance)
                ShrinkMask(temporary, inner, minBorderDistance - total);
            return inner;
        }

        static void ShrinkMask(BitImage temporary, BitImage inner, int amount)
        {
            temporary.Clear();
            temporary.Copy(inner, new Rectangle(amount, 0, inner.Width - amount, inner.Height), new Point(0, 0));
            temporary.And(inner, new Rectangle(0, 0, inner.Width - amount, inner.Height), new Point(amount, 0));
            temporary.And(inner, new Rectangle(0, amount, inner.Width, inner.Height - amount), new Point(0, 0));
            temporary.And(inner, new Rectangle(0, 0, inner.Width, inner.Height - amount), new Point(0, amount));
            inner.Copy(temporary);
        }

        void CollectMinutiae(FingerprintSkeleton skeleton, FingerprintMinutiaType type)
        {
            foreach (SkeletonMinutia skeletonMinutia in skeleton.Minutiae)
            {
                if (skeletonMinutia.IsConsidered && skeletonMinutia.Ridges.Count == 1)
                {
                    FingerprintMinutia templateMinutia = new FingerprintMinutia();
                    templateMinutia.Type = type;
                    templateMinutia.Position = skeletonMinutia.Position;
                    templateMinutia.Direction = skeletonMinutia.Ridges[0].ComputeDirection();
                    Minutiae.Add(templateMinutia);
                }
            }
        }

        void ApplyMask(BitImage mask)
        {
            const double directedExtension = 10.06;
            Minutiae.RemoveAll(minutia =>
            {
                var arrow = (-directedExtension * Angle.ToVector(minutia.Direction)).Round();
                return !mask.GetBitSafe((Point)minutia.Position + arrow, false);
            });
        }

        void RemoveMinutiaClouds()
        {
            const int radius = 20;
            const int maxNeighbors = 4;
            var radiusSq = MathEx.Sq(radius);
            Minutiae = Minutiae.Except(
                (from minutia in Minutiae
                 where Minutiae.Count(neighbor => (neighbor.Position - minutia.Position).SqLength <= radiusSq) - 1 > maxNeighbors
                 select minutia).ToList()).ToList();
        }

        void LimitTemplateSize()
        {
            const int maxMinutiae = 100;
            const int neighborhoodSize = 5;
            if (Minutiae.Count > maxMinutiae)
            {
                Minutiae =
                    (from minutia in Minutiae
                     let radiusSq = (from neighbor in Minutiae
                                     let distanceSq = (minutia.Position - neighbor.Position).SqLength
                                     orderby distanceSq
                                     select distanceSq).Skip(neighborhoodSize).First()
                     orderby radiusSq descending
                     select minutia).Take(maxMinutiae).ToList();
            }
        }

        void ShuffleMinutiae()
        {
            int seed = 0;
            foreach (var minutia in Minutiae)
                seed += minutia.Direction + minutia.Position.X + minutia.Position.Y + (int)minutia.Type;
            Minutiae = MathEx.Shuffle(Minutiae, new Random(seed)).ToList();
        }

        void BuildEdgeTable()
        {
            const int maxDistance = 490;
            const int maxNeighbors = 9;

            EdgeTable = new NeighborEdge[Minutiae.Count][];
            var edges = new List<NeighborEdge>();
            var allSqDistances = new int[Minutiae.Count];

            for (int reference = 0; reference < EdgeTable.Length; ++reference)
            {
                Point referencePosition = Minutiae[reference].Position;
                int sqMaxDistance = MathEx.Sq(maxDistance);
                if (Minutiae.Count - 1 > maxNeighbors)
                {
                    for (int neighbor = 0; neighbor < Minutiae.Count; ++neighbor)
                        allSqDistances[neighbor] = (referencePosition - Minutiae[neighbor].Position).SqLength;
                    Array.Sort(allSqDistances);
                    sqMaxDistance = allSqDistances[maxNeighbors];
                }
                for (int neighbor = 0; neighbor < Minutiae.Count; ++neighbor)
                {
                    if (neighbor != reference && (referencePosition - Minutiae[neighbor].Position).SqLength <= sqMaxDistance)
                    {
                        NeighborEdge record = new NeighborEdge();
                        record.Edge = new EdgeShape(this, reference, neighbor);
                        record.Neighbor = neighbor;
                        edges.Add(record);
                    }
                }

                edges.Sort(NeighborEdgeComparer.Instance);
                if (edges.Count > maxNeighbors)
                    edges.RemoveRange(maxNeighbors, edges.Count - maxNeighbors);
                EdgeTable[reference] = edges.ToArray();
                edges.Clear();
            }
        }

        class NeighborEdgeComparer : IComparer<NeighborEdge>
        {
            public int Compare(NeighborEdge left, NeighborEdge right)
            {
                int result = MathEx.Compare(left.Edge.Length, right.Edge.Length);
                if (result != 0)
                    return result;
                return MathEx.Compare(left.Neighbor, right.Neighbor);
            }

            public static NeighborEdgeComparer Instance = new NeighborEdgeComparer();
        }
    }
}
