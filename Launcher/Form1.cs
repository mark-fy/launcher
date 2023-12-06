using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
        }

        private string recode = Path.Combine(Environment.CurrentDirectory, "files\\Recode-b120423.jar");
        private string natives = Path.Combine(Environment.CurrentDirectory, "files\\natives");
        private string libraries = Path.Combine(Environment.CurrentDirectory, "files\\libraries");
        private string javaInstall = Path.Combine(Environment.CurrentDirectory, "files\\azul-1.8.9");

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr round
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        private bool isDragging = false;
        private Point offset;

        private async void Form1_Load(object sender, EventArgs e) {
            this.FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(round(0, 0, Width, Height, 20, 20));
            launchButton.Region = Region.FromHrgn(round(0, 0, launchButton.Width, launchButton.Height, 4, 4));
            progressBar1.Region = Region.FromHrgn(round(0, 0, progressBar1.Width, progressBar1.Height, 4, 4));
            SetupUtil.createFolder("files");
            SetupUtil.createFolder(".minecraft");
            if(!FileChecker.checkForDirectory(natives) || FileChecker.IsDirectoryEmpty(natives)) {

            }
            launchButton.Enabled = false;

            if (!FileChecker.checkForDirectory(natives) || FileChecker.IsDirectoryEmpty(natives)) {
                
                Dictionary<string, string> downloadUrlsAndPathsNatives = new Dictionary<string, string>{
            { "https://github.com/mark-fy/db/raw/main/natives.zip", Path.Combine(Environment.CurrentDirectory, "files", "natives.zip") }};
                await Downloader.DownloadFilesSequentially(progressBar1, label3, downloadUrlsAndPathsNatives);
                label3.Text = "Idle";
            }

            if (!FileChecker.checkForDirectory(libraries) || FileChecker.IsDirectoryEmpty(libraries)) {
                Dictionary<string, string> downloadUrlsAndPathsLibraries = new Dictionary<string, string>{
                { "https://github.com/mark-fy/db/raw/main/libraries.zip", Path.Combine(Environment.CurrentDirectory, "files", "libraries.zip") }};
                await Downloader.DownloadFilesSequentially(progressBar1, label3, downloadUrlsAndPathsLibraries);
                label3.Text = "Idle";
            }

            if (!FileChecker.checkForDirectory(javaInstall) || FileChecker.IsDirectoryEmpty(javaInstall)) {
                Dictionary<string, string> downloadUrlsAndPathsLibraries = new Dictionary<string, string>{
                { "https://github.com/mark-fy/db/raw/main/azul-1.8.9.zip", Path.Combine(Environment.CurrentDirectory, "files", "azul-1.8.9.zip") }};
                await Downloader.DownloadFilesSequentially(progressBar1, label3, downloadUrlsAndPathsLibraries);
                label3.Text = "Idle";
            }

            launchButton.Enabled = true;

            if (!FileChecker.checkForFile(recode)) {
                launchButton.Text = "Install";
            }
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

        private async void launchButton_Click(object sender, EventArgs e) {
            if (launchButton.Text.Equals("Install")) {
                if (comboBox1.SelectedItem.Equals("Recode-b120423")) {
                    Downloader.downloadFile(progressBar1, "https://github.com/mark-fy/db/raw/main/Recode-b120423.jar", recode);
                    label3.Text = "Downloading: Recode-b120423.jar";
                    timer1.Enabled = true;
                }
            } else {
                string java = Path.Combine(Environment.CurrentDirectory, "files\\azul-1.8.9\\bin\\java.exe");

                string mainFolder = Path.Combine(Environment.CurrentDirectory, ".minecraft");

                try {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = java;
                    startInfo.WorkingDirectory = mainFolder;
                    startInfo.Arguments = $"-Xms1000M -Xmx8000M -Djava.library.path=\"{natives}\" -cp \"{recode};{libraries}\\*\" net.minecraft.client.main.Main -uuid fc5bc365-aedf-30a8-8b89-04e462e29bde -username Steve -accessToken yes -version 1 --assetIndex 1.8";

                    using (Process process = new Process()) {
                        process.StartInfo = startInfo;
                        process.Start();

                        launchButton.Enabled = false;
                        await Task.Run(() => process.WaitForExit());
                        launchButton.Enabled = true;
                    }
                } catch (Exception) {
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if (comboBox1.SelectedItem.Equals("Recode-b120423") && !FileChecker.checkForFile(recode)) {
                launchButton.Text = "Install";
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if(progressBar1.Value.Equals(100)) {
                launchButton.Text = "Launch";
                label3.Text = "Idle";
                progressBar1.Value = 0;
                timer1.Enabled = false;
            }
        }
    }
}
