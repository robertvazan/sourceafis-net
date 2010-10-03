using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SourceAFIS.FingerprintAnalysis
{
    public class Options : INotifyPropertyChanged
    {
        public enum Layer
        {
            OriginalImage,
            Equalized,
            SmoothedRidges,
            OrthogonalSmoothing,
            Binarized,
            BinarySmoothing,
            RemovedCrosses,
            Thinned,
            RidgeTracer,
            DotRemover,
            PoreRemover,
            GapRemover,
            TailRemover,
            FragmentRemover,
            MinutiaMask,
            BranchMinutiaRemover
        }

        public enum SkeletonType
        {
            Ridges,
            Valleys
        }

        public enum QuickCompareType
        {
            None,
            Previous,
            Next,
            OtherLayer
        }
        
        public enum DiffType
        {
            Proportional,
            Normalized,
            Fog,
            Binary
        }

        public enum MaskType
        {
            None,
            Segmentation,
            Inner
        }

        public bool EnableImageDisplay = true;

        FingerprintOptions ProbeValue = new FingerprintOptions();
        public FingerprintOptions Probe
        {
            get { return ProbeValue; }
            set { ProbeValue = value; OnPropertyChanged("Probe"); }
        }

        FingerprintOptions CandidateValue = new FingerprintOptions();
        public FingerprintOptions Candidate
        {
            get { return CandidateValue; }
            set { CandidateValue = value; OnPropertyChanged("Candidate"); }
        }

        Layer DisplayLayerValue;
        public Layer DisplayLayer
        {
            get { return DisplayLayerValue; }
            set { DisplayLayerValue = value; OnPropertyChanged("DisplayLayer"); }
        }

        SkeletonType SkeletonValue;
        public SkeletonType Skeleton
        {
            get { return SkeletonValue; }
            set { SkeletonValue = value; OnPropertyChanged("Skeleton"); }
        }

        QuickCompareType CompareWithValue;
        public QuickCompareType CompareWith
        {
            get { return CompareWithValue; }
            set { CompareWithValue = value; OnPropertyChanged("CompareWith"); }
        }

        Layer CompareWithLayerValue;
        public Layer CompareWithLayer
        {
            get { return CompareWithLayerValue; }
            set { CompareWithLayerValue = value; OnPropertyChanged("CompareWithLayer"); }
        }

        DiffType DiffTypeValue;
        public DiffType DiffMode
        {
            get { return DiffTypeValue; }
            set { DiffTypeValue = value; OnPropertyChanged("DiffMode"); }
        }

        MaskType MaskValue;
        public MaskType Mask
        {
            get { return MaskValue; }
            set { MaskValue = value; OnPropertyChanged("Mask"); }
        }

        bool ContrastValue;
        public bool Contrast
        {
            get { return ContrastValue; }
            set { ContrastValue = value; OnPropertyChanged("Contrast"); }
        }

        bool AbsoluteContrastValue;
        public bool AbsoluteContrast
        {
            get { return AbsoluteContrastValue; }
            set { AbsoluteContrastValue = value; OnPropertyChanged("AbsoluteContrast"); }
        }

        bool RelativeContrastValue;
        public bool RelativeContrast
        {
            get { return RelativeContrastValue; }
            set { RelativeContrastValue = value; OnPropertyChanged("RelativeContrast"); }
        }

        bool LowContrastMajorityValue;
        public bool LowContrastMajority
        {
            get { return LowContrastMajorityValue; }
            set { LowContrastMajorityValue = value; OnPropertyChanged("LowContrastMajority"); }
        }

        bool OrientationValue;
        public bool Orientation
        {
            get { return OrientationValue; }
            set { OrientationValue = value; OnPropertyChanged("Orientation"); }
        }

        bool MinutiaeValue;
        public bool Minutiae
        {
            get { return MinutiaeValue; }
            set { MinutiaeValue = value; OnPropertyChanged("Minutiae"); }
        }

        bool PairedMinutiaeValue;
        public bool PairedMinutiae
        {
            get { return PairedMinutiaeValue; }
            set { PairedMinutiaeValue = value; OnPropertyChanged("PairedMinutiae"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public static Array GetEnumValues(string name)
        {
            return Enum.GetValues(Type.GetType(name));
        }
    }
}
