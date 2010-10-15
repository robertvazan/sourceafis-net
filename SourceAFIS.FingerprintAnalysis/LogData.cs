using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using SourceAFIS.General;

namespace SourceAFIS.FingerprintAnalysis
{
    public class LogData : INotifyPropertyChanged, IPushNotification
    {
        public Func<string, string> LogStringDecoration = log => log;

        INotifyPropertyChanged Collector;
        PropertyInfo CollectorProperty;

        public event PropertyChangedEventHandler PropertyChanged;

        void IPushNotification.PushNotification(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public void SetSource(INotifyPropertyChanged collector, string propertyName)
        {
            Collector = collector;
            CollectorProperty = Collector.GetType().GetProperty(propertyName);
        }

        public void Link(INotifyPropertyChanged source, string sourceProperty, string targetProperty)
        {
            new NotificationLink(source, sourceProperty, this, targetProperty);
        }

        public void Link(string sourceProperty, string targetProperty)
        {
            Link(this, sourceProperty, targetProperty);
        }

        public object GetLog(string propertyName, string logName)
        {
            Link(Collector, CollectorProperty.Name, propertyName);
            DetailLogger.LogData logs = CollectorProperty.GetValue(Collector, null) as DetailLogger.LogData;
            return logs.Retrieve(LogStringDecoration(logName));
        }
    }
}
