using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using SourceAFIS.General;
using SourceAFIS.Visualization;

namespace FingerprintAnalyzer
{
    sealed class MainWindow : Form
    {
        LogCollector Logs = new LogCollector();
        Blender Blender = new Blender();
        OptionsDialog OptionsDialog;
        string ProbePath;
        string CandidatePath;

        MenuStrip MainMenu;
        PictureBox WindowCanvas;

        public MainWindow()
        {
            Blender.Logs = Logs;
            OptionsDialog = new OptionsDialog(Blender.Options);
            OptionsDialog.Owner = this;
            OptionsDialog.OnChange += RefreshCanvas;
            InitializeLayout();

            PersistentStore.Load("ProbePath", ref ProbePath);
            PersistentStore.Load("CandidatePath", ref CandidatePath);
            RefreshCanvas();
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
            PersistentStore.Load(this);
            FormClosing += OnClose;

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

        ToolStripMenuItem CreateMenuItem(string text, Action action)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem();
            menu.Text = text;
            menu.Click += (sender, e) => { action(); };
            return menu;
        }

        ToolStripItem[] BuildMenu()
        {
            return new ToolStripItem[] {
                CreateSubMenu("File", new ToolStripItem[] {
                    CreateMenuItem("Open Probe...", OpenProbe),
                    CreateMenuItem("Open Candidate...", OpenCandidate),
                    new ToolStripSeparator(),
                    CreateMenuItem("Options...", delegate() { OptionsDialog.Show(); OptionsDialog.Focus(); }),
                    new ToolStripSeparator(),
                    CreateMenuItem("Exit", Close)
                })
            };
        }

        void RefreshCanvas()
        {
            if (ProbePath != null)
                Logs.Probe.InputImage = PixelFormat.ToByte(ImageIO.Load(ProbePath));
            else
                Logs.Probe.InputImage = null;
            if (CandidatePath != null)
                Logs.Candidate.InputImage = PixelFormat.ToByte(ImageIO.Load(CandidatePath));
            else
                Logs.Candidate.InputImage = null;

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
                ProbePath = dialog.FileName;
                RefreshCanvas();
            }
        }

        void OpenCandidate()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CandidatePath = dialog.FileName;
                RefreshCanvas();
            }
        }

        void OnClose(object sender, EventArgs e)
        {
            OptionsDialog.Save();
            PersistentStore.Save(this);
            PersistentStore.Save("ProbePath", ProbePath);
            PersistentStore.Save("CandidatePath", CandidatePath);
        }
    }
}
