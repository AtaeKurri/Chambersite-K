using Chambersite_K.Graphics;
using Chambersite_K.Views;
using Microsoft.Xna.Framework.Graphics;
using RftSE_K.GameObjects.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RftSE_K.Views
{
    [InternalName("Game UI")]
    [ViewType(ViewType.Interface)]
    public class GameUI : View
    {
        public override void Initialize()
        {
            this.LoadResource<Texture2D>("ui_bg", "Assets/UI/ui_bg.png");
            CreateGameObject(new Interface());
            base.Initialize();
        }
    }
}
