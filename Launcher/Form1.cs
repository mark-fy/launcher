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

        private void button1_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e) {
            WindowState = FormWindowState.Minimized;
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(round(0, 0, Width, Height, 20, 20));
            gradientPanel1.Region = Region.FromHrgn(round(0, 0, gradientPanel1.Width, gradientPanel1.Height, 8, 8));
            button3.Region = Region.FromHrgn(round(0, 0, button3.Width, button3.Height, 8, 8));
            string avatarUrl = $"https://minotar.net/avatar/Steve/37.png";

            using (WebClient webClient = new WebClient()) {
                try {
                    byte[] data = webClient.DownloadData(avatarUrl);

                    using (System.IO.MemoryStream mem = new System.IO.MemoryStream(data)) {
                        userPicture.Image = Image.FromStream(mem);
                    }
                } catch (Exception ex) {
                    MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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

        // Launch Button
        private void button3_Click(object sender, EventArgs e) {
            string java = Path.Combine(Environment.CurrentDirectory, "files\\azul-1.8.9\\bin\\java.exe");
            string client = Path.Combine(Environment.CurrentDirectory, "client.jar");
            string lwjgl = Path.Combine(Environment.CurrentDirectory, "files\\lwjgl.jar");
            string lwjglUtil = Path.Combine(Environment.CurrentDirectory, "files\\lwjgl_util.jar");
            string minecraftJar = Path.Combine(Environment.CurrentDirectory, "files\\1.8.9.jar");
            string nativesFolder = Path.Combine(Environment.CurrentDirectory, "files\\1.8.9-natives");
            string minecraftFolder = Path.Combine(Environment.CurrentDirectory, ".minecraft");

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = java;
            startInfo.WorkingDirectory = minecraftFolder;
            startInfo.Arguments = $"-Xms1000M -Xmx8000M -Djava.library.path=\"" + nativesFolder + "\" -cp \"" + client + ";" + lwjgl + ";" + lwjglUtil + ";" + minecraftJar + "\" net.minecraft.client.main.Main -uuid fc5bc365-aedf-30a8-8b89-04e462e29bde -username Steve -accessToken yes -version 1 --assetIndex 1.8";
            try {
                Process.Start(startInfo);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
