using Chambersite_K.GameObjects;
using Chambersite_K.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RftSE_K.GameObjects.UI
{
    [InternalName("UI")]
    public class Interface : GameObject
    {
        public override void Initialize()
        {
            Image = "ui_bg";
            base.Initialize();
        }
    }
}
