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
using Microsoft.Win32;
using SourceAFIS.General;

namespace SourceAFIS.FingerprintAnalysis
{
    public partial class MainWindow : Window
    {
        Options Options;
        LogCollector Collector = new LogCollector();
        Blender Blender = new Blender();

        public MainWindow()
        {
            InitializeComponent();
            Options = FindResource("OptionsData") as Options;
            Blender.Options = Options;
            Blender.Logs = Collector;
            Options.PropertyChanged += (source, args) => { OnOptionsChange(); };
        }

        private void LeftOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
                Options.ProbePath = dialog.FileName;
        }

        private void RightOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
                Options.CandidatePath = dialog.FileName;
        }

        void OnOptionsChange()
        {
            if (Options.ProbePath != "")
                Collector.Probe.InputImage = ImageIO.Load(Options.ProbePath);
            else
                Collector.Probe.InputImage = null;
            if (Options.CandidatePath != "")
                Collector.Candidate.InputImage = ImageIO.Load(Options.CandidatePath);
            else
                Collector.Candidate.InputImage = null;

            if (Collector.Probe.InputImage != null)
            {
                Collector.Collect();
                Blender.Blend();

                MemoryStream streamed = new MemoryStream();
                Blender.OutputImage.Save(streamed, System.Drawing.Imaging.ImageFormat.Jpeg);

                System.Windows.Media.Imaging.BitmapImage converted = new System.Windows.Media.Imaging.BitmapImage();
                converted.BeginInit();
                converted.StreamSource = new MemoryStream(streamed.ToArray());
                converted.EndInit();

                LeftImage.Source = converted;
            }
        }
    }
}
