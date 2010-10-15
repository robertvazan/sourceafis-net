using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using SourceAFIS.General;

namespace SourceAFIS.FingerprintAnalysis
{
    public class LogData : INotifyPropertyChanged
    {
        public Func<string, string> LogStringDecoration = log => log;

        INotifyPropertyChanged Collector;
        PropertyInfo CollectorProperty;

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetSource(INotifyPropertyChanged collector, string propertyName)
        {
            Collector = collector;
            CollectorProperty = Collector.GetType().GetProperty(propertyName);
        }

        class NotificationChaining
        {
            string Property;
            Action Target;

            public NotificationChaining(INotifyPropertyChanged source, string property, Action target)
            {
                Property = property;
                Target = target;
                source.PropertyChanged += Notify;
            }

            public void Notify(object source, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == Property)
                {
                    (source as INotifyPropertyChanged).PropertyChanged -= Notify;
                    Target();
                }
            }
        }

        public void Watch(INotifyPropertyChanged source, string sourceProperty, string targetProperty)
        {
            new NotificationChaining(source, sourceProperty, () =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(targetProperty));
            });
        }

        public void Watch(string sourceProperty, string targetProperty)
        {
            Watch(this, sourceProperty, targetProperty);
        }

        public object GetLog(string propertyName, string logName)
        {
            Watch(Collector, CollectorProperty.Name, propertyName);
            DetailLogger.LogData logs = CollectorProperty.GetValue(Collector, null) as DetailLogger.LogData;
            return logs.Retrieve(LogStringDecoration(logName));
        }
    }
}
