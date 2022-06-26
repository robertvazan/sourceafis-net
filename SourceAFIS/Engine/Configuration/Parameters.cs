// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Configuration
{
    static class Parameters
    {
        public const int BlockSize = 15;
        public const int HistogramDepth = 256;
        public const double ClippedContrast = 0.08;
        public const double MinAbsoluteContrast = 17 / 255.0;
        public const double MinRelativeContrast = 0.34;
        public const int RelativeContrastSample = 168568;
        public const double RelativeContrastPercentile = 0.49;
        public const int MaskVoteRadius = 7;
        public const double MaskVoteMajority = 0.51;
        public const int MaskVoteBorderDistance = 4;
        public const int BlockErrorsVoteRadius = 1;
        public const double BlockErrorsVoteMajority = 0.7;
        public const int BlockErrorsVoteBorderDistance = 4;
        public const double MaxEqualizationScaling = 3.99;
        public const double MinEqualizationScaling = 0.25;
        public const double MinOrientationRadius = 2;
        public const double MaxOrientationRadius = 6;
        public const int OrientationSplit = 50;
        public const int OrientationsChecked = 20;
        public const int OrientationSmoothingRadius = 1;
        public const int ParallelSmoothingResolution = 32;
        public const int ParallelSmoothingRadius = 7;
        public const double ParallelSmoothingStep = 1.59;
        public const int OrthogonalSmoothingResolution = 11;
        public const int OrthogonalSmoothingRadius = 4;
        public const double OrthogonalSmoothingStep = 1.11;
        public const int BinarizedVoteRadius = 2;
        public const double BinarizedVoteMajority = 0.61;
        public const int BinarizedVoteBorderDistance = 17;
        public const int InnerMaskBorderDistance = 14;
        public const double MaskDisplacement = 10.06;
        public const int MinutiaCloudRadius = 20;
        public const int MaxCloudSize = 4;
        public const int MaxMinutiae = 100;
        public const int SortByNeighbor = 5;
        public const int EdgeTableNeighbors = 9;
        public const int ThinningIterations = 26;
        public const int MaxPoreArm = 41;
        public const int ShortestJoinedEnding = 7;
        public const int MaxRuptureSize = 5;
        public const int MaxGapSize = 20;
        public const int GapAngleOffset = 22;
        public const int ToleratedGapOverlap = 2;
        public const int MinTailLength = 21;
        public const int MinFragmentLength = 22;
        public const int MaxDistanceError = 13;
        public const float MaxAngleError = FloatAngle.Pi / 180 * 10;
        public const double MaxGapAngle = Math.PI / 180 * 45;
        public const int RidgeDirectionSample = 21;
        public const int RidgeDirectionSkip = 1;
        public const int MaxTriedRoots = 70;
        public const int MinRootEdgeLength = 58;
        public const int MaxRootEdgeLookups = 1633;
        public const int MinSupportingEdges = 1;
        public const double DistanceErrorFlatness = 0.69;
        public const double AngleErrorFlatness = 0.27;
        public const double MinutiaScore = 0.032;
        public const double MinutiaFractionScore = 8.98;
        public const double MinutiaTypeScore = 0.629;
        public const double SupportedMinutiaScore = 0.193;
        public const double EdgeScore = 0.265;
        public const double DistanceAccuracyScore = 9.9;
        public const double AngleAccuracyScore = 2.79;
        public const double ThresholdFmrMax = 8.48;
        public const double ThresholdFmr2 = 11.12;
        public const double ThresholdFmr10 = 14.15;
        public const double ThresholdFmr100 = 18.22;
        public const double ThresholdFmr1000 = 22.39;
        public const double ThresholdFmr10K = 27.24;
        public const double ThresholdFmr100K = 32.01;
    }
}
