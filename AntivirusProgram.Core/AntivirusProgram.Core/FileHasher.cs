using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AntivirusProgram.Core
{
    public static class FileHasher
    {
        public static string GetFileSHA256(string filePath)
        {
            if (!File.Exists(filePath))
                return string.Empty;

            try
            {
                using FileStream stream = File.OpenRead(filePath);
                using SHA256 sha256 = SHA256.Create();
                byte[] hashBytes = sha256.ComputeHash(stream);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
