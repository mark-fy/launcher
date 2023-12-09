using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Launcher {
    internal class FileChecker {

        public static bool checkForFile(string fileName) {
            try {
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(exeDirectory, fileName);

                return File.Exists(filePath);
            } catch (Exception) {
                return false;
            }
        }

        public static bool checkForDirectory(string directoryName) {
            try {
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string directoryPath = Path.Combine(exeDirectory, directoryName);

                return Directory.Exists(directoryPath);
            } catch (Exception) {
                return false;
            }
        }

        public static bool IsDirectoryEmpty(string directoryName) {
            try {
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string directoryPath = Path.Combine(exeDirectory, directoryName);

                return !Directory.EnumerateFileSystemEntries(directoryPath).Any();
            } catch (Exception) {
                return false;
            }
        }

        private const string GitHubApiBaseUrl = "https://api.github.com/repos/mark-fy/db/contents";

        public static async Task<string[]> GetDirectoryContents(string repository) {
            try {
                string url = $"{GitHubApiBaseUrl}/{repository}";

                using (HttpClient client = new HttpClient()) {
                    client.DefaultRequestHeaders.Add("User-Agent", "YourAppName");

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode) {
                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        dynamic content = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);

                        if (content is Newtonsoft.Json.Linq.JArray) {
                            string[] contents = new string[content.Count];
                            for (int i = 0; i < content.Count; i++) {
                                contents[i] = content[i].name.ToString();
                            }
                            return contents;
                        } else {
                            Console.WriteLine("Unexpected response format: " + jsonResponse);
                        }
                    } else {
                        Console.WriteLine("Error: " + response.StatusCode);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return null;
        }

        private const string GitHubRawBaseUrl = "https://raw.githubusercontent.com";

        public static async Task<string[]> GetRawLinks(string repository, string path) {
            try {
                string apiUrl = $"{GitHubApiBaseUrl}/{repository}/contents/{path}";

                using (HttpClient client = new HttpClient()) {
                    client.DefaultRequestHeaders.Add("User-Agent", "YourAppName");

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode) {
                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        dynamic content = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);

                        if (content is Newtonsoft.Json.Linq.JArray) {
                            string[] rawLinks = new string[content.Count];
                            for (int i = 0; i < content.Count; i++) {
                                string rawLink = $"{GitHubRawBaseUrl}/{repository}/main/{path}/{content[i].name}";
                                rawLinks[i] = rawLink;
                            }
                            return rawLinks;
                        } else {
                            Console.WriteLine("Unexpected response format: " + jsonResponse);
                        }
                    } else {
                        Console.WriteLine("Error: " + response.StatusCode);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return null;
        }

    }
}
