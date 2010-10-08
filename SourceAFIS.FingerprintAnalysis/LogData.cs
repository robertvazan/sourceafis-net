using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SourceAFIS.FingerprintAnalysis
{
    public class LogData
    {
        List<LogProperty> LogProperties = new List<LogProperty>();

        protected void RegisterProperties()
        {
            var fields = from field in this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                         where field.FieldType == typeof(LogProperty)
                         select field;
            foreach (var field in fields)
            {
                LogProperty property = field.GetValue(this) as LogProperty;
                property.Name = field.Name.Substring(0, field.Name.Length - 8);
                LogProperties.Add(property);
            }
        }

        public void SetProperty(string name, object value)
        {
            foreach (LogProperty property in LogProperties.Where(property => property.Name == name))
                property.Value = value;
        }
    }
}
