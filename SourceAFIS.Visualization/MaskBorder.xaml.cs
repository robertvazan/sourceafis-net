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
using APoint = SourceAFIS.General.Point;

namespace SourceAFIS.Visualization
{
    public partial class MaskBorder : UserControl
    {
        public static readonly DependencyProperty MaskProperty
            = DependencyProperty.Register("Mask", typeof(BinaryMap), typeof(MaskBorder),
            new PropertyMetadata((self, args) => { (self as MaskBorder).UpdateBorder(); }));
        public BinaryMap Mask
        {
            get { return (BinaryMap)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        public static readonly DependencyProperty InvertedProperty
            = DependencyProperty.Register("Inverted", typeof(bool), typeof(MaskBorder),
            new PropertyMetadata(false, (self, args) => { (self as MaskBorder).UpdateBorder(); }));
        public bool Inverted
        {
            get { return (bool)GetValue(InvertedProperty); }
            set { SetValue(InvertedProperty, value); }
        }

        static readonly DependencyPropertyKey BorderProperty
            = DependencyProperty.RegisterReadOnly("Border", typeof(BinaryMap), typeof(MaskBorder), null);
        public BinaryMap Border
        {
            get { return (BinaryMap)GetValue(BorderProperty.DependencyProperty); }
        }

        BinaryMap Subtract(BinaryMap outer, BinaryMap inner)
        {
            BinaryMap buffer = new BinaryMap(outer);
            buffer.AndNot(inner);
            return buffer;
        }

        BinaryMap Expand(BinaryMap map)
        {
            BinaryMap buffer = new BinaryMap(map);
            buffer.Or(map, new RectangleC(0, 0, map.Width - 1, map.Height), new APoint(1, 0));
            buffer.Or(map, new RectangleC(1, 0, map.Width - 1, map.Height), new APoint(0, 0));
            buffer.Or(map, new RectangleC(0, 0, map.Width, map.Height - 1), new APoint(0, 1));
            buffer.Or(map, new RectangleC(0, 1, map.Width, map.Height - 1), new APoint(0, 0));
            buffer.Or(map, new RectangleC(0, 0, map.Width - 1, map.Height - 1), new APoint(1, 1));
            buffer.Or(map, new RectangleC(1, 1, map.Width - 1, map.Height - 1), new APoint(0, 0));
            buffer.Or(map, new RectangleC(0, 1, map.Width - 1, map.Height - 1), new APoint(1, 0));
            buffer.Or(map, new RectangleC(1, 0, map.Width - 1, map.Height - 1), new APoint(0, 1));
            return buffer;
        }

        void UpdateBorder()
        {
            if (IsVisible && Mask != null)
            {
                BinaryMap mask = Inverted ? Mask.GetInverted() : Mask;
                SetValue(BorderProperty, Subtract(Expand(Expand(mask)), mask));
            }
            else
                SetValue(BorderProperty, null);
        }

        public MaskBorder()
        {
            InitializeComponent();
            IsVisibleChanged += (sender, args) => { UpdateBorder(); };
        }
    }
}
