using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Entities.Models
{
    public class FileHashRecord
    {
        [Key] public int Id { get; set; }

        [Required][MaxLength(40)] public string Sha256 { get; set; }

        public string FileName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ScannedAt { get; set; }

        public string ScanResultJson { get; set; }
    }
}
