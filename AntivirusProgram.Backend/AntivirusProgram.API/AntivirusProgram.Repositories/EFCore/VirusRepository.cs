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
    public class VirusRepository : RepositoryBase<FileScanResult>, IVirusRepository
    {
        public VirusRepository(RepositoryContext context) : base(context)
        {
        }

        public async Task<List<FileScanResult>> GetAllFileScanResultsAsync(bool trackChanges)=> await FindAll(trackChanges).ToListAsync();

        public async Task<FileScanResult> GetFileScanResultByIdAsync(int id, bool trackChanges) => await FindByCondition(f => f.Id == id, trackChanges).SingleOrDefaultAsync();

        public async Task<FileScanResult> GetFileScanResultByHashAsync(string hash, bool trackChanges) => await FindByCondition(f => f.FileHash == hash, trackChanges).SingleOrDefaultAsync();

        public void CreateFileScanResult(FileScanResult fileScanResult) => Create(fileScanResult);

        public void UpdateFileScanResult(FileScanResult fileScanResult) => Update(fileScanResult);

        public void DeleteFileScanResult(FileScanResult fileScanResult) => Delete(fileScanResult);
    }
}