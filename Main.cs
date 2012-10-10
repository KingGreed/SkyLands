using Mogre;
using Mogre.TutorialFramework;
using System;



namespace Game
{
    class Main : BaseApplication
    {
        public static void Main()
        {
            new Main().Go();
        }

        protected override void CreateScene()
        {
            mSceneMgr.AmbientLight = new ColourValue(1, 1, 1);
            
            
        }
    }
}