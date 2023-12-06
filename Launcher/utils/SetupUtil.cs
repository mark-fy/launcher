using System;
using System.IO;

namespace Launcher {
    internal class SetupUtil {

        public static bool createFolder(string folderName) {
            try {
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string folderPath = Path.Combine(exeDirectory, folderName);

                if (!Directory.Exists(folderPath)) {
                    Directory.CreateDirectory(folderPath);
                    return true;
                } else {
                    return false;
                }
            } catch (Exception) {
                return false;
            }
        }

    }
}
