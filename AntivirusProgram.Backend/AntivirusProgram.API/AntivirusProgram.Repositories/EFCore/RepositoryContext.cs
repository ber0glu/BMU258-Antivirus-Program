using AntivirusProgram.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Repositories.EFCore
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options) { }
        
        public DbSet<FileScanResult> FileScanResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //TODO: Burası Hash in uniq olmasını sağlar.
            modelBuilder.Entity<FileScanResult>()
               .HasIndex(f => f.FileHash)
               .IsUnique();
        }
    }
}
