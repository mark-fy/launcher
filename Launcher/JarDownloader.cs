using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher {
    internal class JarDownloader {

        private static ProgressBar progressBar;

        public static bool downloadJar(ProgressBar progressBarC, string downloadUrl, string savePath) {
            progressBar = progressBarC;

            try {
                using (WebClient webClient = new WebClient()) {
                    webClient.DownloadProgressChanged += WebClientDownloadProgressChanged;

                    webClient.DownloadFileAsync(new Uri(downloadUrl), savePath);
                    return true;
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error downloading file: {ex.Message}");
                return false;
            }
        }

        private static void WebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            progressBar.Value = e.ProgressPercentage;
        }
    }
}
