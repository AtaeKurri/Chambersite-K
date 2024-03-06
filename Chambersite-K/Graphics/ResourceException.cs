using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Graphics
{
    /// <summary>
    /// Base Exception class for all exceptions in Resources.
    /// </summary>
    public class ResourceException : Exception
    {
        public ResourceException() { }
        public ResourceException(string message) : base(message) { }
        public ResourceException(string message, Exception inner) : base(message, inner) { }
    }
}
