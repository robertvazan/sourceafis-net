using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
#if !MONO
using Microsoft.Win32;
#endif

namespace FingerprintAnalyzer
{
    sealed class PersistentStore
    {
        const string OptionsPath = @"Software\SourceAFIS\FingerprintAnalyzer\Options";

        public static void Save(Options options)
        {
#if !MONO
            Save(options, Registry.CurrentUser.CreateSubKey(OptionsPath));
#endif
        }

        public static void Load(Options options)
        {
#if !MONO
            RegistryKey key = Registry.CurrentUser.OpenSubKey(OptionsPath);
            if (key != null)
                Load(options, key);
#endif
        }

#if !MONO
        public static void Save(object root, RegistryKey key)
        {
            foreach (FieldInfo fieldInfo in root.GetType().GetFields())
            {
                if (fieldInfo.FieldType == typeof(bool))
                    key.SetValue(fieldInfo.Name, fieldInfo.GetValue(root));
                if (fieldInfo.FieldType.IsEnum)
                    key.SetValue(fieldInfo.Name, fieldInfo.GetValue(root));
                if (fieldInfo.FieldType.IsClass)
                    Save(fieldInfo.GetValue(root), key.CreateSubKey(fieldInfo.Name));
            }
        }

        public static void Load(object root, RegistryKey key)
        {
            foreach (FieldInfo fieldInfo in root.GetType().GetFields())
            {
                try
                {
                    if (key.GetValue(fieldInfo.Name) != null)
                    {
                        if (fieldInfo.FieldType == typeof(bool))
                            fieldInfo.SetValue(root, Convert.ToBoolean(key.GetValue(fieldInfo.Name)));
                        if (fieldInfo.FieldType.IsEnum)
                            fieldInfo.SetValue(root, Enum.Parse(fieldInfo.FieldType, (string)key.GetValue(fieldInfo.Name)));
                    }
                    if (fieldInfo.FieldType.IsClass)
                    {
                        RegistryKey subkey = key.OpenSubKey(fieldInfo.Name);
                        if (subkey != null)
                            Load(fieldInfo.GetValue(root), subkey);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(String.Format("Could not read {0} from registry. {1}", fieldInfo.Name, e.Message));
                }
            }
        }
#endif
    }
}
