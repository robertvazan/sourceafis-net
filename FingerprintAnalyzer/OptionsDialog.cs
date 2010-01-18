using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;

namespace FingerprintAnalyzer
{
    sealed class OptionsDialog : Form
    {
        public delegate void VoidFunction();

        public VoidFunction OnChange;

        List<VoidFunction> ResumeLayoutQueue = new List<VoidFunction>();

        public OptionsDialog(object optionsRoot)
        {
            SuspendLayout();
            
            Text = "Options for SourceAFIS Fingerprint Analyzer";
            Controls.Add(GenerateDialog(optionsRoot));

            foreach (VoidFunction resume in ResumeLayoutQueue)
                resume();
            ResumeLayout(false);
        }

        Control GenerateDialog(object optionsRoot)
        {
            TableLayoutPanel table = new TableLayoutPanel();
            table.SuspendLayout();
            ResumeLayoutQueue.Add(delegate() { table.ResumeLayout(false); });
            table.Dock = DockStyle.Fill;
            table.ColumnCount = 1;
            table.RowCount = 2;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            table.RowStyles.Add(new RowStyle());

            TabControl mainTabControl = GenerateTabControl(optionsRoot);
            mainTabControl.Dock = DockStyle.Fill;
            table.Controls.Add(mainTabControl);
            table.Controls.Add(GenerateButtonRow());

            return table;
        }

        Control GenerateButtonRow()
        {
            TableLayoutPanel table = new TableLayoutPanel();
            table.SuspendLayout();
            ResumeLayoutQueue.Add(delegate() { table.ResumeLayout(false); });
            table.Anchor = AnchorStyles.Right;
            table.AutoSize = true;
            table.RowCount = 1;
            table.RowStyles.Add(new RowStyle());
            table.ColumnCount = 3;
            table.ColumnStyles.Add(new ColumnStyle());
            table.ColumnStyles.Add(new ColumnStyle());
            table.ColumnStyles.Add(new ColumnStyle());

            table.Controls.Add(GenerateButton("OK", delegate() { }));
            table.Controls.Add(GenerateButton("Cancel", delegate() { }));
            table.Controls.Add(GenerateButton("Defaults", delegate() { }));

            return table;
        }

        Control GenerateButton(string text, VoidFunction action)
        {
            Button button = new Button();
            button.Text = text;
            button.Click += delegate(object sender, EventArgs e) { action(); };
            return button;
        }

        Control GenerateStack(object root)
        {
            List<Control> result = new List<Control>();
            foreach (FieldInfo fieldInfo in root.GetType().GetFields())
            {
                if (fieldInfo.FieldType == typeof(bool))
                    result.Add(GenerateBool(root, fieldInfo));
            }

            TabControl tabControl = GenerateTabControl(root);
            if (tabControl != null)
                result.Add(tabControl);

            return WrapStack(result);
        }

        Control WrapStack(IList<Control> controls)
        {
            TableLayoutPanel table = new TableLayoutPanel();
            table.SuspendLayout();
            ResumeLayoutQueue.Add(delegate() { table.ResumeLayout(false); });
            table.Dock = DockStyle.Top;
            table.AutoSize = true;
            table.ColumnCount = 1;
            table.RowCount = controls.Count;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            foreach (Control control in controls)
            {
                control.Dock = DockStyle.Top;
                table.Controls.Add(control);
                table.RowStyles.Add(new RowStyle());
            }

            return table;
        }

        TabControl GenerateTabControl(object root)
        {
            TabControl tabControl = null;
            foreach (FieldInfo fieldInfo in root.GetType().GetFields())
            {
                if (fieldInfo.FieldType.IsClass)
                {
                    if (tabControl == null)
                    {
                        tabControl = new TabControl();
                        tabControl.SuspendLayout();
                        ResumeLayoutQueue.Add(delegate() { tabControl.ResumeLayout(false); });
                        tabControl.Height = 200;
                    }
                    TabPage page = GenerateTabPage(root, fieldInfo);
                    tabControl.TabPages.Add(page);
                }
            }
            return tabControl;
        }

        TabPage GenerateTabPage(object root, FieldInfo fieldInfo)
        {
            TabPage page = new TabPage();
            page.SuspendLayout();
            ResumeLayoutQueue.Add(delegate()
            {
                page.ResumeLayout(false);
                page.PerformLayout();
            });
            page.Text = fieldInfo.Name;
            page.BackColor = SystemColors.ButtonFace;
            page.AutoScroll = true;
            page.Controls.Add(GenerateStack(fieldInfo.GetValue(root)));
            return page;
        }

        CheckBox GenerateBool(object root, FieldInfo fieldInfo)
        {
            CheckBox result = new CheckBox();
            result.Text = fieldInfo.Name;
            result.Checked = (bool)fieldInfo.GetValue(root);
            result.Click += delegate(object sender, EventArgs e)
            {
                fieldInfo.SetValue(root, result.Checked);
                OnChange();
            };
            return result;
        }
    }
}
