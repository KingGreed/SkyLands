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
        CharacMgr mCharacMgr;

        static void Main()
        {
            new Program().Go();
        }

        protected override void CreateScene()
        {
            
            LogManager.Singleton.DefaultLog.LogMessage("\n ====================Program Logs====================\n");


            GraphicBlock.generateFace();

            new World(this.mSceneMgr);
            this.mCamera.FarClipDistance = 3000;
            //this.mCamera.SetPosition(0, 0, -50);

            this.mCharacMgr = new CharacMgr();
            this.mCharacMgr.AddPlayer(new Race(this.mSceneMgr, "Sinbad.mesh"), new CharacterInfo("Sinbad", new Vector3(0, 0, -250)));

            LogManager.Singleton.DefaultLog.LogMessage("\n ====================Program Logs End====================\n");

       }

        protected override void UpdateScene(FrameEvent evt)
        {
            this.mCharacMgr.Update(evt.timeSinceLastFrame);
        }
    }
}