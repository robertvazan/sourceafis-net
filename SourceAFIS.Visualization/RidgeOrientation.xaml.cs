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

namespace SourceAFIS.Visualization
{
    public partial class RidgeOrientation : UserControl
    {
        public struct LineInfo
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double X1 { get; set; }
            public double Y1 { get; set; }
            public double X2 { get { return -X1; } }
            public double Y2 { get { return -Y1; } }
        }

        public static readonly DependencyProperty BlocksProperty
            = DependencyProperty.Register("Blocks", typeof(BlockMap), typeof(RidgeOrientation),
            new PropertyMetadata((self, args) => { (self as RidgeOrientation).UpdateLines(); }));
        public BlockMap Blocks
        {
            get { return (BlockMap)GetValue(BlocksProperty); }
            set { SetValue(BlocksProperty, value); }
        }

        public static readonly DependencyProperty MaskProperty
            = DependencyProperty.Register("Mask", typeof(BinaryMap), typeof(RidgeOrientation),
            new PropertyMetadata((self, args) => { (self as RidgeOrientation).UpdateLines(); }));
        public BinaryMap Mask
        {
            get { return (BinaryMap)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        public static readonly DependencyProperty OrientationMapProperty
            = DependencyProperty.Register("OrientationMap", typeof(byte[,]), typeof(RidgeOrientation),
            new PropertyMetadata((self, args) => { (self as RidgeOrientation).UpdateLines(); }));
        public byte[,] OrientationMap
        {
            get { return (byte[,])GetValue(OrientationMapProperty); }
            set { SetValue(OrientationMapProperty, value); }
        }

        static readonly DependencyPropertyKey LinesProperty
            = DependencyProperty.RegisterReadOnly("Lines", typeof(IEnumerable<LineInfo>), typeof(RidgeOrientation), null);
        public IEnumerable<LineInfo> Lines
        {
            get { return (IEnumerable<LineInfo>)GetValue(LinesProperty.DependencyProperty); }
        }

        void UpdateLines()
        {
            var lines = from block in Blocks != null ? Blocks.AllBlocks : new RectangleC()
                        where Mask != null && Mask.GetBit(block)
                        where OrientationMap != null
                        let direction = Angle.ToVector(Angle.ToDirection(OrientationMap[block.Y, block.X]))
                        select new LineInfo()
                        {
                            X = Blocks.BlockCenters[block].X,
                            Y = Blocks.PixelCount.Height - 1 - Blocks.BlockCenters[block].Y,
                            X1 = direction.X * Blocks.BlockAreas[block].Width * 0.5,
                            Y1 = -direction.Y * Blocks.BlockAreas[block].Height * 0.5
                        };
            SetValue(LinesProperty, lines.ToList());
        }

        public RidgeOrientation()
        {
            InitializeComponent();
        }
    }
}
