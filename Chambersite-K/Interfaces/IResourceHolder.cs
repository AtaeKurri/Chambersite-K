using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Interfaces
{
    public interface IResourceHolder
    {
        public List<IResource> ResourcePool { get; set; }

        //public Resource<T> LoadLocalResource<T>(string name, string filepath);
        public List<IResource> GetGlobalResources();
    }
}
