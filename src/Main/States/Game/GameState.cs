using System;

using Game.CharacSystem;
using Game.World;
using Game.GUICreator;
using Game.Console;

namespace Game.States
{
    public class GameState : State
    {
        private MainWorld   mWorld;
        private CharacMgr   mCharacMgr;
        private DebugMode   mDebugMode;
        private GameConsole mConsole;

        public GameState(StateManager stateMgr) : base(stateMgr) { }
        public GameConsole GameConsole { get { return this.mConsole; } }

        protected override void Startup()
        {
            this.mConsole = new GameConsole(this.mStateMgr.MiyagiManager, this.mStateMgr.Input, OnCommandEntered);
            this.mWorld = new MainWorld(this.mStateMgr);
            this.mCharacMgr = new CharacMgr(this.mStateMgr, this.mWorld, this.mConsole);

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

        public override void Hide() { this.mConsole.Hide(); }
        public override void Show()
        {
            this.mStateMgr.MiyagiManager.AllGuisVisibility(false);
            this.mStateMgr.MiyagiManager.CursorVisibility = false;
            this.mConsole.Show();
        }

        public override void Update(float frameTime)
        {
            this.mWorld.Update(frameTime);
            this.mConsole.Update();
            this.mDebugMode.IsConsoleMode = this.mConsole.Enable;
            this.mDebugMode.Update(frameTime);

            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(this.mStateMgr.NumberState - 1); }
        }

        protected override void Shutdown()
        {
            Mogre.LogManager.Singleton.DefaultLog.LogMessage(" => Game loop end");
            this.mWorld.Shutdown();
            this.mDebugMode.Dispose();
            this.mConsole.Dispose();
        }

        private void OnCommandEntered(string text)
        {

        }
    }
}
