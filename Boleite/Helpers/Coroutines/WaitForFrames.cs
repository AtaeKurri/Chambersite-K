using Chambersite_K.GameObjects.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boleite.Helpers.Coroutines
{
    public class WaitForFrames : Coroutine
    {
        protected int FramesToWait { get; private set; } = 1;
        protected int FramesWaited { get; private set; } = 0;

        public WaitForFrames(int time)
            : base()
        {
            FramesToWait = time;
            Routine = waitForFrames();
        }

        IEnumerator waitForFrames()
        {
            while (FramesWaited < FramesToWait)
                yield return null;

            yield break;
        }
    }
}
