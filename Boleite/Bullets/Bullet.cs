﻿using Chambersite_K.GameObjects;
using Chambersite_K.Graphics;
using Chambersite_K.Views;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boleite.Bullets
{
    public class BulletResourceLoader : GlobalResourceLoader
    {
        public override async Task<int> LoadResources()
        {
            LoadResource<Texture2D>("bullet1", "Assets/bullet/bullet1.png");
            LoadResource<Texture2D>("bullet2", "Assets/bullet/bullet2.png");
            return NumOfResourceLoaded;
        }
    }

    [InternalName("Bullet")]
    public class Bullet : GameObject
    {

    }
}
