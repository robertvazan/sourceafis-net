using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FingerprintAnalyzer
{
    class MainWindow : Form
    {
        MenuStrip MainMenu;
        PictureBox WindowCanvas;

        public MainWindow()
        {
            MainMenu = new MenuStrip();
            WindowCanvas = new PictureBox();
            
            MainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)WindowCanvas).BeginInit();
            SuspendLayout();

            MainMenu.Dock = DockStyle.Top;
            MainMenu.Items.AddRange(BuildMenu());
            WindowCanvas.Dock = DockStyle.Fill;

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

        ToolStripItem[] BuildMenu()
        {
            return new ToolStripItem[] {
                CreateSubMenu("File", new ToolStripItem[] {
                    CreateMenuItem("Open Probe...", delegate() { /* ...TODO... */ }),
                    new ToolStripSeparator(),
                    CreateMenuItem("Exit", delegate() { Close(); })
                })
            };
        }
    }
}
