using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SourceAFIS.FingerprintAnalysis
{
    public class FingerprintOptions : INotifyPropertyChanged, IPushNotification
    {
        string PathValue = "";
        public string Path
        {
            get { return PathValue; }
            set { PathValue = value; OnPropertyChanged("Path"); }
        }

        public string FileName
        {
            get
            {
                new NotificationLink(this, "Path", this, "FileName");
                return System.IO.Path.GetFileNameWithoutExtension(Path);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        void IPushNotification.PushNotification(string property) { OnPropertyChanged(property); }
    }
}
