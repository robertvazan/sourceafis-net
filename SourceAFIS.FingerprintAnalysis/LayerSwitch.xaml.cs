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
    public partial class LayerSwitch : UserControl
    {
        public static readonly DependencyProperty LayerProperty
            = DependencyProperty.Register("Layer", typeof(Layer), typeof(LayerSwitch), new PropertyMetadata(Layer.None));
        public Layer Layer
        {
            get { return (Layer)GetValue(LayerProperty); }
            set { SetValue(LayerProperty, value); }
        }

        public static readonly DependencyProperty ExtractionDataProperty
            = DependencyProperty.Register("ExtractionData", typeof(ExtractionData), typeof(LayerSwitch));
        public ExtractionData ExtractionData
        {
            get { return (ExtractionData)GetValue(ExtractionDataProperty); }
            set { SetValue(ExtractionDataProperty, value); }
        }

        public LayerSwitch()
        {
            InitializeComponent();
        }
    }
}
