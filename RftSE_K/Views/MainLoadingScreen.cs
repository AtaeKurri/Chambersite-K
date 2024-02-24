using Chambersite_K.Views;
using RftSE_K.GameObjects.Bullets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RftSE_K.Views
{
    public class MainLoadingScreen : LoadingScreen
    {
        public MainLoadingScreen()
            : base()
        {
            ViewsToLoad.Add(typeof(TestStage));
            ViewsToLoad.Add(typeof(GameUI));
            AddResourceLoader<BulletResourceLoader>();
        }
    }
}
