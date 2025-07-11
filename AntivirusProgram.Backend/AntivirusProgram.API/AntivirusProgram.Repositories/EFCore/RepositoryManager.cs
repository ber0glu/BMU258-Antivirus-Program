﻿using AntivirusProgram.Repositories.Abstracts;
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
        private readonly Lazy<IVirusRepository> hashRepository;

        public RepositoryManager(RepositoryContext context)
        {
            _context = context;
            hashRepository = new Lazy<IVirusRepository>(() => new VirusRepository(_context));
        }

        public IVirusRepository VirusRepository => hashRepository.Value;
        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}
