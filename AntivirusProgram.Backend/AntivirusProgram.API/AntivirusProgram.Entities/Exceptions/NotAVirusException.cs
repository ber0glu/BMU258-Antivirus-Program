using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusProgram.Entities.Exceptions
{
    public class NotAVirusException : Exception
    {
        public NotAVirusException(string hash)
           : base($"The file with hash '{hash}' is not identified as a virus.")
        {
        }
    }
}
