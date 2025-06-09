using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Entities.Exceptions
{
    public class FileHashAlreadyExistsException : ConflictException
    {
        public FileHashAlreadyExistsException(string hash) : base($"A record with the hash '{hash}' already exists in the database.")
        {
            
        }
    }
}
