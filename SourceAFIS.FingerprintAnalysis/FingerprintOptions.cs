using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SourceAFIS.FingerprintAnalysis
{
    public class FingerprintOptions : INotifyPropertyChanged
    {
        string PathValue = "";
        public string Path
        {
            get { return PathValue; }
            set { PathValue = value; OnPropertyChanged("Path"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
