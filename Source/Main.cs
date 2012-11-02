using Mogre;
using Mogre.TutorialFramework;
using System;

using Game.Land;
using Game.CharacSystem;

namespace Game
{
    class Program : BaseApplication
    {
        public static void Main()
        {
            new Program().Go();
        }

        protected override void CreateScene()
        {
            World world = new World(ref mSceneMgr);

            CharacMgr characMgr = new CharacMgr(ref world);
            characMgr.addPlayer(new Breed(mSceneMgr, "Sinbad.mesh"), new CharacterInfo("Sinbad", new Vector3(5, 15, 5)));
        }
    }
}