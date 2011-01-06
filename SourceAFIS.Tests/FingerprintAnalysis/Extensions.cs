using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Automation;
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
        public static void SelectSlowly(this ComboBox combo, string text)
        {
            if (combo.GetSelectedItemText() != text)
            {
                combo.Click();
                Thread.Sleep(300);
                combo.Items.Where(item => item.Text == text).First().Click();
                Thread.Sleep(100);
                Assert.AreEqual(text, combo.SelectedItemText);
            }
        }

        public static string GetSelectedItemText(this ComboBox combo)
        {
            AutomationElement element = combo.AutomationElement;
            AutomationPattern automationPattern = element.GetSupportedPatterns().Where(
                p => p.ProgrammaticName == "SelectionPatternIdentifiers.Pattern").First();
            SelectionPattern selectionPattern = element.GetCurrentPattern(automationPattern) as SelectionPattern;
            return selectionPattern.Current.GetSelection()[0].Current.Name;
        }

        public static IUIItem GetChecked(this UIItemContainer container, SearchCriteria criteria)
        {
            IUIItem result = container.Get(criteria);
            Assert.IsNotNull(result);
            return result;
        }

        public static T GetChecked<T>(this UIItemContainer container, SearchCriteria criteria)
            where T : UIItem
        {
            T result = container.Get<T>(criteria);
            Assert.IsNotNull(result);
            return result;
        }

        public static Button GetPanelButton(this IUIItem panel, SearchCriteria criteria)
        {
            AutomationElement element = panel.GetElement(criteria);
            Assert.IsNotNull(element);
            return new Button(element, panel.ActionListener);
        }

        public static Label GetPanelLabel(this IUIItem panel, SearchCriteria criteria)
        {
            AutomationElement element = panel.GetElement(criteria);
            Assert.IsNotNull(element);
            return new Label(element, panel.ActionListener);
        }
    }
}
