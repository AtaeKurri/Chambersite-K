using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Interfaces
{
    public interface IResource
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public object GetRes();
    }
}
