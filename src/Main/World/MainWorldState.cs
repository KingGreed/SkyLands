using System;
using System.Collections.Generic;
using Mogre;
using CaelumSharp;

using Game.CharacSystem;
using Game.BaseApp;
using Game.States;

namespace Game.World {

    public partial class MainWorld : State
    {

        public override void Hide() { }
        public override void Show() { }

        public override void Update(float frameTime)
        {
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
