using AntivirusProgram.Entities.Models;
using AntivirusProgram.Repositories.Abstracts;
using AntivirusProgram.Services.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Services
{
    public class FileHashRecordService : IFileHashRecordService
    {
        private readonly IRepositoryManager repositoryManager;

        public FileHashRecordService(IRepositoryManager repositoryManager)
        {
            this.repositoryManager = repositoryManager;
        }

        public void CreateFileHashRecord(FileHashRecord fileHashRecord)
        {
            repositoryManager.FileHashRecordRepository.CreateOneFileHashRecord(fileHashRecord);
        }

        public void GetOneFileHashRecord(string hash, bool trackChanges)
        {
            if (hash == string.Empty)
                throw new ArgumentNullException("hash cannot be null!");
            var response = repositoryManager.FileHashRecordRepository.GetOneFileHashRecordAsync(hash, trackChanges);
            //TODO: Burada exceptionHandler ile 404 not found döndür.
            if (response == null)
                throw new Exception("hash bulunamadı");
            //TODO: response model düşün. kullanıcıya nasıl yanıt vermeli hakkında.
        }
    }
}
