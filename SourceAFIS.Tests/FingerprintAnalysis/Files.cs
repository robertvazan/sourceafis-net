using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using NUnit.Framework;
using White.Core.UIItems;
using White.Core.UIItems.Finders;

namespace SourceAFIS.Tests.FingerprintAnalysis
{
    [TestFixture]
    public class Files : Common
    {
        [Test]
        public void ClickThrough()
        {
            ResetOptions();
            Orientation.Checked = true;

            CloseFiles();
            SelectFiles();

            SaveFile(Left);
            SaveFile(Right);

            CloseFiles();
        }

        [Test]
        public void OpenCancel()
        {
            CloseFiles();

            Left.Open.Click();
            OpenDialog.Path.EditableText = Settings.SomeFingerprintPath;
            OpenDialog.Cancel.Click();

            Assert.AreEqual("", Left.FileName.Text);
        }

        [Test]
        public void SaveCancel()
        {
            ResetOptions();
            Orientation.Checked = true;

            SelectFiles();

            string path = Path.GetFullPath("saved.png");
            File.Delete(path);

            Left.Save.Click();
            SaveDialog.Path.Text = path;
            SaveDialog.Cancel.Click();

            Assert.IsFalse(File.Exists(path));
        }

        [Test]
        public void OpenInvalid()
        {
            CloseFiles();

            Left.Open.Click();
            FileOpenDialog dialog = OpenDialog;

            dialog.Path.EditableText = Path.GetFullPath("nonexistent.tif");
            Assert.AreEqual(0, dialog.Dialog.ModalWindows().Count);

            dialog.Open.Click();
            Thread.Sleep(300);
            Assert.AreEqual(2, Win.ModalWindows().Count);
            Assert.AreEqual(1, dialog.Dialog.ModalWindows().Count);

            dialog.Dialog.ModalWindows()[0].Get<Button>(SearchCriteria.ByText("OK")).Click();
            Assert.AreEqual(0, dialog.Dialog.ModalWindows().Count);

            dialog.Cancel.Click();
            Assert.AreEqual("", Left.FileName.Text);
        }

        [Test]
        public void SaveExisting()
        {
            SelectFiles();
            File.WriteAllBytes(Settings.SavedImagePath, new byte[0]);

            Left.Save.Click();
            FileSaveDialog dialog = SaveDialog;

            dialog.Path.Text = Settings.SavedImagePath;
            Assert.AreEqual(0, dialog.Dialog.ModalWindows().Count);

            dialog.Save.Click();
            Thread.Sleep(300);
            Assert.AreEqual(2, Win.ModalWindows().Count);
            Assert.AreEqual(1, dialog.Dialog.ModalWindows().Count);

            dialog.Dialog.ModalWindows()[0].Get<Button>(SearchCriteria.ByAutomationId("7")).Click();
            Assert.AreEqual(0, dialog.Dialog.ModalWindows().Count);

            dialog.Cancel.Click();
            Assert.AreEqual(0, File.ReadAllBytes(Settings.SavedImagePath).Length);
        }
    }
}
