using System;

using Miyagi.Common.Events;

using Game.CharacSystem;
using Game.World;
using Game.GUICreator;
using Game.Shoot;

namespace Game.States
{
    public class GameState : State
    {
        private MainWorld     mWorld;
        private CharacMgr     mCharacMgr;
        private BulletManager mBulletMgr;
        private DebugMode     mDebugMode;
        private GameGUI       mGUI;

        public MainWorld World { get { return this.mWorld; } }

        public GameState(StateManager stateMgr) : base(stateMgr, "Game") { }

        protected override void Startup()
        {
            this.mWorld = new MainWorld(this.mStateMgr);
            this.mBulletMgr = new BulletManager(this.mStateMgr.SceneMgr, this.mWorld);
            this.mCharacMgr = new CharacMgr(this.mStateMgr, this.mWorld, this.mBulletMgr);
            this.mBulletMgr.AttachCharacMgr(this.mCharacMgr);
            this.mGUI = new GameGUI(this.mStateMgr);
            this.mGUI.IGMenu.SetListenerMenu(this.ClickMenuButton);
            this.mGUI.IGMenu.SetListenerOption(this.ClickOptionButton);

            CharacterInfo playerInfo = new CharacterInfo("Sinbad", true);
            playerInfo.SpawnPoint = this.mWorld.getSpawnPoint();
            this.mCharacMgr.AddCharacter(playerInfo);


            CharacterInfo iaInfo = new CharacterInfo("Robot-01", false);
            iaInfo.SpawnPoint = playerInfo.SpawnPoint + new Mogre.Vector3(800, 0, 200);

            this.mCharacMgr.AddCharacter(iaInfo);

            this.mDebugMode = new DebugMode(this.mStateMgr.Input, this.mCharacMgr, this.mGUI);
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

        private void ClickMenuButton(object obj, MouseButtonEventArgs arg)
        {
            this.mStateMgr.RequestStatePop(this.mStateMgr.NumberState - 1);
        }
        private void ClickOptionButton(object obj, MouseButtonEventArgs arg)
        {
            this.mStateMgr.RequestStatePush(typeof(OptionsState));
        }

        public override void Update(float frameTime)
        {
            this.mWorld.Update(frameTime);
            this.mBulletMgr.Update(frameTime);
            this.mDebugMode.IsConsoleMode = this.mStateMgr.MyConsole.Enabled;
            this.mDebugMode.Update(frameTime);

            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_ESCAPE))
            {
                this.mDebugMode.IsAllowedToMoveCam = !this.mGUI.SwitchVisibleIGMenu();
            }
            else if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_E))
            {

            }
        }

        protected override void Shutdown()
        {
            Mogre.LogManager.Singleton.DefaultLog.LogMessage(" => Game loop end");
            this.mGUI.Dispose();
            this.mDebugMode.Dispose();
            this.mWorld.Shutdown();
        }
    }
}
