using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Graphics
{
    public interface IResourceHolder
    {
        public List<Resource> LocalResources { get; set; }

        public Resource LoadLocalResource<T>(string name, string filepath);
        public List<Resource> GetGlobalResources();
    }
}
