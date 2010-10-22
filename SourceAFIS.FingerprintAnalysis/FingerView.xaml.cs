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

namespace SourceAFIS.FingerprintAnalysis
{
    public partial class FingerView : UserControl
    {
        public static readonly DependencyProperty BlenderOutputProperty
            = DependencyProperty.Register("BlenderOutput", typeof(BitmapSource), typeof(FingerView));
        public BitmapSource BlenderOutput
        {
            get { return (BitmapSource)GetValue(BlenderOutputProperty); }
            set { SetValue(BlenderOutputProperty, value); }
        }

        public FingerView()
        {
            InitializeComponent();
        }
    }
}
