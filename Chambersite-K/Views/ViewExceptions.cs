using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Views
{
    public class InvalidViewOperationException : Exception
    {
        public InvalidViewOperationException() { }
        public InvalidViewOperationException(string message) : base(message) { }
        public InvalidViewOperationException(string message, Exception inner) : base(message, inner) { }
    }
}
