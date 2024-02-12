using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Color = Microsoft.Xna.Framework.Color;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Chambersite_K.World
{
    public struct FogSettings
    {
        public FogSettings() { }

        public bool EnableFog { get; set; } = false;
        public Color FogColor { get; set; } = Color.Blue;
        public float FogStart { get; set; } = 5f;
        public float FogEnd { get; set; } = 20f;
        public float FogDensity { get; set; } = 0.1f;
    }

    public sealed class World3D
    {
        public Camera3D Camera { get; set; }
        public FogSettings FogSettings { get; set; }

        public World3D() { }

        public override string ToString()
        {
            return "World3D placeholder ToString()";
        }

        public void InitCamera(Vector3 camPosition, Vector3 camTarget, Vector3 camUp, float FOV, float nearPlaneDistance, float farPlaneDistance)
        {
            Camera = new Camera3D(camPosition, camTarget, camUp, FOV, nearPlaneDistance, farPlaneDistance);
        }

        public void InitFog(FogSettings fogSettings)
        {
            FogSettings = fogSettings;
        }

        public void RenderModel(Model model, Vector3 position)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateTranslation(position);
                    effect.View = Camera.ViewMatrix;
                    effect.Projection = Camera.ProjectionMatrix;
                }
                mesh.Draw();
            }
        }
    }
}
