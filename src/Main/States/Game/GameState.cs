using System;

using Game.CharacSystem;
using Game.World;

namespace Game.States
{
    public class GameState : State
    {
        private MainWorld    mWorld;
        private CharacMgr    mCharacMgr;
        private DebugMode    mDebugMode;

        public GameState(StateManager stateMgr) : base(stateMgr) { }

        protected override void Startup()
        {
            this.mWorld = new MainWorld(this.mStateMgr);

            this.mCharacMgr = new CharacMgr(this.mStateMgr, this.mWorld);
            CharacterInfo info = new CharacterInfo("Sinbad", true);
            info.SpawnPoint = this.mWorld.getSpawnPoint();
            this.mCharacMgr.AddCharacter(info);
            //this.mCharacMgr.AddCharacter(new CharacterInfo("Sinbad", this.mWorld.getSpawnPoint()));   // Add a NPC at the same point
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
            this.mWorld.Update(frameTime);
            this.mDebugMode.Update(frameTime);

            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(this.mStateMgr.NumberState - 1); }
        }

        protected override void Shutdown()
        {
            Mogre.LogManager.Singleton.DefaultLog.LogMessage(" => Game loop end");
            this.mWorld.Shutdown();
            this.mDebugMode.Dispose();
        }
    }
}
