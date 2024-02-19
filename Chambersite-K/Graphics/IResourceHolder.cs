using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Graphics
{
    public interface IResourceHolder
    {
        public List<IResource> LocalResources { get; set; }

        public Resource<T> LoadLocalResource<T>(string name, string filepath);
        public List<IResource> GetGlobalResources();
    }
}
