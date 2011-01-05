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

        public ComboBox LayerChoice { get { return Win.GetChecked<ComboBox>(SearchCriteria.ByAutomationId("LayerChoice")); } }
        public ComboBox SkeletonChoice { get { return Win.GetChecked<ComboBox>(SearchCriteria.ByAutomationId("SkeletonChoice")); } }
        public ComboBox MaskChoice { get { return Win.GetChecked<ComboBox>(SearchCriteria.ByAutomationId("MaskChoice")); } }
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
                if (modals.Count == 0)
                {
                    Thread.Sleep(500);
                    modals = parent.ModalWindows();
                }
                Assert.AreEqual(1, modals.Count);
                Dialog = modals[0];
            }

            public ComboBox Path { get { return Dialog.GetChecked<ComboBox>(SearchCriteria.ByAutomationId("1148")); } }
            public Button Open { get { return Dialog.GetChecked<Button>(SearchCriteria.ByAutomationId("1")); } }
        }

        public FileOpenDialog OpenDialog { get { return new FileOpenDialog(Win); } }

        public class FileSaveDialog
        {
            public Window Dialog;

            public FileSaveDialog(Window parent)
            {
                List<Window> modals = parent.ModalWindows();
                if (modals.Count == 0)
                {
                    Thread.Sleep(500);
                    modals = parent.ModalWindows();
                }
                Assert.AreEqual(1, modals.Count);
                Dialog = modals[0];
            }

            public TextBox Path { get { return Dialog.GetChecked<TextBox>(SearchCriteria.ByAutomationId("1001")); } }
            public Button Save { get { return Dialog.GetChecked<Button>(SearchCriteria.ByAutomationId("1")); } }
        }

        public FileSaveDialog SaveDialog { get { return new FileSaveDialog(Win); } }

        public void SelectFile(MatchSide side, string path)
        {
            if (path != null)
            {
                side.Open.Click();
                OpenDialog.Path.EditableText = Settings.SomeFingerprintPath;
                OpenDialog.Open.Click();
                Assert.AreEqual(0, Win.ModalWindows().Count);
            }
            else
                side.Close.Click();
        }

        public void SelectFiles()
        {
            SelectFile(Left, Settings.SomeFingerprintPath);
            SelectFile(Right, Settings.MatchingFingerprintPath);
        }

        public void SaveFile(MatchSide side)
        {
            File.Delete(Settings.SavedImagePath);
            side.Save.Click();
            SaveDialog.Path.Text = Settings.SavedImagePath;
            SaveDialog.Save.Click();
            Assert.AreEqual(0, Win.ModalWindows().Count);
        }

        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
            StartApp();
        }

        [TearDown]
        public virtual void TearDown()
        {
            Thread.Sleep(300);
            Assert.AreEqual(0, Win.ModalWindows().Count);
            Assert.IsFalse(App.HasExited);
        }

        [TestFixtureTearDown]
        public virtual void TestFixtureTearDown()
        {
            if (App != null && !App.HasExited)
                App.Kill();
        }
    }
}
