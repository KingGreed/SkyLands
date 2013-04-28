using System;

using Miyagi.Common.Events;

using Game.CharacSystem;
using Game.World;
using Game.GUICreator;
using Game.Shoot;
using Game.RTS;

namespace Game.States
{
    public class GameState : State
    {
        private MainWorld     mWorld;
        private CharacMgr     mCharacMgr;
        private BulletManager mBulletMgr;
        private DebugMode     mDebugMode;
        private HUD           mHUD;
        private PlayerRTS     mPlayerRTS; 

        public GameState(StateManager stateMgr) : base(stateMgr, "Game") { }

        protected override void Startup()
        {
            this.mStateMgr.IsInGame = true;
            this.mWorld = new MainWorld(this.mStateMgr);
            this.mBulletMgr = new BulletManager(this.mStateMgr.SceneMgr, this.mWorld);

            this.mHUD = new HUD(this.mStateMgr);
            this.mHUD.IGMenu.SetListener(InGameMenuGUI.ButtonName.Menu, this.ClickMenuButton);
            this.mHUD.IGMenu.SetListener(InGameMenuGUI.ButtonName.Options, this.ClickOptionsButton);
            this.mHUD.IGMenu.SetListener(InGameMenuGUI.ButtonName.Save, this.ClickSaveButton);

            this.mPlayerRTS = new PlayerRTS(this.mHUD);
            this.mCharacMgr = new CharacMgr(this.mStateMgr, this.mWorld, this.mBulletMgr, this.mHUD, this.mPlayerRTS);
            this.mBulletMgr.AttachCharacMgr(this.mCharacMgr);

            CharacterInfo playerInfo = new CharacterInfo("Sinbad", true);
            playerInfo.SpawnPoint = this.mWorld.getSpawnPoint();
            this.mCharacMgr.AddCharacter(playerInfo);

            CharacterInfo iaInfo = new CharacterInfo("Robot-01", false);
            iaInfo.SpawnPoint = playerInfo.SpawnPoint + new Mogre.Vector3(800, 0, 200);

            this.mCharacMgr.AddCharacter(iaInfo);

            this.mDebugMode = new DebugMode(this.mStateMgr.Input, this.mCharacMgr, this.mHUD);
            this.Show();
            Mogre.LogManager.Singleton.DefaultLog.LogMessage(" => Game loop begin");
        }

        public override void Hide()
        {
            this.mHUD.Hide();
            this.mHUD.IGMenu.ShowSaveMessage(false);
        }
        public override void Show()
        {
            this.mStateMgr.HideGUIs();
            this.mStateMgr.MiyagiMgr.CursorVisibility = false;
            this.mHUD.Show();
            this.mDebugMode.IsAllowedToMoveCam = true;
        }

        private void ClickMenuButton(object obj, MouseButtonEventArgs arg)
        {
            this.mStateMgr.RequestStatePop(this.mStateMgr.NumberState - 1);
        }

        private void ClickOptionsButton(object obj, MouseButtonEventArgs arg)
        {
            this.mStateMgr.RequestStatePush(typeof(OptionsState));
        }

        private void ClickSaveButton(object obj, MouseButtonEventArgs arg)
        {
            this.mWorld.getIslandAt(this.mCharacMgr.GetCharacterByListPos().Info.IslandLoc).save();
            this.mHUD.IGMenu.ShowSaveMessage(true);
        }

        public override void Update(float frameTime)
        {
            this.mWorld.Update(frameTime);
            this.mBulletMgr.Update(frameTime);
            this.mDebugMode.IsConsoleMode = this.mStateMgr.MyConsole.Enabled;
            this.mDebugMode.Update(frameTime);

            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_ESCAPE))
            {
                this.mDebugMode.IsAllowedToMoveCam = !this.mHUD.SwitchVisibleIGMenu();
            }
            else if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_E))
            {

            }
        }

        protected override void Shutdown()
        {
            Mogre.LogManager.Singleton.DefaultLog.LogMessage(" => Game loop end");
            this.mHUD.Dispose();
            this.mDebugMode.Dispose();
            this.mWorld.Shutdown();
        }
    }
}
