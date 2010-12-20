using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using NUnit.Framework;
using White.Core;
using White.Core.Factory;
using White.Core.UIItems;
using White.Core.UIItems.WindowItems;
using White.Core.UIItems.ListBoxItems;
using White.Core.UIItems.Finders;

namespace SourceAFIS.Tests.FingerprintAnalysis
{
    public class Common
    {
        //protected string AppPath = Path.Combine("FingerprintAnalysis", "SourceAFIS.FingerprintAnalysis.exe");
        protected string AppPath = Path.Combine("..", "..", "..", "SourceAFIS.FingerprintAnalysis", "bin", "Debug", "SourceAFIS.FingerprintAnalysis.exe");
        protected Application App;
        protected Window Win;

        public void StartApp()
        {
            App = Application.Launch(AppPath);
            Win = App.GetWindow("Fingerprint Analysis", InitializeOption.NoCache);
        }

        public WPFComboBox LayerChoice { get { return Win.Get<WPFComboBox>(SearchCriteria.ByAutomationId("LayerChoice")); } }
        public WPFComboBox SkeletonChoice { get { return Win.Get<WPFComboBox>(SearchCriteria.ByAutomationId("SkeletonChoice")); } }
        public WPFComboBox MaskChoice { get { return Win.Get<WPFComboBox>(SearchCriteria.ByAutomationId("MaskChoice")); } }
        public CheckBox Contrast { get { return Win.Get<CheckBox>(SearchCriteria.ByText("Contrast")); } }
        public CheckBox Orientation { get { return Win.Get<CheckBox>(SearchCriteria.ByText("Orientation field")); } }
        public CheckBox Minutiae { get { return Win.Get<CheckBox>(SearchCriteria.ByText("Minutiae")); } }
        public CheckBox Paired { get { return Win.Get<CheckBox>(SearchCriteria.ByText("Paired minutiae")); } }

        public void ResetOptions()
        {
            LayerChoice.SelectSlowly("OriginalImage");
            SkeletonChoice.SelectSlowly("Ridges");
            MaskChoice.SelectSlowly("None");
            Contrast.Checked = false;
            Orientation.Checked = false;
            Minutiae.Checked = false;
            Paired.Checked = false;
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            if (App != null && !App.HasExited)
                App.Kill();
        }
    }
}
