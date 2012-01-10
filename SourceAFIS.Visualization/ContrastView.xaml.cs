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
        public struct PieInfo
        {
            public WPoint Center { get; set; }
            public WPoint Top { get; set; }
            public WPoint ArcStart { get; set; }
            public WSize ArcSize { get; set; }
            public bool IsLargeArc { get; set; }
        }

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

        static readonly DependencyPropertyKey PiesProperty
            = DependencyProperty.RegisterReadOnly("Pies", typeof(IEnumerable<PieInfo>), typeof(ContrastView), null);
        public IEnumerable<PieInfo> Pies
        {
            get { return (IEnumerable<PieInfo>)GetValue(PiesProperty.DependencyProperty); }
        }

        void UpdatePies()
        {
            if (IsVisible && Blocks != null && ContrastMap != null
                && Blocks.BlockCount.Width == ContrastMap.GetLength(1)
                && Blocks.BlockCount.Height == ContrastMap.GetLength(0))
            {
                var blocks = Blocks;
                var contrasts = ContrastMap;
                var pies = from block in blocks.AllBlocks
                           let centerX = blocks.BlockCenters[block].X
                           let centerY = blocks.PixelCount.Height - blocks.BlockCenters[block].Y
                           let radius = 0.37 * Math.Min(blocks.BlockAreas[block].Width, blocks.BlockAreas[block].Height)
                           let contrast = contrasts[block.Y, block.X] / 255f
                           let angle = Angle.ToVector(Angle.FromFraction(contrast))
                           select new PieInfo()
                           {
                               Center = new WPoint(centerX, centerY),
                               Top = new WPoint(0, -radius),
                               ArcSize = new WSize(radius, radius),
                               ArcStart = new WPoint(angle.Y * radius, -angle.X * radius),
                               IsLargeArc = contrast > 0.5
                           };
                SetValue(PiesProperty, pies.ToList());
            }
            else
                SetValue(PiesProperty, null);
        }

        public ContrastView()
        {
            InitializeComponent();
            IsVisibleChanged += (sender, args) => { UpdatePies(); };
        }
    }
}
