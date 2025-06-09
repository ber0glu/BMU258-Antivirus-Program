using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Entities.Models
{
    public class FileScanResult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        [StringLength(64)]
        public string FileHash { get; set; }  // SHA-256 için 64 karakter

        public bool IsVirus { get; set; }

        public bool IsTestFile { get; set; }

        public DateTime ScanDate { get; set; } = DateTime.UtcNow;

        public string AdditionalInfo { get; set; } 
    }
}
