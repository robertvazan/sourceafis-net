using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SourceAFIS.FingerprintAnalysis
{
    public class Options : INotifyPropertyChanged
    {
        public enum LayerType
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

        public enum QuickCompare
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

        string ProbePathValue = "";
        public string ProbePath
        {
            get { return ProbePathValue; }
            set { ProbePathValue = value; OnPropertyChanged("ProbePath"); }
        }

        string CandidatePathValue = "";
        public string CandidatePath
        {
            get { return CandidatePathValue; }
            set { CandidatePathValue = value; OnPropertyChanged("CandidatePath"); }
        }

        LayerType DisplayLayerValue;
        public LayerType DisplayLayer
        {
            get { return DisplayLayerValue; }
            set { DisplayLayerValue = value; OnPropertyChanged("DisplayLayer"); }
        }

        SkeletonType SelectedSkeletonValue;
        public SkeletonType SelectedSkeleton
        {
            get { return SelectedSkeletonValue; }
            set { SelectedSkeletonValue = value; OnPropertyChanged("SelectedSkeleton"); }
        }

        QuickCompare CompareWithValue;
        public QuickCompare CompareWith
        {
            get { return CompareWithValue; }
            set { CompareWithValue = value; OnPropertyChanged("CompareWith"); }
        }

        LayerType CompareWithLayerValue;
        public LayerType CompareWithLayer
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

        bool MinutiaCollectorValue;
        public bool MinutiaCollector
        {
            get { return MinutiaCollectorValue; }
            set { MinutiaCollectorValue = value; OnPropertyChanged("MinutiaCollector"); }
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
    }
}
