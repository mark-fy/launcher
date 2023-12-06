using System;
using System.IO;
using System.Linq;

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

    }
}
