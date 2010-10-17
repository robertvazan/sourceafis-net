using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SourceAFIS.General;

namespace SourceAFIS.FingerprintAnalysis
{
    public class LogCollector : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected DetailLogger Logger = new DetailLogger();

        DetailLogger.LogData LogsValue;
        public DetailLogger.LogData Logs
        {
            get { return LogsValue; }
            set { LogsValue = value; OnPropertyChanged("Logs"); }
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
