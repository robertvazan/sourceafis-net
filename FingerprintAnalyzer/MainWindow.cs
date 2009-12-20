using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using SourceAFIS.Visualization;

namespace FingerprintAnalyzer
{
    sealed class MainWindow : Form
    {
        LogCollector Logs = new LogCollector();
        Blender Blender = new Blender();

        MenuStrip MainMenu;
        PictureBox WindowCanvas;

        public MainWindow()
        {
            Blender.Logs = Logs;
            Blender.Probe.OriginalImage = true;
            InitializeLayout();
        }

        void InitializeLayout()
        {
            MainMenu = new MenuStrip();
            WindowCanvas = new PictureBox();

            MainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)WindowCanvas).BeginInit();
            SuspendLayout();

            MainMenu.Dock = DockStyle.Top;
            MainMenu.Items.AddRange(BuildMenu());
            
            WindowCanvas.Dock = DockStyle.Fill;
            WindowCanvas.SizeMode = PictureBoxSizeMode.Zoom;

            ClientSize = new System.Drawing.Size(800, 600);
            Name = "FingerprintAnalyzerWindow";
            Text = "SourceAFIS Fingerprint Analyzer";
            Controls.Add(WindowCanvas);
            Controls.Add(MainMenu);
            MainMenuStrip = MainMenu;

            MainMenu.ResumeLayout(false);
            MainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)WindowCanvas).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        ToolStripMenuItem CreateSubMenu(string text, ToolStripItem[] items)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem();
            menu.DropDownItems.AddRange(items);
            menu.Text = text;
            return menu;
        }

        delegate void VoidFunction();

        ToolStripMenuItem CreateMenuItem(string text, VoidFunction action)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem();
            menu.Text = text;
            menu.Click += delegate(object sender, EventArgs e) { action(); };
            return menu;
        }

        bool SwitchFlag(ref bool flag)
        {
            flag = !flag;
            return flag;
        }

        delegate bool BoolFunction();

        ToolStripMenuItem CreateCheckMenu(string text, BoolFunction switcher)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem();
            menu.Text = text;
            menu.Click += delegate(object sender, EventArgs e)
            {
                menu.Checked = switcher();
                RefreshCanvas();
            };
            switcher();
            menu.Checked = switcher();
            return menu;
        }

        ToolStripItem[] BuildMenu()
        {
            return new ToolStripItem[] {
                CreateSubMenu("File", new ToolStripItem[] {
                    CreateMenuItem("Open Probe...", OpenProbe),
                    new ToolStripSeparator(),
                    CreateMenuItem("Exit", delegate() { Close(); })
                }),
                CreateSubMenu("Probe", new ToolStripItem[] {
                    CreateCheckMenu("Original Image", delegate() { return SwitchFlag(ref Blender.Probe.OriginalImage); }),
                    CreateCheckMenu("Equalized", delegate() { return SwitchFlag(ref Blender.Probe.Equalized); }),
                    CreateCheckMenu("Smoothed", delegate() { return SwitchFlag(ref Blender.Probe.SmoothedRidges); }),
                    new ToolStripSeparator(),
                    CreateCheckMenu("Contrast", delegate() { return SwitchFlag(ref Blender.Probe.Contrast); }),
                    CreateCheckMenu("Absolute Contrast", delegate() { return SwitchFlag(ref Blender.Probe.AbsoluteContrast); }),
                    CreateCheckMenu("Relative Contrast", delegate() { return SwitchFlag(ref Blender.Probe.RelativeContrast); }),
                    CreateCheckMenu("Majority Filter", delegate() { return SwitchFlag(ref Blender.Probe.LowContrastMajority); }),
                    CreateCheckMenu("Mask", delegate() { return SwitchFlag(ref Blender.Probe.SegmentationMask); }),
                    new ToolStripSeparator(),
                    CreateCheckMenu("Orientation", delegate() { return SwitchFlag(ref Blender.Probe.Orientation); }),
                    CreateCheckMenu("Orthogonal Smoothing", delegate() { return SwitchFlag(ref Blender.Probe.OrthogonalSmoothing); }),
                    new ToolStripSeparator(),
                    CreateCheckMenu("Binarized", delegate() { return SwitchFlag(ref Blender.Probe.Binarized); })
                })
            };
        }

        void RefreshCanvas()
        {
            if (Logs.Probe.InputImage != null)
            {
                Logs.Collect();
                Blender.Blend();
                WindowCanvas.Image = Blender.OutputImage;
            }
            else
                WindowCanvas.Image = null;
        }

        void OpenProbe()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Logs.Probe.InputImage = PixelFormat.ToByte(ImageIO.Load(dialog.FileName));
                RefreshCanvas();
            }
        }
    }
}
