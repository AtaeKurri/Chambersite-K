using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Graphics
{
    public abstract class GlobalResourceLoader
    {
        public bool AreResourcesLoaded { get; set; } = false;
        public int NumOfResourceLoaded { get; set; } = 0;

        public virtual async Task<int> LoadResources()
        {
            AreResourcesLoaded = true;
            return NumOfResourceLoaded;
        }

        /// <summary>
        /// Loads a resource, works the same as <see cref="Resource.LoadGlobalResource{T}(string, string)"/>
        /// </summary>
        /// <remarks>This method increments <see cref="NumOfResourceLoaded"/> by one each time.</remarks>
        /// <typeparam name="T">The resource internal type.</typeparam>
        /// <param name="resourceName">The identifier name for this resource.</param>
        /// <param name="filePath">The relative path to the resource's file.</param>
        public async Task<T> LoadResource<T>(string resourceName, string filePath)
        {
            Resource<T> res = Resource<T>.LoadGlobalResource(resourceName, filePath);
            NumOfResourceLoaded++;
            return (T)Convert.ChangeType(res, typeof(T));
        }
    }
}
