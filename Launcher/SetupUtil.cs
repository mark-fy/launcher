using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher {
    internal class SetupUtil {

        public static bool CreateFolder(string folderName) {
            try {
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string folderPath = Path.Combine(exeDirectory, folderName);

                if (!Directory.Exists(folderPath)) {
                    Directory.CreateDirectory(folderPath);
                    return true;
                } else {
                    Console.WriteLine($"Folder '{folderName}' already exists.");
                    return false;
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error creating folder: {ex.Message}");
                return false;
            }
        }

    }
}
