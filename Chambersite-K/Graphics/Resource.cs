using Assimp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Chambersite_K.Graphics
{
    public sealed class Resource
    {
        public string Name { get; set; }
        public object Res { get; set; }
        public string Path { get; set; }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Resource(string name, object res, string path)
        {
            Name = name;
            Res = res;
            Path = path;
            Logger.Info("Resource of type {0}: '{1}' ({2}) loaded.", Res.GetType().Name, Name, Path);
        }

        public override string ToString()
        {
            return $"\"{Name}\" ({Res.GetType().Name})";
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
                try
                {
                    Type type = typeof(T);
                    switch (type)
                    {
                        // TODO: Ajouter tous les types pouvant être lus par un FileStream.
                        case Type when type == typeof(Texture2D):
                            return new Resource(resourceName, (T)Convert.ChangeType(_loadTexture2DFromStream(fs), typeof(T)), filePath);
                        case Type when type == typeof(Model):
                            return new Resource(resourceName, (T)Convert.ChangeType(_LoadModelFromFile(filePath), typeof(T)), filePath);
                        case Type when type == typeof(TTFFont):
                            return new Resource(resourceName, (T)Convert.ChangeType(new TTFFont(filePath), typeof(T)), filePath);
                        case Type when type == typeof(Song):
                            return new Resource(resourceName, (T)Convert.ChangeType(Song.FromUri(resourceName, new Uri(filePath)), typeof(T)), filePath);
                        case Type when type == typeof(SoundEffect):
                            return new Resource(resourceName, (T)Convert.ChangeType(_loadSoundEffectFromStream(fs), typeof(T)), filePath);
                        default:
                            throw new ArgumentException("The type provided is not a valid resource type.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"The file {filePath} couldn't be loaded: {ex}");
                    return null;
                }
                finally
                {
                    // Normally that shouldn't be needed but JUST IN CASE.
                    fs.Close();
                }
            }
        }

        private static Texture2D _loadTexture2DFromStream(FileStream fs) => Texture2D.FromStream(GAME._graphics.GraphicsDevice, fs);
        private static SoundEffect _loadSoundEffectFromStream(FileStream fs) => SoundEffect.FromStream(fs);

        private static Model _LoadModelFromFile(string fileName)
        {
            AssimpContext importer = new AssimpContext();
            Scene scene = importer.ImportFile(fileName, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);

            List<ModelMesh> meshes = new List<ModelMesh>();
            foreach (var mesh in scene.Meshes)
            {
                VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[mesh.VertexCount];
                int[] indices = new int[mesh.FaceCount * 3];

                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    Vector3 position = new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z);
                    Vector3 normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);
                    Vector2 textureCoordinate = mesh.HasTextureCoords(0) ? new Vector2(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y) : Vector2.Zero;

                    vertices[i] = new VertexPositionNormalTexture(position, normal, textureCoordinate);
                }

                for (int i = 0; i < mesh.FaceCount; i++)
                {
                    Face face = mesh.Faces[i];
                    indices[i * 3] = face.Indices[0];
                    indices[i * 3 + 1] = face.Indices[1];
                    indices[i * 3 + 2] = face.Indices[2];
                }

                ModelMeshPart meshPart = new ModelMeshPart();
                meshPart.VertexBuffer = new VertexBuffer(GAME.GraphicsDevice, typeof(VertexPositionNormalTexture), mesh.VertexCount, BufferUsage.WriteOnly);
                meshPart.VertexBuffer.SetData(vertices);
                meshPart.IndexBuffer = new IndexBuffer(GAME.GraphicsDevice, IndexElementSize.ThirtyTwoBits, mesh.FaceCount * 3, BufferUsage.WriteOnly);
                meshPart.IndexBuffer.SetData(indices);

                ModelMesh modelMesh = new ModelMesh(GAME.GraphicsDevice, new List<ModelMeshPart> { meshPart });
                meshes.Add(modelMesh);
            }

            return new Model(GAME.GraphicsDevice, null, meshes);
        }

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
