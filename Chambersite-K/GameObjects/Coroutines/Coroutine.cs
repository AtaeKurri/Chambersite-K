using Chambersite_K;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Chambersite_K.GameObjects.Coroutines
{
    public class Coroutine
    {
        public IEnumerator Routine { get; set; }
        public bool IsFinished { get; set; }

        public Coroutine() { }

        public Coroutine(IEnumerator routine)
        {
            Routine = routine;
        }
    }
}
