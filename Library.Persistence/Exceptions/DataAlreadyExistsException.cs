using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Persistence.Exceptions
{
    public class DataAlreadyExistsException : Exception
    {
        public DataAlreadyExistsException() : base() { }
        public DataAlreadyExistsException(string message) : base(message) { }
        public DataAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
