using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chambersite_K.GameObjects
{
    public interface IGameObject
    {
        public void Init();
        public void Frame();
        public void Render();
        public void Delete(); // Deletes the GameObject without doing anything else
        public void Kill(); // Deletes the GameObject with a defined behaviour
    }
}
