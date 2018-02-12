using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Persistence.Exceptions
{
    public class DataCannotChangeIdException : Exception
    {
        public DataCannotChangeIdException() : base() { }
        public DataCannotChangeIdException(string message) : base(message) { }
        public DataCannotChangeIdException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
