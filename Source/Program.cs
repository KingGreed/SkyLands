using Mogre;
using Mogre.TutorialFramework;
using System;

using Game.Terrain;
using Game.CharacSystem;
using Game.Display;

namespace Game
{
    class Program : BaseApplication
    {
        private const int CUBE_SIDE = 10;   // Small CUBE_SIZE since Sinbad.mesh is small as well
        public static void Main()
        {
            new Program().Go();
        }

        protected override void CreateScene()
        {
            //this.mSceneMgr.RootSceneNode.CreateChildSceneNode().AttachObject(this.mSceneMgr.CreateEntity("truc", "cube.mesh"));

            GraphicBlock.generateFace();
            LogManager.Singleton.DefaultLog.LogMessage("\n ====================Program Logs====================\n");
            new World(ref this.mSceneMgr);

            LogManager.Singleton.DefaultLog.LogMessage("\n ====================Program Logs End====================\n");
            this.mCamera.SetPosition(0, 0, -50);

            
       }
    }
}