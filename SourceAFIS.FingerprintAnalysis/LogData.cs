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
            INotifyPropertyChanged Source;
            string SourceProperty;
            LogData Target;
            string TargetProperty;

            static List<NotificationChaining> Registry = new List<NotificationChaining>();

            public NotificationChaining(INotifyPropertyChanged source, string sourceProperty, LogData target, string targetProperty)
            {
                Source = source;
                SourceProperty = sourceProperty;
                Target = target;
                TargetProperty = targetProperty;

                foreach (NotificationChaining other in Registry)
                {
                    if (other.Source == Source && other.SourceProperty == SourceProperty
                        && other.Target == Target && other.TargetProperty == TargetProperty)
                    {
                        return;
                    }
                }

                Registry.Add(this);

                source.PropertyChanged += Notify;
            }

            public void Notify(object source, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == SourceProperty)
                {
                    Source.PropertyChanged -= Notify;

                    Registry.Remove(this);
                    
                    Target.PropertyChanged(Target, new PropertyChangedEventArgs(TargetProperty));
                }
            }
        }

        public void Watch(INotifyPropertyChanged source, string sourceProperty, string targetProperty)
        {
            new NotificationChaining(source, sourceProperty, this, targetProperty);
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
