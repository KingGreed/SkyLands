using System;

using Game.CharacSystem;
using Game.World;
using Game.GUICreator;

namespace Game.States
{
    public class GameState : State
    {
        private MainWorld mWorld;
        private CharacMgr mCharacMgr;
        private DebugMode mDebugMode;
        private GameGUI mGUI;

        public GameState(StateManager stateMgr) : base(stateMgr) { }

        protected override void Startup()
        {
            this.mWorld = new MainWorld(this.mStateMgr);
            this.mCharacMgr = new CharacMgr(this.mStateMgr, this.mWorld);
            this.mGUI = new GameGUI(this.mStateMgr);

            CharacterInfo playerInfo = new CharacterInfo("Sinbad", true);
            playerInfo.SpawnPoint = this.mWorld.getSpawnPoint();
            this.mCharacMgr.AddCharacter(playerInfo);

            /*CharacterInfo iaInfo = new CharacterInfo("NPC_01", false);
            iaInfo.SpawnPoint = playerInfo.SpawnPoint;
            this.mCharacMgr.AddCharacter(iaInfo);*/

            this.mDebugMode = new DebugMode(this.mStateMgr.Input, this.mCharacMgr);
            this.Show();
            Mogre.LogManager.Singleton.DefaultLog.LogMessage(" => Game loop begin");
        }

        public override void Hide() { this.mGUI.Hide(); }
        public override void Show()
        {
            this.mStateMgr.HideGUIs();
            this.mStateMgr.MiyagiMgr.CursorVisibility = false;
            this.mGUI.Show();
        }

        public override void Update(float frameTime)
        {
            this.mWorld.Update(frameTime);
            this.mDebugMode.IsConsoleMode = this.mStateMgr.MyConsole.Enable;
            this.mDebugMode.Update(frameTime);

            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(this.mStateMgr.NumberState - 1); }
        }

        protected override void Shutdown()
        {
            Mogre.LogManager.Singleton.DefaultLog.LogMessage(" => Game loop end");
            this.mGUI.Dispose();
            this.mWorld.Shutdown();
            this.mDebugMode.Dispose();
        }
    }
}
