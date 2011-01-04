using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Reflection;
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
        public string AppPath = Path.Combine("FingerprintAnalysis", "SourceAFIS.FingerprintAnalysis.exe");
        public Application App;
        public Window Win;

        public void StartApp()
        {
            if (!File.Exists(AppPath))
                AppPath = Path.Combine("..", "..", "..", "SourceAFIS.FingerprintAnalysis", "bin", "Debug", "SourceAFIS.FingerprintAnalysis.exe");
            App = Application.Launch(AppPath);
            Win = App.GetWindow("Fingerprint Analysis", InitializeOption.NoCache);
        }

        public WPFComboBox LayerChoice { get { return Win.GetChecked<WPFComboBox>(SearchCriteria.ByAutomationId("LayerChoice")); } }
        public WPFComboBox SkeletonChoice { get { return Win.GetChecked<WPFComboBox>(SearchCriteria.ByAutomationId("SkeletonChoice")); } }
        public WPFComboBox MaskChoice { get { return Win.GetChecked<WPFComboBox>(SearchCriteria.ByAutomationId("MaskChoice")); } }
        public CheckBox Contrast { get { return Win.GetChecked<CheckBox>(SearchCriteria.ByText("Contrast")); } }
        public CheckBox Orientation { get { return Win.GetChecked<CheckBox>(SearchCriteria.ByText("Orientation field")); } }
        public CheckBox Minutiae { get { return Win.GetChecked<CheckBox>(SearchCriteria.ByText("Minutiae")); } }
        public CheckBox Paired { get { return Win.GetChecked<CheckBox>(SearchCriteria.ByText("Paired minutiae")); } }

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

        public class MatchSide
        {
            public Common TestSuite;
            public readonly IUIItem FilePicker;

            public MatchSide(Common test, string side)
            {
                TestSuite = test;
                FilePicker = TestSuite.Win.GetChecked(SearchCriteria.ByAutomationId(side + "FilePicker"));
            }

            public Button Open { get { return FilePicker.GetPanelButton(SearchCriteria.ByText("Open...")); } }
            public Button Save { get { return FilePicker.GetPanelButton(SearchCriteria.ByText("Save...")); } }
            public Button Close { get { return FilePicker.GetPanelButton(SearchCriteria.ByText("Close")); } }
        }

        public MatchSide Left { get { return new MatchSide(this, "Left"); } }
        public MatchSide Right { get { return new MatchSide(this, "Right"); } }

        public class FileOpenDialog
        {
            public Window Dialog;

            public FileOpenDialog(Window parent)
            {
                List<Window> modals = parent.ModalWindows();
                Assert.AreEqual(1, modals.Count);
                Dialog = modals[0];
            }

            public ComboBox Path { get { return Dialog.GetChecked<ComboBox>(SearchCriteria.ByAutomationId("1148")); } }
            public Button Open { get { return Dialog.GetChecked<Button>(SearchCriteria.ByAutomationId("1")); } }
        }

        public FileOpenDialog OpenDialog { get { return new FileOpenDialog(Win); } }

        public void SelectFile(MatchSide side, string path)
        {
            if (path != null)
            {
                side.Open.Click();
                OpenDialog.Path.EditableText = Settings.SomeFingerprintPath;
                OpenDialog.Open.Click();
            }
            else
                side.Close.Click();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (App != null && !App.HasExited)
                App.Kill();
        }
    }
}
