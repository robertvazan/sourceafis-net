using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.Win32;

namespace FingerprintAnalyzer
{
    sealed class PersistentStore
    {
        const string RegistryPath = @"Software\SourceAFIS\FingerprintAnalyzer";
        const string OptionsPath = RegistryPath + @"\Options";

        public static void Save(Options options)
        {
            Save(options, Registry.CurrentUser.CreateSubKey(OptionsPath));
        }

        public static void Load(Options options)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(OptionsPath);
            if (key != null)
                Load(options, key);
        }

        public static void Save(Form form)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath + @"\" + form.GetType().Name);
            key.SetValue("Visible", form.Visible);
            key.SetValue("WindowState", form.WindowState);
            key.SetValue("Left", form.Left);
            key.SetValue("Top", form.Top);
            key.SetValue("Width", form.Width);
            key.SetValue("Height", form.Height);
        }

        public static void Load(Form form)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath + @"\" + form.GetType().Name);
                if (key != null)
                {
                    form.WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), (string)key.GetValue("WindowState"));
                    form.Left = Convert.ToInt32(key.GetValue("Left"));
                    form.Top = Convert.ToInt32(key.GetValue("Top"));
                    form.Width = Convert.ToInt32(key.GetValue("Width"));
                    form.Height = Convert.ToInt32(key.GetValue("Height"));
                    form.StartPosition = FormStartPosition.Manual;
                    form.Visible = Convert.ToBoolean(key.GetValue("Visible"));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("Could not read form {0} settings from registry. {1}", form.GetType().Name, e.ToString()));
            }
        }

        public static void Save(string name, object value)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath);
            if (value != null)
                key.SetValue(name, value);
            else
                key.DeleteValue(name, false);
        }

        public static void Load(string name, ref string value)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath);
                if (key != null && key.GetValue(name) != null)
                    value = Convert.ToString(key.GetValue(name));
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("Could not read entry {0} from registry. {1}", name, e.ToString()));
            }
        }

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
                    MessageBox.Show(String.Format("Could not read {0} from registry. {1}", fieldInfo.Name, e.ToString()));
                }
            }
        }
    }
}
