using System;
using Mogre;

using Game.Terrain;
using Game.CharacSystem;
using Game.Display;
using Game.BaseApp;

namespace Game
{
    public class Program : BaseApplication
    {

        static void Main()
        {
            new Program().Go();
        }

        protected override void CreateScene()
        {
            CharacMgr CharacMgr = new CharacMgr();
            GraphicBlock.generateFace();

            CharacMgr.addPlayer(new Race(this.mSceneMgr, "Sinbad.mesh"), new CharacterInfo("Sinbad", new Vector3(0, 0, -250)));
            
            LogManager.Singleton.DefaultLog.LogMessage("\n ====================Program Logs====================\n");

            new World(this.mSceneMgr);

            LogManager.Singleton.DefaultLog.LogMessage("\n ====================Program Logs End====================\n");
            //this.mCamera.SetPosition(0, 0, -50);

            
       }
    }
}