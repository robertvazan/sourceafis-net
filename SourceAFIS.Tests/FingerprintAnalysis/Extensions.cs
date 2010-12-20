using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public static class Extensions
    {
        public static void SelectSlowly(this WPFComboBox combo, string text)
        {
            combo.Click();
            Thread.Sleep(300);
            combo.Items.Where(item => item.Text == text).First().Click();
            Thread.Sleep(100);
            Assert.AreEqual(text, combo.SelectedItemText);
        }
    }
}
