using System;
using System.Collections.Generic;
using Mogre;

using Game.CharacSystem;
using Game.BaseApp;
using Game.Display;

namespace Game {

    public partial class World
    {
        public override void Hide() { }
        public override void Show() { }

        public override void Update(float frameTime)
        {
            if (!this.mIsWorldLoaded)
            {
                if (this.mIsWorldGenerated)
                {
                    new DisplayWorld(this.mStateMgr.SceneManager).displayAllChunks();
                    this.mSkyMgr.AddListeners();
                    this.mLoadingBar.Dispose();
                    LogManager.Singleton.DefaultLog.LogMessage("World Displayed");
                    this.mIsWorldLoaded = true;
                }
                else
                {
                    this.mLoadingBar.Value += 100 * frameTime;
                    return;
                }
            }

            this.mDebugMode.Update(frameTime);
            this.mSkyMgr.Update();

            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }    // Return to the MenuState
        }

        public override void Shutdown() {
            if (this.mStateMgr == null) { return; }

            this.mSkyMgr.Shutdown();
            this.mStateMgr.SceneManager.ClearScene();

            this.mIsStartedUp = false;
        }
    }
}
