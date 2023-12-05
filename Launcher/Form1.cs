using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Launcher {
    public partial class Form1 : Form {

        private string recode = Path.Combine(Environment.CurrentDirectory, "files\\Recode-b120423.jar");
        private string old = Path.Combine(Environment.CurrentDirectory, "files\\Old-0.0.5.jar");

        // for rounded corners
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr round
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        private bool isDragging = false;
        private Point offset;

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(round(0, 0, Width, Height, 20, 20));
            launchButton.Region = Region.FromHrgn(round(0, 0, launchButton.Width, launchButton.Height, 8, 8));
            SetupUtil.CreateFolder("files");
        }

        private void button1_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e) {
            WindowState = FormWindowState.Minimized;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                isDragging = true;
                offset = new Point(e.X, e.Y);
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e) {
            if (isDragging) {
                Point newLocation = this.PointToScreen(new Point(e.X, e.Y));
                newLocation.Offset(-offset.X, -offset.Y);
                this.Location = newLocation;
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                isDragging = false;
            }
        }

        private void launchButton_Click(object sender, EventArgs e) {
            if (launchButton.Text.Equals("Install")) {

                if(comboBox1.SelectedItem.Equals("Recode-b120423")) {
                    JarDownloader.downloadJar(progressBar1, "https://github.com/mark-fy/db/raw/main/Recode-b120423.jar", Path.Combine(Environment.CurrentDirectory, "files\\Recode-b120423.jar"));
                    if (progressBar1.Value.Equals(100)) {
                        launchButton.Text = "Launch";
                    }
                } else {
                    /* currently not available to download
                    JarDownloader.downloadJar(progressBar1, "https://github.com/mark-fy/db/raw/main/Recode-b120423.jar", Path.Combine(Environment.CurrentDirectory, "files\\Recode-b120423.jar"));
                    if (progressBar1.Value.Equals(100)) {
                        launchButton.Text = "Launch";
                    }
                    */
                }
            } else {
                string java = Path.Combine(Environment.CurrentDirectory, "files\\azul-1.8.9\\bin\\java.exe");

                string libraries = Path.Combine(Environment.CurrentDirectory, "files\\libraries");
                string natives = Path.Combine(Environment.CurrentDirectory, "files\\natives");
                string mainFolder = Path.Combine(Environment.CurrentDirectory, ".minecraft");

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = java;
                startInfo.WorkingDirectory = mainFolder;
                startInfo.Arguments = $"-Xms1000M -Xmx8000M -Djava.library.path=\"" + natives + "\" -cp \"" + recode + ";" + libraries + "\" net.minecraft.client.main.Main -uuid fc5bc365-aedf-30a8-8b89-04e462e29bde -username Steve -accessToken yes -version 1 --assetIndex 1.8";
                try {
                    Process.Start(startInfo);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if (comboBox1.SelectedItem.Equals("Recode-b120423") && !FileChecker.checkForFile(recode)) {
                launchButton.Text = "Install";
            } else if(comboBox1.SelectedItem.Equals("Old-0.0.5") && FileChecker.checkForFile(old)) {
                launchButton.Text = "Install";
            }
        }
    }
}
