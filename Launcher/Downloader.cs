using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using System.Net;

namespace Launcher {
    internal class Downloader {

        private static ProgressBar progressBar;
        private static Label progressLabel;

        public static bool downloadFile(ProgressBar progressBarC, string downloadUrl, string savePath) {
            progressBar = progressBarC;

            try {
                using (WebClient webClient = new WebClient()) {
                    webClient.DownloadProgressChanged += WebClientDownloadProgressChanged;

                    webClient.DownloadFileAsync(new Uri(downloadUrl), savePath);
                    return true;
                }
            } catch (Exception) {
                return false;
            }
        }

        private static void WebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            progressBar.Value = e.ProgressPercentage;
        }

        public static bool downloadAndExtract(ProgressBar progressBarC, string downloadUrl, string extractPath, string fileName) {
            progressBar = progressBarC;

            try {
                string zipFilePath = Path.Combine(Environment.CurrentDirectory, "files\\" + fileName + " .zip");
                using (WebClient webClient = new WebClient()) {
                    webClient.DownloadProgressChanged += WebClientDownloadProgressChanged;
                    webClient.DownloadFileCompleted += (sender, e) =>
                    {
                        try {
                            Directory.CreateDirectory(extractPath);
                            ZipFile.ExtractToDirectory(zipFilePath, extractPath);
                            File.Delete(zipFilePath);
                        } catch (Exception) {}
                    };
                    webClient.DownloadFileAsync(new Uri(downloadUrl), zipFilePath);
                }

                return true;
            } catch (Exception) {
                return false;
            }
        }

        public static async Task DownloadFilesSequentially(ProgressBar progressBarC, Label progressLabelC, Dictionary<string, string> downloadUrlsAndPaths) {
            progressBar = progressBarC;
            progressLabel = progressLabelC;

            foreach (var entry in downloadUrlsAndPaths) {
                string downloadUrl = entry.Key;
                string savePath = entry.Value;

                string fileName = Path.GetFileName(savePath);
                progressLabel.Text = $"Downloading: {fileName} (0/0MB)";

                if (await DownloadFile(downloadUrl, savePath)) {
                    Console.WriteLine($"Downloaded: {downloadUrl}");
                    progressBar.Value = 0;
                } else {
                    Console.WriteLine($"Error downloading: {downloadUrl}");
                }
            }
        }

        private static async Task<bool> DownloadFile(string downloadUrl, string savePath) {
            try {
                using (HttpClient httpClient = new HttpClient())
                using (HttpResponseMessage response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead)) {
                    response.EnsureSuccessStatusCode();

                    long totalBytes = response.Content.Headers.ContentLength ?? -1;
                    long bytesRead = 0;

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                                  stream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true)) {
                        byte[] buffer = new byte[8192];
                        int bytesReadThisChunk;

                        while ((bytesReadThisChunk = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                            await stream.WriteAsync(buffer, 0, bytesReadThisChunk);
                            bytesRead += bytesReadThisChunk;

                            // Update progress bar and label
                            int progressPercentage = (int)(bytesRead * 100 / totalBytes);
                            progressBar.Value = progressPercentage;
                            progressLabel.Text = $"Downloading: {Path.GetFileName(savePath)} ({bytesRead / 1024}KB/{totalBytes / 1024}KB)";
                        }
                    }
                }

                // Extract the downloaded file
                await ExtractFile(savePath);

                return true;
            } catch (Exception ex) {
                Console.WriteLine($"Error downloading file: {ex.Message}");
                return false;
            }
        }

        private static async Task ExtractFile(string zipFilePath) {
            try {
                string extractPath = Path.GetDirectoryName(zipFilePath);

                using (var archive = ZipFile.OpenRead(zipFilePath)) {
                    foreach (var entry in archive.Entries) {
                        string entryPath = Path.Combine(extractPath, entry.FullName);
                        entryPath = entryPath.Replace('/', Path.DirectorySeparatorChar);

                        // Ensure the extracted file's parent directory exists
                        Directory.CreateDirectory(Path.GetDirectoryName(entryPath));

                        if (entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\")) {
                            // Ensure the extracted directory exists
                            Directory.CreateDirectory(entryPath);
                        } else {
                            // Extract the file
                            entry.ExtractToFile(entryPath, true);
                        }
                    }
                }

                // Delete the downloaded zip file
                File.Delete(zipFilePath);
            } catch (Exception ex) {
                Console.WriteLine($"Error extracting file: {ex.Message}");
            }
        }
    }
}
