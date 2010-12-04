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
using System.Windows.Threading;
using SourceAFIS.Tuning.Errors;

namespace SourceAFIS.Visualization
{
    public partial class RocGraph : UserControl
    {
        public static readonly DependencyProperty CurveProperty
            = DependencyProperty.Register("Curve", typeof(ROCCurve), typeof(RocGraph),
            new PropertyMetadata((self, args) => { (self as RocGraph).UpdatePoints(); }));
        public ROCCurve Curve
        {
            get { return (ROCCurve)GetValue(CurveProperty); }
            set { SetValue(CurveProperty, value); }
        }

        static readonly DependencyPropertyKey PointsProperty
            = DependencyProperty.RegisterReadOnly("Points", typeof(PointCollection), typeof(RocGraph), null);
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty.DependencyProperty); }
        }

        double TransformAxis(double rate)
        {
            return Math.Log10(Math.Min(1, Math.Max(rate, 0.00001))) * -100;
        }

        void UpdatePoints()
        {
            PointCollection points = new PointCollection();
            foreach (var rate in Curve.Curve)
            {
                double LogFAR = TransformAxis(rate.FAR);
                double LogFRR = TransformAxis(rate.FRR);
                points.Add(new Point(500 - LogFAR, LogFRR));
            }
            SetValue(PointsProperty, points);
        }

        public RocGraph()
        {
            InitializeComponent();
        }

        public static BitmapSource Render(ROCCurve curve)
        {
            RocGraph graph = new RocGraph();
            graph.Curve = curve;

            graph.Arrange(new Rect(0, 0, 600, 600));
            graph.UpdateLayout();
            graph.Dispatcher.Invoke(DispatcherPriority.Loaded, new Action(() => { }));

            RenderTargetBitmap render = new RenderTargetBitmap(
                Convert.ToInt32(graph.ActualWidth), Convert.ToInt32(graph.ActualHeight),
                96, 96, PixelFormats.Pbgra32);
            render.Render(graph);

            return render;
        }
    }
}
