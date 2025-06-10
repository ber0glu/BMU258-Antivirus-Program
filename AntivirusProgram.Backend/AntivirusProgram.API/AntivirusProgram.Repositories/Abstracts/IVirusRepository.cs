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
    public interface IVirusRepository: IRepositoryBase<FileScanResult>
    {
        Task<List<FileScanResult>> GetAllFileScanResultsAsync(bool trackChanges);

        Task<FileScanResult> GetFileScanResultByIdAsync(int id, bool trackChanges);

        Task<FileScanResult> GetFileScanResultByHashAsync(string hash, bool trackChanges);

        void CreateFileScanResult(FileScanResult fileScanResult);

        void UpdateFileScanResult(FileScanResult fileScanResult);

        void DeleteFileScanResult(FileScanResult fileScanResult);
    }
}
