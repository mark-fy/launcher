using System;
using System.IO;

namespace Launcher {
    internal class FileChecker {

        public static bool checkForFile(string fileName) {
            try {
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string filePath = Path.Combine(exeDirectory, fileName);

                return File.Exists(filePath);
            } catch (Exception ex) {
                Console.WriteLine($"Error checking file existence: {ex.Message}");
                return false;
            }
        }

    }
}
