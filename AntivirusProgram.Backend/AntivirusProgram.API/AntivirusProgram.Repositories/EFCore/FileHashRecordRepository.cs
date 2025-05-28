using AntivirusProgram.Entities.Models;
using AntivirusProgram.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Repositories.EFCore
{
    public class FileHashRecordRepository : RepositoryBase<FileHashRecord>,IFileHashRecordRepository
    {
        public FileHashRecordRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateOneFileHashRecord(FileHashRecord fileHashRecord) => Create(fileHashRecord);

        public void DeleteOneFileHashRecord(FileHashRecord fileHashRecord)=>Delete(fileHashRecord);

        public async Task<List<FileHashRecord>> GetAllFileHashRecordsAsync(bool trackChanges)
        {
            return await
                 FindAll(trackChanges)
                 .OrderBy(a => a.Id)
                 .ToListAsync();
        }

        public async Task<FileHashRecord> GetOneFileHashRecordAsync(string hash, bool trackChanges)
        {
            return await FindByCondition(b => b.Sha256.Equals(hash), trackChanges)
            .SingleOrDefaultAsync();
        }

        public async Task<FileHashRecord> GetOneFileHashRecordtAsync(int id, bool trackChanges)
        {
            return await FindByCondition(b => b.Id.Equals(id), trackChanges)
            .SingleOrDefaultAsync();
        }

        public void UpdateOneFileHashRecord(FileHashRecord fileHashRecord) => Update(fileHashRecord);
    }
}
