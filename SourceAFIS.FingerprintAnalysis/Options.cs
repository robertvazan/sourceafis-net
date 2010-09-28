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

        public LayerType DisplayLayer { get; set; }
        public SkeletonType SkeletonType { get; set; }
        public QuickCompare CompareWith { get; set; }
        public LayerType CompareWithLayer { get; set; }
        public DiffType DiffType { get; set; }
        public MaskType Mask { get; set; }
        public bool Contrast { get; set; }
        public bool AbsoluteContrast { get; set; }
        public bool RelativeContrast { get; set; }
        public bool LowContrastMajority { get; set; }
        public bool Orientation { get; set; }
        public bool MinutiaCollector { get; set; }
        public bool PairedMinutiae { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
