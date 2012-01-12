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
using SourceAFIS.General;
using SourceAFIS.Templates;

namespace SourceAFIS.Visualization
{
    public partial class Minutiae : UserControl
    {
        public struct MinutiaInfo
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Angle { get; set; }
            public TemplateBuilder.MinutiaType Type { get; set; }
        }

        public static readonly DependencyProperty FpTemplateProperty
            = DependencyProperty.Register("FpTemplate", typeof(TemplateBuilder), typeof(Minutiae),
            new PropertyMetadata((self, args) => { (self as Minutiae).UpdateLists(); }));
        public TemplateBuilder FpTemplate
        {
            get { return (TemplateBuilder)GetValue(FpTemplateProperty); }
            set { SetValue(FpTemplateProperty, value); }
        }

        static readonly DependencyPropertyKey EndingsProperty
            = DependencyProperty.RegisterReadOnly("Endings", typeof(IEnumerable<MinutiaInfo>), typeof(Minutiae), null);
        public IEnumerable<MinutiaInfo> Endings
        {
            get { return (IEnumerable<MinutiaInfo>)GetValue(EndingsProperty.DependencyProperty); }
        }

        static readonly DependencyPropertyKey BifurcationsProperty
            = DependencyProperty.RegisterReadOnly("Bifurcations", typeof(IEnumerable<MinutiaInfo>), typeof(Minutiae), null);
        public IEnumerable<MinutiaInfo> Bifurcations
        {
            get { return (IEnumerable<MinutiaInfo>)GetValue(BifurcationsProperty.DependencyProperty); }
        }

        void UpdateLists()
        {
            if (IsVisible && FpTemplate != null)
            {
                var list = from minutia in FpTemplate.Minutiae
                           let dpiScaling = FpTemplate.OriginalDpi / 500.0
                           select new MinutiaInfo()
                           {
                               X = dpiScaling * (minutia.Position.X + 0.5),
                               Y = FpTemplate.OriginalHeight - dpiScaling * (minutia.Position.Y + 0.5),
                               Angle = Angle.ToDegrees(Angle.Complementary(minutia.Direction)),
                               Type = minutia.Type
                           };
                SetValue(EndingsProperty, list.Where(minutia => minutia.Type == TemplateBuilder.MinutiaType.Ending).ToList());
                SetValue(BifurcationsProperty, list.Where(minutia => minutia.Type == TemplateBuilder.MinutiaType.Bifurcation).ToList());
            }
            else
            {
                SetValue(EndingsProperty, null);
                SetValue(BifurcationsProperty, null);
            }
        }

        public Minutiae()
        {
            InitializeComponent();
            IsVisibleChanged += (sender, args) => { UpdateLists(); };
        }
    }
}
