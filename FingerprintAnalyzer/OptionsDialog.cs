using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;
using SourceAFIS.General;

namespace FingerprintAnalyzer
{
    sealed class OptionsDialog : Form
    {
        public event Action OnChange;

        event Action ResumeLayoutEvent;
        event Action RefreshEvent;
        Options Options;
        Options Defaults;
        Options LastCommit;

        public OptionsDialog(Options options)
        {
            Defaults = options.DeepClone();
            PersistentStore.Load(options);
            Options = options;
            LastCommit = options.DeepClone();

            SuspendLayout();
            
            Text = "Options for SourceAFIS Fingerprint Analyzer";
            Size = new Size(300, 450);
            FormClosing += OnClose;
            Controls.Add(GenerateDialog(options));

            if (RefreshEvent != null)
                RefreshEvent();
            if (ResumeLayoutEvent != null)
                ResumeLayoutEvent();
            ResumeLayout(false);
            PersistentStore.Load(this);
        }

        void OnClose(object sender, EventArgs e)
        {
            Save();
        }
        
        public void Save()
        {
            PersistentStore.Save(Options);
            PersistentStore.Save(this);
        }

        void DoOk()
        {
            Options.DeepCopyTo(LastCommit);
            Hide();
        }

        void DoCancel()
        {
            LastCommit.DeepCopyTo(Options);
            if (RefreshEvent != null)
                RefreshEvent();
            Hide();
            if (OnChange != null)
                OnChange();
        }

        void DoDefaults()
        {
            Defaults.DeepCopyTo(Options);
            if (RefreshEvent != null)
                RefreshEvent();
            if (OnChange != null)
                OnChange();
        }

        TableLayoutPanel GetDefaultTable(int columns, int rows)
        {
            TableLayoutPanel table = new TableLayoutPanel();
            table.SuspendLayout();
            ResumeLayoutEvent += delegate() { table.ResumeLayout(false); };
            table.ColumnCount = columns;
            table.RowCount = rows;
            if (columns == 1)
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            else
            {
                for (int i = 0; i < columns; ++i)
                    table.ColumnStyles.Add(new ColumnStyle());
            }
            for (int i = 0; i < rows; ++i)
                table.RowStyles.Add(new RowStyle());
            return table;
        }

        Control GenerateDialog(Options options)
        {
            TableLayoutPanel table = GetDefaultTable(1, 2);
            table.Dock = DockStyle.Fill;
            table.RowStyles[0] = new RowStyle(SizeType.Percent, 100);

            TabControl mainTabControl = GenerateTabControl(options);
            mainTabControl.Dock = DockStyle.Fill;
            table.Controls.Add(mainTabControl);
            table.Controls.Add(GenerateButtonRow());

            return table;
        }

        Control GenerateButtonRow()
        {
            TableLayoutPanel table = GetDefaultTable(3, 1);
            table.Anchor = AnchorStyles.Right;
            table.AutoSize = true;

            Button ok = GenerateButton("OK", DoOk);
            Button cancel = GenerateButton("Cancel", DoCancel);
            table.Controls.Add(ok);
            table.Controls.Add(cancel);
            table.Controls.Add(GenerateButton("Defaults", DoDefaults));

            AcceptButton = ok;
            CancelButton = cancel;

            return table;
        }

        Button GenerateButton(string text, Action action)
        {
            Button button = new Button();
            button.Text = text;
            button.Click += (sender, e) => { action(); };
            return button;
        }

        Control GenerateStack(object root)
        {
            List<Control> result = new List<Control>();
            foreach (FieldInfo fieldInfo in root.GetType().GetFields())
            {
                if (fieldInfo.FieldType == typeof(bool))
                    result.Add(GenerateBool(root, fieldInfo));
                if (fieldInfo.FieldType.IsEnum)
                    result.Add(GenerateEnum(root, fieldInfo));
            }

            TabControl tabControl = GenerateTabControl(root);
            if (tabControl != null)
                result.Add(tabControl);

            return WrapStack(result);
        }

        Control WrapStack(IList<Control> controls)
        {
            TableLayoutPanel table = GetDefaultTable(1, controls.Count);
            table.Dock = DockStyle.Top;
            table.AutoSize = true;

            foreach (Control control in controls)
            {
                control.Dock = DockStyle.Top;
                table.Controls.Add(control);
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
                        ResumeLayoutEvent += delegate() { tabControl.ResumeLayout(false); };
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
            ResumeLayoutEvent += delegate()
            {
                page.ResumeLayout(false);
                page.PerformLayout();
            };
            page.Text = fieldInfo.Name;
            page.AutoScroll = true;
            page.Controls.Add(GenerateStack(fieldInfo.GetValue(root)));
            return page;
        }

        CheckBox GenerateBool(object root, FieldInfo fieldInfo)
        {
            CheckBox result = new CheckBox();
            result.Text = fieldInfo.Name;
            result.Click += delegate(object sender, EventArgs e)
            {
                fieldInfo.SetValue(root, result.Checked);
                if (OnChange != null)
                    OnChange();
            };
            RefreshEvent += delegate() { result.Checked = (bool)fieldInfo.GetValue(root); };
            return result;
        }

        Control GenerateEnum(object root, FieldInfo fieldInfo)
        {
            Label label = new Label();
            label.Text = fieldInfo.Name + ":";
            label.AutoSize = true;
            label.Anchor = AnchorStyles.Left;

            ComboBox combo = new ComboBox();
            combo.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (string name in Enum.GetNames(fieldInfo.FieldType))
                combo.Items.Add(name);
            combo.SelectedIndexChanged += delegate(object sender, EventArgs e)
            {
                if (combo.SelectedIndex >= 0)
                {
                    fieldInfo.SetValue(root, Enum.Parse(fieldInfo.FieldType, combo.Text));
                    if (OnChange != null)
                        OnChange();
                }
            };
            RefreshEvent += delegate() { combo.Text = Enum.GetName(fieldInfo.FieldType, fieldInfo.GetValue(root)); };

            TableLayoutPanel table = GetDefaultTable(2, 1);
            table.AutoSize = true;
            table.Dock = DockStyle.Top;
            table.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 100);
            table.Controls.Add(label);
            table.Controls.Add(combo);
            return table;
        }
    }
}
