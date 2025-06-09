using AntivirusProgram.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Services.Abstracts
{
    public interface IVirusTotalClient
    {
        Task<VirusTotalResult> QueryHashAsync(string sha256);
    }
}
