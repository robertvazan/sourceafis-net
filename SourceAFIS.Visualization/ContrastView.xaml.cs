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
using WPoint = System.Windows.Point;
using WSize = System.Windows.Size;

namespace SourceAFIS.Visualization
{
    public partial class ContrastView : UserControl
    {
        public static readonly DependencyProperty BlocksProperty
            = DependencyProperty.Register("Blocks", typeof(BlockMap), typeof(ContrastView),
            new PropertyMetadata((self, args) => { (self as ContrastView).UpdatePies(); }));
        public BlockMap Blocks
        {
            get { return (BlockMap)GetValue(BlocksProperty); }
            set { SetValue(BlocksProperty, value); }
        }

        public static readonly DependencyProperty ContrastMapProperty
            = DependencyProperty.Register("ContrastMap", typeof(byte[,]), typeof(ContrastView),
            new PropertyMetadata((self, args) => { (self as ContrastView).UpdatePies(); }));
        public byte[,] ContrastMap
        {
            get { return (byte[,])GetValue(ContrastMapProperty); }
            set { SetValue(ContrastMapProperty, value); }
        }

        static readonly DependencyPropertyKey PieGeometryProperty
            = DependencyProperty.RegisterReadOnly("PieGeometry", typeof(Geometry), typeof(ContrastView), null);
        public Geometry PieGeometry
        {
            get { return (Geometry)GetValue(PieGeometryProperty.DependencyProperty); }
        }

        void UpdatePies()
        {
            if (IsVisible && Blocks != null && ContrastMap != null
                && Blocks.BlockCount.Width == ContrastMap.GetLength(1)
                && Blocks.BlockCount.Height == ContrastMap.GetLength(0))
            {
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;
                var blocks = Blocks;
                var contrasts = ContrastMap;
                using (StreamGeometryContext context = geometry.Open())
                {
                    foreach (var block in Blocks.AllBlocks)
                    {
                        var centerX = blocks.BlockCenters[block].X;
                        var centerY = blocks.PixelCount.Height - blocks.BlockCenters[block].Y;
                        var radius = 0.37 * Math.Min(blocks.BlockAreas[block].Width, blocks.BlockAreas[block].Height);
                        var contrast = contrasts[block.Y, block.X] / 255f;
                        var angle = Angle.ToVector(Angle.FromFraction(contrast));

                        context.BeginFigure(new WPoint(centerX, centerY), true, true);
                        context.LineTo(new WPoint(centerX + angle.Y * radius, centerY - angle.X * radius), true, true);
                        context.ArcTo(new WPoint(centerX, centerY - radius), new WSize(radius, radius), 0, contrast > 0.5, SweepDirection.Counterclockwise, true, true);
                    }
                }
                geometry.Freeze();
                SetValue(PieGeometryProperty, geometry);
            }
            else
                SetValue(PieGeometryProperty, null);
        }

        public ContrastView()
        {
            InitializeComponent();
            IsVisibleChanged += (sender, args) => { UpdatePies(); };
        }
    }
}
