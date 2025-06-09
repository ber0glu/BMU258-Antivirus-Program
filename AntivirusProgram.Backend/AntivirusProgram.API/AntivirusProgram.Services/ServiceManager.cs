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
        private readonly Lazy<IVirusScanService> _fileHashRecordService;
        public IVirusTotalClient VirusTotalClient { get; }


        public ServiceManager(IRepositoryManager repository, IVirusTotalClient virusTotalClient)
        {
            _fileHashRecordService = new Lazy<IVirusScanService>(() => new VirusScanService(repository,virusTotalClient));
            VirusTotalClient = virusTotalClient;
        }

        public IVirusScanService FileHashRecordService => _fileHashRecordService.Value;
    }
}
