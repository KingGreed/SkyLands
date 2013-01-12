using System;
using Mogre;

using Game.States;
using Game.BaseApp;

using Game.Display;

namespace Game
{
    public class Program : BaseApplication
    {
        private StateManager mStateMgr;
        
        static void Main()
        {
            new Program().Go();
        }

        protected override void CreateScene()
        {
            GraphicBlock.generateFace();
            LogManager.Singleton.DefaultLog.LogMessage("***********************Program\'s Log***********************");
            this.mStateMgr = new StateManager(this.mRoot, this.mSceneMgr, this.mInput, this.mWindow);
            LogManager.Singleton.DefaultLog.LogMessage("StateMgr created");
            //mStateMgr.Startup(typeof(World));
            this.mStateMgr.Startup(typeof(MainMenu));
       }

        protected override void UpdateScene(FrameEvent evt)
        {
            this.mStateMgr.Update(evt.timeSinceLastFrame);

            if (this.mStateMgr.IsShuttedDown)
            {
                this.Shutdown();
                LogManager.Singleton.DefaultLog.LogMessage("***********************End of Program\'s Log***********************");
            }
        }

        /* We don't use the camera of BaseApplication */
        protected override void CreateCamera() {}

        protected override void CreateViewports() {}

        protected override void ProcessInput()
        {
            this.mInput.Update();

            if (mInput.WasKeyPressed(MOIS.KeyCode.KC_R))     { this.CycleTextureFilteringMode(); }
            if (mInput.WasKeyPressed(MOIS.KeyCode.KC_F5))    { this.ReloadAllTextures(); }
            if (mInput.WasKeyPressed(MOIS.KeyCode.KC_SYSRQ)) { this.TakeScreenshot(); }
        }
    }
}