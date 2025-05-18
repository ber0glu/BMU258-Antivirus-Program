using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Entities.Models
{
    /// <summary>
    /// For VirusTotal Api
    /// </summary>
    public class FileHashRequest
    {
        public string Sha256 { get; set; }
    }
}
