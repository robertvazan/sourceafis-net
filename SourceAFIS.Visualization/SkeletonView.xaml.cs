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
using WPoint = System.Windows.Point;
using SourceAFIS.General;
using APoint = SourceAFIS.General.Point;
using SourceAFIS.Extraction.Model;

namespace SourceAFIS.Visualization
{
    public partial class SkeletonView : UserControl
    {
        public static readonly DependencyProperty OriginalWidthProperty
            = DependencyProperty.Register("OriginalWidth", typeof(int), typeof(SkeletonView),
            new PropertyMetadata(0, (self, args) => { (self as SkeletonView).UpdateView(); }));
        public int OriginalWidth
        {
            get { return (int)GetValue(OriginalWidthProperty); }
            set { SetValue(OriginalWidthProperty, value); }
        }

        public static readonly DependencyProperty OriginalHeightProperty
            = DependencyProperty.Register("OriginalHeight", typeof(int), typeof(SkeletonView),
            new PropertyMetadata(0, (self, args) => { (self as SkeletonView).UpdateView(); }));
        public int OriginalHeight
        {
            get { return (int)GetValue(OriginalHeightProperty); }
            set { SetValue(OriginalHeightProperty, value); }
        }

        public static readonly DependencyProperty SkeletonProperty
            = DependencyProperty.Register("Skeleton", typeof(SkeletonBuilder), typeof(SkeletonView),
            new PropertyMetadata((self, args) => { (self as SkeletonView).UpdateView(); }));
        public SkeletonBuilder Skeleton
        {
            get { return (SkeletonBuilder)GetValue(SkeletonProperty); }
            set { SetValue(SkeletonProperty, value); }
        }

        static readonly DependencyPropertyKey ShadowProperty
            = DependencyProperty.RegisterReadOnly("Shadow", typeof(BinaryMap), typeof(SkeletonView), null);
        public BinaryMap Shadow
        {
            get { return (BinaryMap)GetValue(ShadowProperty.DependencyProperty); }
        }

        static readonly DependencyPropertyKey ShadowBlurProperty
            = DependencyProperty.RegisterReadOnly("ShadowBlur", typeof(BinaryMap), typeof(SkeletonView), null);
        public BinaryMap ShadowBlur
        {
            get { return (BinaryMap)GetValue(ShadowBlurProperty.DependencyProperty); }
        }

        static readonly DependencyPropertyKey PositionsProperty
            = DependencyProperty.RegisterReadOnly("Positions", typeof(IEnumerable<WPoint>), typeof(SkeletonView), null);
        public IEnumerable<WPoint> Positions
        {
            get { return (IEnumerable<WPoint>)GetValue(PositionsProperty.DependencyProperty); }
        }

        void UpdateView()
        {
            if (IsVisible && Skeleton != null)
            {
                SourceAFIS.General.Size minSize = new SkeletonShadow().GetSize(Skeleton);
                if (OriginalWidth >= minSize.Width && OriginalHeight >= minSize.Height)
                {
                    BinaryMap shadow = new BinaryMap(OriginalWidth, OriginalHeight);
                    new SkeletonShadow().Draw(Skeleton, shadow);
                    SetValue(ShadowProperty, shadow);
                    BinaryMap blur = new BinaryMap(shadow);
                    blur.Or(shadow, new RectangleC(0, 0, OriginalWidth - 1, OriginalHeight), new APoint(1, 0));
                    blur.Or(shadow, new RectangleC(1, 0, OriginalWidth - 1, OriginalHeight), new APoint(0, 0));
                    blur.Or(shadow, new RectangleC(0, 0, OriginalWidth, OriginalHeight - 1), new APoint(0, 1));
                    blur.Or(shadow, new RectangleC(0, 1, OriginalWidth, OriginalHeight - 1), new APoint(0, 0));
                    SetValue(ShadowBlurProperty, blur);

                    var points = from minutia in Skeleton.Minutiae
                                 where minutia.Valid
                                 select new WPoint(minutia.Position.X - 3, OriginalHeight - 1 - minutia.Position.Y - 3);
                    SetValue(PositionsProperty, points);
                }
                else
                {
                    SetValue(ShadowProperty, null);
                    SetValue(PositionsProperty, null);
                }
            }
            else
            {
                SetValue(ShadowProperty, null);
                SetValue(PositionsProperty, null);
            }
        }

        public SkeletonView()
        {
            InitializeComponent();
            IsVisibleChanged += (sender, args) => { UpdateView(); };
        }
    }
}
