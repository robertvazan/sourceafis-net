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
using SourceAFIS.Visualization;

namespace SourceAFIS.FingerprintAnalysis
{
    public partial class FingerView : UserControl
    {
        public static readonly DependencyProperty BlenderOutputProperty
            = DependencyProperty.Register("BlenderOutput", typeof(BitmapSource), typeof(FingerView));
        public BitmapSource BlenderOutput
        {
            get { return (BitmapSource)GetValue(BlenderOutputProperty); }
            set { SetValue(BlenderOutputProperty, value); }
        }

        public static readonly DependencyProperty OptionsProperty
            = DependencyProperty.Register("Options", typeof(Options), typeof(FingerView));
        public Options Options
        {
            get { return (Options)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        public static readonly DependencyProperty ExtractionDataProperty
            = DependencyProperty.Register("ExtractionData", typeof(ExtractionData), typeof(FingerView),
            new PropertyMetadata((self, args) => { (self as FingerView).UpdateSkeletonType(); }));
        public ExtractionData ExtractionData
        {
            get { return (ExtractionData)GetValue(ExtractionDataProperty); }
            set { SetValue(ExtractionDataProperty, value); }
        }

        public static readonly DependencyProperty MatchDataProperty
            = DependencyProperty.Register("MatchData", typeof(MatchData), typeof(FingerView));
        public MatchData MatchData
        {
            get { return (MatchData)GetValue(MatchDataProperty); }
            set { SetValue(MatchDataProperty, value); }
        }

        public static readonly DependencyProperty MatchSideProperty
            = DependencyProperty.Register("MatchSide", typeof(MatchSide), typeof(FingerView));
        public MatchSide MatchSide
        {
            get { return (MatchSide)GetValue(MatchSideProperty); }
            set { SetValue(MatchSideProperty, value); }
        }

        public static readonly DependencyProperty SkeletonChoiceProperty
            = DependencyProperty.Register("SkeletonChoice", typeof(SkeletonType), typeof(FingerView),
            new PropertyMetadata(SkeletonType.Ridges, (self, args) => { (self as FingerView).UpdateSkeletonType(); }));
        public SkeletonType SkeletonChoice
        {
            get { return (SkeletonType)GetValue(SkeletonChoiceProperty); }
            set { SetValue(SkeletonChoiceProperty, value); }
        }

        static readonly DependencyPropertyKey SkeletonDataProperty
            = DependencyProperty.RegisterReadOnly("SkeletonData", typeof(SkeletonData), typeof(FingerView), null);
        public SkeletonData SkeletonData
        {
            get { return (SkeletonData)GetValue(SkeletonDataProperty.DependencyProperty); }
        }

        void UpdateSkeletonType()
        {
            if (ExtractionData != null)
            {
                if (SkeletonChoice == SkeletonType.Ridges)
                    SetValue(SkeletonDataProperty, ExtractionData.Ridges);
                else
                    SetValue(SkeletonDataProperty, ExtractionData.Valleys);
            }
            else
                SetValue(SkeletonDataProperty, null);
        }

        public FingerView()
        {
            InitializeComponent();
        }
    }
}
