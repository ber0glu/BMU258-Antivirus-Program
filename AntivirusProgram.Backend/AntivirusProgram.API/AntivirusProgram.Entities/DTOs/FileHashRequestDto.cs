using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Entities.DTOs
{
    public class FileHashRequestDto
    {
        [Required(ErrorMessage = "SHA256 değeri zorunludur.")]
        [MaxLength(64, ErrorMessage = "SHA256 maksimum 64 karakter olmalıdır.")]
        public string Sha256 { get; set; }
        public string FileName { get; set; }
    }
}
