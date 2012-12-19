using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.CharacSystem;
using Game.Terrain;
using Game.States;
using Game.BaseApp;

using Mogre;

namespace Game {

    public partial class World
    {
        public override void Hide() { }
        public override void Show() { }

        public override void Update(float frameTime) {
            this.mCharacMgr.Update(frameTime);

            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }    // Return to the MenuState
        }

        public override void Shutdown() {

            if (this.mStateMgr == null) { return; }

            this.mStateMgr = null;
            this.mIsStartedUp = false;
        }
    }
}
