using AntivirusProgram.Repositories.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Repositories.EFCore
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _context;

        //this place add for lazy loading 
        public RepositoryManager(RepositoryContext context)
        {
            _context = context;
        }
        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}
