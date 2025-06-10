using System;
using System.IO;

namespace AntivirusProgram.Core
{
    public static class QuarantineManager
    {
        private static readonly string QuarantineDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quarantine");

        public static bool QuarantineFile(string filePath, bool secureDelete = false)
        {
            try
            {
                if (!Directory.Exists(QuarantineDirectory))
                    Directory.CreateDirectory(QuarantineDirectory);

                string fileName = Path.GetFileName(filePath);
                string destPath = Path.Combine(QuarantineDirectory, fileName);

                // If file with same name exists, add a timestamp
                if (File.Exists(destPath))
                {
                    string name = Path.GetFileNameWithoutExtension(fileName);
                    string ext = Path.GetExtension(fileName);
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    destPath = Path.Combine(QuarantineDirectory, $"{name}_{timestamp}{ext}");
                }

                File.Move(filePath, destPath);

                if (secureDelete)
                {
                    SecureDeleteFile(destPath);
                }

                return true;
            }
            catch (Exception)
            {
                // Optionally log the error
                return false;
            }
        }

        public static void SecureDeleteFile(string filePath)
        {
            if (!File.Exists(filePath)) return;
            try
            {
                var fileInfo = new FileInfo(filePath);
                long length = fileInfo.Length;
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Write))
                {
                    byte[] randomData = new byte[4096];
                    var rng = new Random();
                    long written = 0;
                    while (written < length)
                    {
                        rng.NextBytes(randomData);
                        int toWrite = (int)Math.Min(randomData.Length, length - written);
                        fs.Write(randomData, 0, toWrite);
                        written += toWrite;
                    }
                }
                File.Delete(filePath);
            }
            catch (Exception)
            {
                // Optionally log the error
            }
        }
    }
} 