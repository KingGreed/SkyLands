using System;
using Mogre;

using Game.States;
using Game.BaseApp;


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
            mStateMgr = new StateManager(mSceneMgr, mInput, mWindow);
            mStateMgr.Startup(typeof(GameState));
       }

        protected override void UpdateScene(FrameEvent evt)
        {
            this.mStateMgr.Update(evt.timeSinceLastFrame);
        }

        /* We don't use the camera of BaseApplication */
        protected override void CreateCamera() {}

        protected override void CreateViewports() {}

        protected override void ProcessInput()
        {
            this.mInput.Update();

            if (mInput.WasKeyPressed(MOIS.KeyCode.KC_R)) { this.CycleTextureFilteringMode(); }
            if (mInput.WasKeyPressed(MOIS.KeyCode.KC_F5)) { this.ReloadAllTextures(); }
            if (mInput.WasKeyPressed(MOIS.KeyCode.KC_SYSRQ)) { this.TakeScreenshot(); }
            if (mInput.WasKeyPressed(MOIS.KeyCode.KC_ESCAPE)) { this.Shutdown(); }
        }

        protected override void DestroyScene()
        {
            this.mStateMgr.Shutdown();
        }
    }
}