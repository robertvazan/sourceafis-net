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
    public partial class BlockMaskBorder : UserControl
    {
        public static readonly DependencyProperty MaskProperty
            = DependencyProperty.Register("Mask", typeof(BinaryMap), typeof(BlockMaskBorder),
            new PropertyMetadata((self, args) => { (self as BlockMaskBorder).UpdateScaled(); }));
        public BinaryMap Mask
        {
            get { return (BinaryMap)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        public static readonly DependencyProperty BlocksProperty
            = DependencyProperty.Register("Blocks", typeof(BlockMap), typeof(BlockMaskBorder),
            new PropertyMetadata((self, args) => { (self as BlockMaskBorder).UpdateScaled(); }));
        public BlockMap Blocks
        {
            get { return (BlockMap)GetValue(BlocksProperty); }
            set { SetValue(BlocksProperty, value); }
        }

        public static readonly DependencyProperty InvertedProperty
            = DependencyProperty.Register("Inverted", typeof(bool), typeof(BlockMaskBorder), new PropertyMetadata(false));
        public bool Inverted
        {
            get { return (bool)GetValue(InvertedProperty); }
            set { SetValue(InvertedProperty, value); }
        }

        static readonly DependencyPropertyKey ScaledProperty
            = DependencyProperty.RegisterReadOnly("Scaled", typeof(BinaryMap), typeof(BlockMaskBorder), null);
        public BinaryMap Scaled
        {
            get { return (BinaryMap)GetValue(ScaledProperty.DependencyProperty); }
        }

        void UpdateScaled()
        {
            if (Mask != null && Blocks != null && Mask.Size == Blocks.BlockCount)
                SetValue(ScaledProperty, Mask.FillBlocks(Blocks));
            else
                SetValue(ScaledProperty, null);
        }

        public BlockMaskBorder()
        {
            InitializeComponent();
        }
    }
}
