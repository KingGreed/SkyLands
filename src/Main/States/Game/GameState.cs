using System;

using Game.CharacSystem;
using Game.World;

namespace Game.States
{
    public class GameState : State
    {
        private DebugMode mDebugMode;
        private MainWorld mWorld;
        private CharacMgr mCharacMgr;

        public GameState(StateManager stateMgr) : base(stateMgr) { }

        protected override void Startup()
        {
            this.mWorld = new MainWorld(this.mStateMgr);
            this.mCharacMgr = new CharacMgr(this.mStateMgr.SceneManager, this.mStateMgr.Input, this.mWorld, this.mStateMgr.Camera);
            this.mCharacMgr.AddPlayer("Sinbad.mesh", new CharacterInfo("Sinbad", this.mWorld.getSpawnPoint()));
            this.mDebugMode = new DebugMode(this.mStateMgr.Input, this.mCharacMgr);
            this.Show();
            Mogre.LogManager.Singleton.DefaultLog.LogMessage(" => Game loop begin");
        }

        public override void Hide() { }
        public override void Show()
        {
            this.mStateMgr.MiyagiManager.AllGuisVisibility(false);
            this.mStateMgr.MiyagiManager.CursorVisibility = false;
        }

        public override void Update(float frameTime)
        {
            this.mDebugMode.Update(frameTime);
            this.mWorld.Update();

            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(this.mStateMgr.NumberState - 1); }
        }

        protected override void Shutdown()
        {
            Mogre.LogManager.Singleton.DefaultLog.LogMessage(" => Game loop end");
            this.mWorld.Shutdown();
            this.mStateMgr.SceneManager.ClearScene();
        }
    }
}
