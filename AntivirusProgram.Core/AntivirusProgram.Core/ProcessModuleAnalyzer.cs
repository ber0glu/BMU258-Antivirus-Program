using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AntivirusProgram.Core
{
    public static class ProcessModuleAnalyzer
    {
        public static List<string> GetModulePaths(Process process)
        {
            var modules = new List<string>();

            try
            {
                foreach (ProcessModule module in process.Modules)
                {
                    modules.Add(module.FileName);
                }
            }
            catch
            {
                // Erişim engeli vs. olabilir, loglanabilir
            }

            return modules;
        }

        public static bool HasSuspiciousModules(Process process)
        {
            var modules = GetModulePaths(process);
            foreach (string path in modules)
            {
                string lowerPath = path.ToLowerInvariant();
                if (lowerPath.Contains("appdata") || lowerPath.Contains("temp") || lowerPath.Contains("roaming"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
