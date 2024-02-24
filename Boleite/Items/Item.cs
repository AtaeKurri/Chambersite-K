using Boleite.Players;
using Chambersite_K.GameObjects;
using Chambersite_K.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boleite.Items
{
    [InternalName("Base Item")]
    public abstract class Item : GameObject
    {
        public override GameObjectGroup Group => GameObjectGroup.Item;
        public override bool CheckBound => false;

        public override void Initialize()
        {
            
            base.Initialize();
        }

        public override void Update()
        {
            if (Position.Y > ParentView.WorldBounds.WorldTop)
                return; // Render the ItemUp texture
            else
                base.Update();
        }

        public override void Draw()
        {

            base.Draw();
        }

        public virtual void Collect(Player player)
        {

        }
    }
}
