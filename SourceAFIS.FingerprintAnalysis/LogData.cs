using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SourceAFIS.General;

namespace SourceAFIS.FingerprintAnalysis
{
    public class LogData
    {
        List<LogProperty> LogProperties = new List<LogProperty>();

        public Func<string, string> LogStringDecoration = log => log;

        public Func<Options, bool> Filter = options => true;
        public Func<FingerprintOptions, bool> FpFilter = options => true;

        protected void RegisterProperties()
        {
            var fields = from field in this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                         where field.FieldType == typeof(LogProperty)
                         select field;
            foreach (var field in fields)
            {
                LogProperty property = field.GetValue(this) as LogProperty;
                property.Name = field.Name.Substring(0, field.Name.Length - 8);
                LogProperties.Add(property);
            }
        }

        public void CollectLogs(DetailLogger logger)
        {
            foreach (LogProperty property in LogProperties)
                property.Value = logger.Retrieve(LogStringDecoration(property.Log));
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
