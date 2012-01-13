using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SourceAFIS.FingerprintAnalysis
{
    public class Options : INotifyPropertyChanged
    {
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

        BitmapLayer BitmapLayerValue = BitmapLayer.OriginalImage;
        public BitmapLayer BitmapLayer
        {
            get { return BitmapLayerValue; }
            set { BitmapLayerValue = value; OnPropertyChanged("BitmapLayer"); }
        }

        MarkerLayer MarkerLayerValue = MarkerLayer.MinutiaMask;
        public MarkerLayer MarkerLayer
        {
            get { return MarkerLayerValue; }
            set { MarkerLayerValue = value; OnPropertyChanged("MarkerLayer"); }
        }

        SkeletonType SkeletonValue;
        public SkeletonType Skeleton
        {
            get { return SkeletonValue; }
            set { SkeletonValue = value; OnPropertyChanged("Skeleton"); }
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

        bool OrientationValue;
        public bool Orientation
        {
            get { return OrientationValue; }
            set { OrientationValue = value; OnPropertyChanged("Orientation"); }
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
