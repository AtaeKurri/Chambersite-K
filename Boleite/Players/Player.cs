using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chambersite_K.GameObjects;
using Chambersite_K.Views;

namespace Boleite.Players
{
    [InternalName("Player")]
    public class Player : GameObject
    {
        public float Speed { get; set; } = 4.5f;
        public float FocusSpeed { get; set; } = 2.0f;
        public bool Focus { get; set; } = false;
        public bool IsLocked { get; set; } = false;

        public int ProtectTime { get; set; } = 0;
        public List<PlayerSupport> Supports { get; set; } = new List<PlayerSupport>();
        public GameObject? Target { get; set; }

        public int NextShoot { get; set; } = 0;
        public int NextBomb { get; set; } = 0;
        public int NextSpecial { get; set; } = 0;

        public override void Init()
        {
            base.Init();
        }

        public override void Frame()
        {
            base.Frame();
        }

        public void Shoot()
        {

        }

        public void Bomb()
        {

        }

        public void Special()
        {

        }
    }
}
