using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SourceAFIS.FingerprintAnalysis
{
    public class Options : INotifyPropertyChanged
    {
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

        SkeletonType SkeletonTypeValue;
        public SkeletonType SkeletonType
        {
            get { return SkeletonTypeValue; }
            set { SkeletonTypeValue = value; OnPropertyChanged("SkeletonType"); }
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
        public DiffType DiffType
        {
            get { return DiffTypeValue; }
            set { DiffTypeValue = value; OnPropertyChanged("DiffType"); }
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
