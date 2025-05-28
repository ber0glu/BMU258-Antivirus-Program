using AntivirusProgram.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Services.Abstracts
{
    public interface IFileHashRecordService
    {
        void GetOneFileHashRecord(string hash, bool trackChanges);
        void CreateFileHashRecord(FileHashRecord fileHashRecord);
    }
}
