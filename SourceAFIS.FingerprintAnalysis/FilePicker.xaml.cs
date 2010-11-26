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
    }
}
