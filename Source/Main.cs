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
<<<<<<< HEAD
=======
            World world = new World(ref mSceneMgr);
>>>>>>> 5c3cc93d3dff01cb6a4c43c92d95002796543de9

            CharacMgr characMgr = new CharacMgr(ref world);
            characMgr.addPlayer(new Breed(mSceneMgr, "Sinbad.mesh"), new CharacterInfo("Sinbad", new Vector3(5, 15, 5)));
        }
    }
}