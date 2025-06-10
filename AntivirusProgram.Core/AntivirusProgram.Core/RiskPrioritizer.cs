using System;
using System.Collections.Generic;
using System.Linq;

namespace AntivirusProgram.Core
{
    public class FileTask
    {
        public string FilePath { get; set; }
        public string Hash { get; set; }
        public int RiskScore { get; set; }
    }

    public class RiskPrioritizer
    {
        private readonly List<FileTask> taskList = new();

        public void AddFile(string filePath, string hash)
        {
            int score = FileRiskEvaluator.CalculateRiskScore(filePath);
            taskList.Add(new FileTask
            {
                FilePath = filePath,
                Hash = hash,
                RiskScore = score
            });
        }

        public bool HasNext()
        {
            return taskList.Count > 0;
        }

        public FileTask GetNext()
        {
            if (!HasNext()) return null;
            var top = taskList.OrderByDescending(t => t.RiskScore).First();
            taskList.Remove(top);
            return top;
        }
    }
}
