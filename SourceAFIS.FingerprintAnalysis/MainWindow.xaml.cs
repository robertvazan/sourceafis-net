using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Reflection;
using Microsoft.Win32;
using SourceAFIS.General;

namespace SourceAFIS.FingerprintAnalysis
{
    public partial class MainWindow : Window
    {
        static readonly string SettingsPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SourceAFIS", "FingerprintAnalysisSettings.xml");
        static readonly string ProgramVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        Options Options;
        LogDecoder Collector;

        public MainWindow()
        {
            InitializeComponent();

            Options = FindResource("OptionsData") as Options;
            LoadSettings();

            Collector = FindResource("Logs") as LogDecoder;
            Collector.Initialize(Options);
        }

        private void LeftOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
                Options.Probe.Path = dialog.FileName;
        }

        private void RightOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
                Options.Candidate.Path = dialog.FileName;
        }

        void LoadSettings()
        {
            try
            {
                XDocument xml = XDocument.Load(SettingsPath);
                if ((string)xml.Root.Attribute("ProgramVersion") == ProgramVersion)
                {
                    xml.Root.Attribute("ProgramVersion").Remove();

                    MemoryStream unversioned = new MemoryStream();
                    xml.Save(unversioned);
                    unversioned.Close();

                    XmlSerializer serializer = new XmlSerializer(typeof(Options));
                    Options loaded = serializer.Deserialize(new MemoryStream(unversioned.GetBuffer())) as Options;

                    foreach (PropertyInfo property in typeof(Options).GetProperties())
                        property.SetValue(Options, property.GetValue(loaded, null), null);
                }
            }
            catch
            {
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MemoryStream unversioned = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(Options));
            serializer.Serialize(unversioned, Options);
            unversioned.Close();

            XDocument xml = XDocument.Load(new MemoryStream(unversioned.GetBuffer()));
            xml.Root.SetAttributeValue("ProgramVersion", ProgramVersion);

            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(SettingsPath));
            xml.Save(SettingsPath);
        }
    }
}
