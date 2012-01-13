using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NUnit.Framework;
using White.Core;
using White.Core.Factory;
using White.Core.UIItems;
using White.Core.UIItems.WindowItems;
using White.Core.UIItems.ListBoxItems;
using White.Core.UIItems.Finders;
using SourceAFIS.General;

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
            Thread.Sleep(1000);
            Win = App.GetWindow("Fingerprint Analysis", InitializeOption.NoCache);
        }

        public void CloseApp()
        {
            Assert.IsNotNull(App);
            Assert.IsNotNull(Win);

            Win.Close();
            Win = null;

            Wait(() => App.HasExited);
            App = null;
        }

        public ComboBox BitmapLayerChoice { get { return Win.GetPatient<ComboBox>(SearchCriteria.ByAutomationId("BitmapLayerChoice")); } }
        public ComboBox MarkerLayerChoice { get { return Win.GetPatient<ComboBox>(SearchCriteria.ByAutomationId("MarkerLayerChoice")); } }
        public ComboBox SkeletonChoice { get { return Win.GetPatient<ComboBox>(SearchCriteria.ByAutomationId("SkeletonChoice")); } }
        public ComboBox MaskChoice { get { return Win.GetPatient<ComboBox>(SearchCriteria.ByAutomationId("MaskChoice")); } }
        public CheckBox Contrast { get { return Win.GetPatient<CheckBox>(SearchCriteria.ByText("Contrast")); } }
        public CheckBox Orientation { get { return Win.GetPatient<CheckBox>(SearchCriteria.ByText("Orientation field")); } }
        public CheckBox Paired { get { return Win.GetPatient<CheckBox>(SearchCriteria.ByText("Paired minutiae")); } }

        public static string[] OptionNames = new string[]
        {
            "BitmapLayerChoice",
            "MarkerLayerChoice",
            "SkeletonChoice",
            "MaskChoice",
            "Contrast",
            "Orientation",
            "Paired"
        };

        public UIItem GetOptionControl(string name)
        {
            PropertyInfo property = this.GetType().GetProperty(name);
            return property.GetValue(this, null) as UIItem;
        }

        public void SetOptions(Dictionary<string, string> options)
        {
            foreach (string name in OptionNames)
            {
                object control = GetOptionControl(name);
                if (control is ComboBox)
                    (control as ComboBox).SelectSlowly(options[name]);
                else if (control is CheckBox)
                    (control as CheckBox).Checked = Convert.ToBoolean(options[name]);
            }
        }

        public Dictionary<string, string> GetOptions()
        {
            Dictionary<string, string> options = new Dictionary<string,string>();
            foreach (string name in OptionNames)
            {
                object control = GetOptionControl(name);
                if (control is ComboBox)
                    options.Add(name, (control as ComboBox).GetSelectedItemText());
                else if (control is CheckBox)
                    options.Add(name, (control as CheckBox).Checked.ToString());
            }
            return options;
        }

        public Dictionary<string, string> GetResetOptions()
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            options["BitmapLayerChoice"] = "OriginalImage";
            options["MarkerLayerChoice"] = "UniqueMinutiaSorter";
            options["SkeletonChoice"] = "Ridges";
            options["MaskChoice"] = "None";
            options["Contrast"] = Boolean.FalseString;
            options["Orientation"] = Boolean.FalseString;
            options["Paired"] = Boolean.FalseString;
            return options;
        }

        public void ResetOptions()
        {
            SetOptions(GetResetOptions());
        }

        public Dictionary<string, string> GetFullOptions()
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            options["BitmapLayerChoice"] = "SmoothedRidges";
            options["MarkerLayerChoice"] = "RidgeTracer";
            options["SkeletonChoice"] = "Valleys";
            options["MaskChoice"] = "Segmentation";
            options["Contrast"] = Boolean.TrueString;
            options["Orientation"] = Boolean.TrueString;
            options["Paired"] = Boolean.TrueString;
            return options;
        }

        public void FullOptions()
        {
            SetOptions(GetFullOptions());
        }

        public Dictionary<string, string> GetSingleOption(string name, string value)
        {
            Dictionary<string, string> options = GetResetOptions();
            options[name] = value;
            return options;
        }

        public class MatchSide
        {
            public Common TestSuite;
            public string Side;

            public bool LastFileValid;
            public string LastFile;

            public MatchSide(Common test, string side)
            {
                Side = side;
                TestSuite = test;
            }

            public IUIItem FilePicker { get { return TestSuite.Win.GetChecked(SearchCriteria.ByAutomationId(Side + "FilePicker")); } }

            public Button Open { get { return FilePicker.GetPanelButton(SearchCriteria.ByText("Open...")); } }
            public Button Save { get { return FilePicker.GetPanelButton(SearchCriteria.ByText("Save...")); } }
            public Button Close { get { return FilePicker.GetPanelButton(SearchCriteria.ByText("Close")); } }
            public Label FileName { get { return FilePicker.GetPanelLabel(SearchCriteria.ByAutomationId("ShortFileName")); } }
        }

        MatchSide LeftSide;
        MatchSide RightSide;
        public MatchSide Left { get { return LeftSide ?? (LeftSide = new MatchSide(this, "Left")); } }
        public MatchSide Right { get { return RightSide ?? (RightSide = new MatchSide(this, "Right")); } }

        public class FileOpenDialog
        {
            public Window Dialog;

            public FileOpenDialog(Window parent)
            {
                Wait(() => parent.ModalWindows().Count > 0);
                List<Window> modals = parent.ModalWindows();
                Assert.AreEqual(1, modals.Count);
                Dialog = modals[0];
            }

            public ComboBox Path { get { return Dialog.GetPatient<ComboBox>(SearchCriteria.ByAutomationId("1148")); } }
            public Button Open { get { return Dialog.GetPatient<Button>(SearchCriteria.ByAutomationId("1")); } }
            public Button Cancel { get { return Dialog.GetPatient<Button>(SearchCriteria.ByAutomationId("2")); } }
        }

        public FileOpenDialog OpenDialog { get { return new FileOpenDialog(Win); } }

        public class FileSaveDialog
        {
            public Window Dialog;

            public FileSaveDialog(Window parent)
            {
                Wait(() => parent.ModalWindows().Count > 0);
                List<Window> modals = parent.ModalWindows();
                Assert.AreEqual(1, modals.Count);
                Dialog = modals[0];
            }

            public TextBox Path { get { return Dialog.GetPatient<TextBox>(SearchCriteria.ByAutomationId("1001")); } }
            public Button Save { get { return Dialog.GetPatient<Button>(SearchCriteria.ByAutomationId("1")); } }
            public Button Cancel { get { return Dialog.GetPatient<Button>(SearchCriteria.ByAutomationId("2")); } }
        }

        public FileSaveDialog SaveDialog { get { return new FileSaveDialog(Win); } }

        public void SelectFile(MatchSide side, string path)
        {
            if (side.LastFileValid && side.LastFile == path)
                return;
            if (path != null)
            {
                side.Open.Click();
                OpenDialog.Dialog.PasteText(path);
                OpenDialog.Open.Click();
                Assert.AreEqual(0, Win.ModalWindows().Count);
            }
            else
                side.Close.Click();
            side.LastFile = path;
            side.LastFileValid = true;
        }

        public void SelectFiles()
        {
            SelectFile(Left, Settings.SomeFingerprintPath);
            SelectFile(Right, Settings.MatchingFingerprintPath);
        }

        public void CloseFiles()
        {
            SelectFile(Left, null);
            SelectFile(Right, null);
        }

        public void GenerateSavedFileName()
        {
            Settings.LastSavedImage = String.Format(Settings.SavedImagePath, ++Settings.SavedImageCounter);
            Directory.CreateDirectory(Path.GetDirectoryName(Settings.LastSavedImage));
            File.Delete(Settings.LastSavedImage);
        }

        public void SaveFile(MatchSide side)
        {
            GenerateSavedFileName();
            side.Save.Click();
            SaveDialog.Dialog.PasteText(Settings.LastSavedImage);
            SaveDialog.Save.Click();
            Assert.AreEqual(0, Win.ModalWindows().Count);
        }

        public BitmapSource CaptureImage(MatchSide side)
        {
            SaveFile(side);
            return WpfIO.Load(Settings.LastSavedImage);
        }

        public int ChecksumImage(MatchSide side)
        {
            BitmapSource image = CaptureImage(side);

            FormatConvertedBitmap converted = new FormatConvertedBitmap(image, PixelFormats.Bgra32, null, 0.5);

            int width = (int)converted.PixelWidth;
            int height = (int)converted.PixelHeight;

            byte[] flat = new byte[4 * width * height];

            converted.CopyPixels(flat, 4 * width, 0);

            int hash = unchecked((int)2166136261);
            for (int i = 0; i < flat.Length; ++i)
                hash = (hash ^ flat[i]) * 16777619;
            return hash;
        }

        public static void Wait(Func<bool> condition) { Wait(5000, condition); }

        public static void Wait(int millis, Func<bool> condition)
        {
            WeakWait(millis, condition);
            Assert.IsTrue(condition());
        }

        public static void WeakWait(Func<bool> condition) { WeakWait(5000, condition); }

        public static void WeakWait(int millis, Func<bool> condition)
        {
            int sofar = 0;
            do
            {
                if (condition())
                    return;
                Thread.Sleep(50);
                sofar += 50;
            } while (sofar < millis);
        }

        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
            StartApp();
        }

        [TearDown]
        public virtual void TearDown()
        {
            Wait(() => Win.ModalWindows().Count == 0);
            Assert.IsFalse(App.HasExited);
        }

        [TestFixtureTearDown]
        public virtual void TestFixtureTearDown()
        {
            if (App != null && !App.HasExited)
            {
                if (Win != null)
                {
                    Win.Close();
                    WeakWait(() => App.HasExited);
                }
                if (!App.HasExited)
                    App.Kill();
            }
        }
    }
}
