using AntivirusProgram.Repositories.Abstracts;
using AntivirusProgram.Services.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IFileHashRecordService> _fileHashRecordService;


        public ServiceManager(IRepositoryManager repository)
        {
            _fileHashRecordService = new Lazy<IFileHashRecordService>(() => new FileHashRecordService(repository));
        }

        public IFileHashRecordService FileHashRecordService => _fileHashRecordService.Value;
    }
}
