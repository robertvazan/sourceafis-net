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
using System.IO;
using Microsoft.Win32;

namespace SourceAFIS.FingerprintAnalysis
{
    public partial class FilePicker : UserControl
    {
        public static readonly DependencyProperty FpOptionsProperty
            = DependencyProperty.Register("FpOptions", typeof(FingerprintOptions), typeof(FilePicker));
        public FingerprintOptions FpOptions
        {
            get { return (FingerprintOptions)GetValue(FpOptionsProperty); }
            set { SetValue(FpOptionsProperty, value); }
        }

        public static readonly DependencyProperty RenderSourceProperty
            = DependencyProperty.Register("RenderSource", typeof(FrameworkElement), typeof(FilePicker));
        public FrameworkElement RenderSource
        {
            get { return (FrameworkElement)GetValue(RenderSourceProperty); }
            set { SetValue(RenderSourceProperty, value); }
        }

        public FilePicker()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
                FpOptions.Path = dialog.FileName;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            FpOptions.Path = "";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "PNG Images | *.png";
            dialog.DefaultExt = "png";

            if (dialog.ShowDialog() == true)
            {
                double scaling = 2;
                RenderTargetBitmap render = new RenderTargetBitmap(
                    Convert.ToInt32(scaling * RenderSource.Width), Convert.ToInt32(scaling * RenderSource.Height),
                    scaling * 96, scaling * 96, PixelFormats.Pbgra32);
                render.Render(RenderSource);

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(render));
                using (var stream = File.OpenWrite(dialog.FileName))
                    encoder.Save(stream);
            }
        }
    }
}
