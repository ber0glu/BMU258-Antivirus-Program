using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Services.Abstracts
{
    public interface IServiceManager
    {
        IFileHashRecordService FileHashRecordService { get; }

    }
}
