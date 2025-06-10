using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace AntivirusProgram.Core
{
    public static class FileRiskEvaluator
    {
        private static readonly Dictionary<string, int> ExtensionScores = new()
        {
            [".exe"] = 50,
            [".dll"] = 50,
            [".scr"] = 50,
            [".bat"] = 50,
            [".ps1"] = 50,
            [".vbs"] = 50,
            [".js"] = 50,
            [".docm"] = 30,
            [".xlsm"] = 30,
            [".pdf"] = 10,
            [".docx"] = 10,
            [".xlsx"] = 10,
            [".rtf"] = 10
        };

        public static int CalculateRiskScore(string filePath)
        {
            int score = 0;

            string ext = Path.GetExtension(filePath)?.ToLower();
            if (!ExtensionScores.TryGetValue(ext, out int extScore))
                return 0;
            score += extScore;

            if (IsInSuspiciousDirectory(filePath))
                score += 20;

            long size = GetFileSize(filePath);
            if (size < 1024)
                score += 15;
            else if (size > 100 * 1024 * 1024)
                score -= 10;

            if (!IsValidDigitalSignature(filePath))
                score += 30;

            if (IsHiddenOrReadOnly(filePath))
                score += 10;

            if (ContainsSuspiciousStrings(filePath))
                score += 40;

            if (ContainsBase64Strings(filePath))
                score += 20;

            if (HasHighNullByteRatio(filePath))
                score += 15;

            if (HasSuspiciousEntryPoint(filePath))
                score += 20;

            return score;
        }

        public static long GetFileSize(string filePath)
        {
            try
            {
                return new FileInfo(filePath).Length;
            }
            catch
            {
                return 0;
            }
        }

        public static bool IsInSuspiciousDirectory(string filePath)
        {
            string lowerPath = filePath.ToLower();
            return lowerPath.Contains("appdata") || lowerPath.Contains("temp") ||
                   lowerPath.Contains("downloads") || lowerPath.Contains("startup");
        }

        public static bool WasRecentlyModified(string filePath)
        {
            try
            {
                var lastWriteTime = File.GetLastWriteTime(filePath);
                return (DateTime.Now - lastWriteTime) < TimeSpan.FromHours(24);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsHiddenOrReadOnly(string filePath)
        {
            try
            {
                var attributes = File.GetAttributes(filePath);
                return attributes.HasFlag(FileAttributes.Hidden) || attributes.HasFlag(FileAttributes.ReadOnly);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidDigitalSignature(string filePath)
        {
            try
            {
                var cert = new X509Certificate2(filePath);
                var chain = new X509Chain();
                return chain.Build(cert);
            }
            catch
            {
                return false;
            }
        }

        public static bool ContainsSuspiciousStrings(string filePath)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                string content = Encoding.ASCII.GetString(bytes);

                string[] suspiciousPatterns = {
                    "CreateRemoteThread", "VirtualAllocEx", "WriteProcessMemory",
                    "cmd.exe", "powershell", "socket", "WinExec"
                };

                return suspiciousPatterns.Any(pattern => content.Contains(pattern, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        public static bool ContainsBase64Strings(string filePath)
        {
            try
            {
                string content = Convert.ToBase64String(File.ReadAllBytes(filePath));
                return Regex.IsMatch(content, @"([A-Za-z0-9+/]{20,}={0,2})");
            }
            catch
            {
                return false;
            }
        }

        public static bool HasHighNullByteRatio(string filePath)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                if (bytes.Length == 0) return false;

                int nullCount = bytes.Count(b => b == 0x00);
                double ratio = (double)nullCount / bytes.Length;
                return ratio > 0.2; 
            }
            catch
            {
                return false;
            }
        }

        public static bool HasSuspiciousEntryPoint(string filePath)
        {
            try
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using var br = new BinaryReader(fs);

                fs.Seek(0x3C, SeekOrigin.Begin);
                int e_lfanew = br.ReadInt32();

                fs.Seek(e_lfanew, SeekOrigin.Begin);
                uint ntSignature = br.ReadUInt32();
                if (ntSignature != 0x00004550) 
                    return false;

                fs.Seek(16, SeekOrigin.Current); 
                ushort magic = br.ReadUInt16();  
                fs.Seek(14, SeekOrigin.Current); 

                uint entryPoint = br.ReadUInt32();
                uint imageBase = br.ReadUInt32();

                return entryPoint < 0x400;
            }
            catch
            {
                return false;
            }
        }
    }
}
