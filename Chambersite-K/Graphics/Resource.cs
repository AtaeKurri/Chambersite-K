using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.Graphics
{
    public sealed class Resource
    {
        public string Name { get; set; }
        public object Res { get; set; }
        public string Path { get; set; }

        public Resource(string name, object res, string path)
        {
            Name = name;
            Res = res;
            Path = path;
        }

        public override string ToString()
        {
            return $"\"{Name}\" ({Res.GetType()})";
        }

        // TODO: Implémenter un check pour savoir si une resource du même nom (et type) existe, si oui, throw une Exception.
        public static Resource Load<T>(string resourceName, string filePath)
        {
            filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file provided doesn't exist: '{filePath}'");
            }
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                Type type = typeof(T);
                switch (type)
                {
                    // TODO: Ajouter tous les types pouvant être lus par un FileStream.
                    case Type _ when type == typeof(Texture2D):
                        return new Resource(resourceName, (T)Convert.ChangeType(_loadFromStream(fs), typeof(T)), filePath);
                    default:
                        throw new ArgumentException("The type provided is not a valid resource type.");
                }
            }
        }

        private static Texture2D _loadFromStream(FileStream fs) => Texture2D.FromStream(GAME._graphics.GraphicsDevice, fs);

        /// <summary>
        /// Load a resource into the Global Resource pool (will never go out of scope).<br/>
        /// </summary>
        /// <typeparam name="T">The type of the resource you want to load. Exemple: <see cref="Texture2D"/></typeparam>
        /// <param name="name">The resource name, used to identify the resource among other files</param>
        /// <param name="filepath">The relative filepath to the resource</param>
        /// <returns></returns>
        public static Resource LoadGlobalResource<T>(string name, string filepath)
        {
            Resource res = Load<T>(name, filepath);
            if (res != null)
                GAME.GlobalResource.Add(res);
            return res;
        }

        /// <summary>
        /// Attempts to find a resource inside both the local and global resource.<br/>
        /// If <paramref name="localResourceSource"/> is null, will only try in the global resources list.
        /// </summary>
        /// <typeparam name="T">The resource type</typeparam>
        /// <param name="localSource">The source of the local resource dict, usually a <see cref="View"/></param>
        /// <param name="name">The name identifier of the resource</param>
        /// <returns>A <see cref="Resource"/> with the matching name and type.</returns>
        /// <exception cref="KeyNotFoundException">The resource is not found in the local or global resources lists.</exception>
        public static Resource FindResource<T>(string resourceName, IResourceHolder localResourceSource=null)
        {
            Resource foundRes = null;
            // Will attempt to find in local resources first, and if not found, will try into the global dict, to save on resources.
            if (localResourceSource != null)
                 foundRes = localResourceSource.LocalResources.Find(res => res.Res is T && res.Name == resourceName);
            if (foundRes == null)
            {
                foundRes = GAME.GlobalResource.Find(res => res.Res is T && res.Name == resourceName);
                if (foundRes == null)
                    throw new KeyNotFoundException($"This resource '{resourceName}' doesn't exist with the type '{typeof(T)}'.");
            }
            return foundRes;
        }

        /// <summary>
        /// Renders a <see cref="Texture2D"/>. Doesn't do anything if the Resource is not a <see cref="Texture2D"/>.
        /// </summary>
        /// <param name="position">Position of the image on the screen</param>
        public void Render(Vector2 position)
        {   
            Render(position, 0f, Vector2.One, Color.White, SpriteEffects.None);
        }

        /// <summary>
        /// Renders a <see cref="Texture2D"/>. Doesn't do anything if the Resource is not a <see cref="Texture2D"/>.
        /// </summary>
        /// <param name="position">Position of the image on the screen</param>
        /// <param name="rotation">Rotation in radian of the image</param>
        /// <param name="scale">Size multiplier. A scale of Vector2(1, 1) is the default behaviour</param>
        public void Render(Vector2 position, float rotation, Vector2 scale)
        {
            Render(position, rotation, scale, Color.White, SpriteEffects.None);
        }

        /// <summary>
        /// Renders a <see cref="Texture2D"/>. Doesn't do anything if the Resource is not a <see cref="Texture2D"/>.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        /// <param name="spriteEffects"></param>
        public void Render(Vector2 position, float rotation, Vector2 scale, Color color, SpriteEffects spriteEffects=SpriteEffects.None)
        {
            if (Res is Texture2D)
            {
                Vector2 origin = new Vector2((Res as Texture2D).Width/2, (Res as Texture2D).Height / 2);
                GAME._spriteBatch.Draw((Texture2D)Res, position, null, color, rotation, origin, scale, spriteEffects, 0);
            }
        }

        public void RenderRect(Rectangle position, Rectangle originRect, float rotation)
        {
            if (Res is Texture2D)
            {
                Vector2 origin = new Vector2((Res as Texture2D).Width / 2, (Res as Texture2D).Height / 2);
                GAME._spriteBatch.Draw((Texture2D)Res, position, originRect, Color.White, rotation, origin, SpriteEffects.None, 0);
            }
        }
    }

    public static class ResourceExtensions
    {
        /// <summary>
        /// Finds the first resource that matches the given type and name.
        /// </summary>
        /// <typeparam name="T">The resource's type</typeparam>
        /// <param name="list">The source list</param>
        /// <param name="resourceName">The resource's name as given in the <see cref="Resource.Load{T}(string, string)"/> method</param>
        /// <returns>The resource's data</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no resource with the given type and/or name exists</exception>
        public static Resource FindResource<T>(this List<Resource> list, string resourceName)
        {
            Resource foundRes = list.Find(res => res.Res is T && res.Name == resourceName);
            if (foundRes != null)
                return foundRes;
            else
                throw new KeyNotFoundException($"This resource '{resourceName}' doesn't exist with the type '{typeof(T)}'.");
        }
    }
}
