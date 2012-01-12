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
using SourceAFIS.Matching.Minutia;
using SourceAFIS.Templates;

namespace SourceAFIS.Visualization
{
    public partial class MatchedMinutiae : UserControl
    {
        public static readonly DependencyProperty PairingProperty
            = DependencyProperty.Register("Pairing", typeof(MinutiaPairing), typeof(MatchedMinutiae),
            new PropertyMetadata((self, args) => { (self as MatchedMinutiae).UpdatePositions(); }));
        public MinutiaPairing Pairing
        {
            get { return (MinutiaPairing)GetValue(PairingProperty); }
            set { SetValue(PairingProperty, value); }
        }

        public static readonly DependencyProperty FpTemplateProperty
            = DependencyProperty.Register("FpTemplate", typeof(TemplateBuilder), typeof(MatchedMinutiae),
            new PropertyMetadata((self, args) => { (self as MatchedMinutiae).UpdatePositions(); }));
        public TemplateBuilder FpTemplate
        {
            get { return (TemplateBuilder)GetValue(FpTemplateProperty); }
            set { SetValue(FpTemplateProperty, value); }
        }

        public static readonly DependencyProperty MatchSideProperty
            = DependencyProperty.Register("MatchSide", typeof(MatchSide), typeof(MatchedMinutiae),
            new PropertyMetadata(MatchSide.Probe, (self, args) => { (self as MatchedMinutiae).UpdatePositions(); }));
        public MatchSide MatchSide
        {
            get { return (MatchSide)GetValue(MatchSideProperty); }
            set { SetValue(MatchSideProperty, value); }
        }

        static readonly DependencyPropertyKey PositionsProperty
            = DependencyProperty.RegisterReadOnly("Positions", typeof(IEnumerable<Point>), typeof(MatchedMinutiae), null);
        public IEnumerable<Point> Positions
        {
            get { return (IEnumerable<Point>)GetValue(PositionsProperty.DependencyProperty); }
        }

        void UpdatePositions()
        {
            if (IsVisible && Pairing != null && FpTemplate != null)
            {
                var minutiae = from index in Enumerable.Range(0, Pairing.Count)
                               let pair = Pairing.GetPair(index)
                               select MatchSide == MatchSide.Probe ? pair.Probe : pair.Candidate;
                var dpiScaling = FpTemplate.OriginalDpi / 500.0;
                var points = from minutia in minutiae
                             where minutia < FpTemplate.Minutiae.Count
                             let position = FpTemplate.Minutiae[minutia].Position
                             select new Point()
                             {
                                 X = dpiScaling * position.X - 5,
                                 Y = FpTemplate.OriginalHeight - 1 - dpiScaling * position.Y - 5
                             };
                SetValue(PositionsProperty, points.ToList());
            }
            else
                SetValue(PositionsProperty, null);
        }

        public MatchedMinutiae()
        {
            InitializeComponent();
            IsVisibleChanged += (sender, args) => { UpdatePositions(); };
        }
    }
}
