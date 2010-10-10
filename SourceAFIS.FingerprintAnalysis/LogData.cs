using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SourceAFIS.General;
using System.ComponentModel;

namespace SourceAFIS.FingerprintAnalysis
{
    public class LogData : INotifyPropertyChanged
    {
        List<LogProperty> LogProperties = new List<LogProperty>();
        List<ComputedProperty> ComputedProperties = new List<ComputedProperty>();

        public Func<string, string> LogStringDecoration = log => log;

        public Func<Options, bool> Filter = options => true;
        public Func<FingerprintOptions, bool> FpFilter = options => true;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RegisterProperties()
        {
            foreach (var field in this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.FieldType == typeof(LogProperty))
                {
                    LogProperty property = field.GetValue(this) as LogProperty;
                    property.Name = field.Name.Substring(0, field.Name.Length - 8);
                    LogProperties.Add(property);
                }
                if (field.FieldType == typeof(ComputedProperty))
                {
                    ComputedProperty property = field.GetValue(this) as ComputedProperty;
                    property.Name = field.Name.Substring(0, field.Name.Length - 8);
                    ComputedProperties.Add(property);
                }
            }
        }

        public void CollectLogs(DetailLogger logger)
        {
            foreach (LogProperty property in LogProperties)
            {
                object oldValue = property.Value;
                property.Value = logger.Retrieve(LogStringDecoration(property.Log));
                if (PropertyChanged != null && (oldValue != null || property.Value != null))
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(property.Name));
                    foreach (ComputedProperty dependent in property.DependentProperties)
                        PropertyChanged(this, new PropertyChangedEventArgs(dependent.Name));
                }
            }
        }

        public HashSet<string> CreateFilter(Options options, FingerprintOptions fpOptions = null)
        {
            HashSet<string> filtered = new HashSet<string>();
            if (Filter(options) && (fpOptions == null || FpFilter(fpOptions)))
            {
                foreach (LogProperty property in LogProperties)
                    if (property.Filter(options) && (fpOptions == null || property.FpFilter(fpOptions)))
                        filtered.Add(property.Log);
            }
            return filtered;
        }
    }
}
