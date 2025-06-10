using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AntivirusProgram.Core
{
    public class ProcessRiskInfo
    {
        public int Pid { get; set; }
        public string ExePath { get; set; }
        public int FileScore { get; set; }
        public bool HasRemoteThread { get; set; }
        public bool HasRWXMemory { get; set; }
        public bool HasSuspiciousModules { get; set; }
        public int TotalScore { get; set; }
    }

    public static class ProcessRiskEvaluator
    {
        public static List<ProcessRiskInfo> EvaluateAllProcesses()
        {
            var results = new List<ProcessRiskInfo>();

            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    string exePath = process.MainModule?.FileName ?? string.Empty;
                    if (string.IsNullOrEmpty(exePath))
                        continue;

                    int fileScore = FileRiskEvaluator.CalculateRiskScore(exePath);
                    bool hasRemoteThread = ProcessThreadAnalyzer.HasRemoteThread(process.Id);
                    bool hasRWXMemory = ProcessMemoryScanner.HasRWXMemoryRegion(process.Id);
                    bool hasSuspiciousModules = ProcessModuleAnalyzer.HasSuspiciousModules(process);

                    int totalScore = fileScore;
                    if (hasRemoteThread) totalScore += 50;
                    if (hasRWXMemory) totalScore += 30;
                    if (hasSuspiciousModules) totalScore += 20;

                    results.Add(new ProcessRiskInfo
                    {
                        Pid = process.Id,
                        ExePath = exePath,
                        FileScore = fileScore,
                        HasRemoteThread = hasRemoteThread,
                        HasRWXMemory = hasRWXMemory,
                        HasSuspiciousModules = hasSuspiciousModules,
                        TotalScore = totalScore
                    });
                }
                catch
                {
                    //Logger.LogWarning($"Access denied to process PID: {process.Id}");
                }
            }

            return results;
        }
    }
}
