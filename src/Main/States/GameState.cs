using System;
using System.Collections.Generic;
using Mogre;
using CaelumSharp;

using Game.CharacSystem;
using Game.BaseApp;
using Game.World;

namespace Game.States {

    public class GameState : State
    {
        private DebugMode mDebugMode;
        private MainWorld mWorld;
        private CharacMgr mCharacMgr;

        public GameState(StateManager stateMgr) : base(stateMgr) { }

        protected override void Startup()
        {
            this.mWorld = new MainWorld(this.mStateMgr);
            this.mCharacMgr = new CharacMgr(this.mStateMgr.Camera);
            this.mCharacMgr.AddPlayer(this.mStateMgr.SceneManager, "Sinbad.mesh",
                                      new CharacterInfo("Sinbad", this.mWorld.getSpawnPoint()),
                                      this.mStateMgr.Input, this.mWorld);
            this.mDebugMode = new DebugMode(this.mStateMgr.Input, this.mCharacMgr);
        }

        public override void Hide() { }
        public override void Show() { }

        public override void Update(float frameTime)
        {
            this.mDebugMode.Update(frameTime);
            this.mWorld.Update();

            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }    // Return to the MenuState
        }

        protected override void Shutdown() {
            this.mWorld.Shutdown();
            this.mStateMgr.SceneManager.ClearScene();
        }
    }
}
