using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Repositories.Abstracts
{
    public interface IRepositoryManager
    {
        IVirusRepository VirusRepository {  get; }
        Task SaveAsync();
    }
}
