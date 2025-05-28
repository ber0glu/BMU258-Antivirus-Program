using AntivirusProgram.Entities.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Repositories.Abstracts
{
    public interface IFileHashRecordRepository : IRepositoryBase<FileHashRecord>
    {
        Task<List<FileHashRecord>> GetAllFileHashRecordsAsync(bool trackChanges);

        Task<FileHashRecord> GetOneFileHashRecordtAsync(int id, bool trackChanges);
        Task<FileHashRecord> GetOneFileHashRecordAsync(string hash, bool trackChanges);
        void CreateOneFileHashRecord(FileHashRecord fileHashRecord);
        void UpdateOneFileHashRecord(FileHashRecord fileHashRecord);
        void DeleteOneFileHashRecord(FileHashRecord fileHashRecord);

    }
}
