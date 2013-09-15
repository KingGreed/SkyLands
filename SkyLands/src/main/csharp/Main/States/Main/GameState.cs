using System.Media;
using Game.GUIs;
using Game.csharp.Main.RTS.Buildings;
using Mogre;

using Game.BaseApp;
using Game.Input;
using Game.CharacSystem;
using Game.Shoot;
using Game.RTS;
using Game.World.Display;
using Game.World;

namespace Game.States
{
    public class GameState : MainState
    {
        private BulletManager mBulletMgr;

        public RTSManager RTSManager { get; private set; }
        public GameState(StateManager stateMgr) : base(stateMgr, "Game") { }

        protected override void Startup()
        {
            this.World = new MainWorld(this.mStateMgr);
            this.World.setSafeSpawnPoint();

            base.Startup();

            CharacterInfo playerInfo = new CharacterInfo("Sinbad", this.RTSManager.PlayerRTS, true) { SpawnPoint = this.World.getSpawnPoint() };
            this.CharacMgr.AddCharacter(playerInfo);

            CharacterInfo ally1 = new CharacterInfo("Robot01", this.RTSManager.PlayerRTS) { SpawnPoint = this.World.getSpawnPoint() - Vector3.UNIT_X * 5 };
            this.CharacMgr.AddCharacter(ally1);

            CharacterInfo ally2 = new CharacterInfo("Robot02", this.RTSManager.PlayerRTS) { SpawnPoint = this.World.getSpawnPoint() + Vector3.UNIT_X * 5 };
            this.CharacMgr.AddCharacter(ally2);

            this.User.SwitchFreeCamMode();
        }

        protected override void AfterWorldCreation()
        {
            this.RTSManager = new RTSManager(this.mStateMgr);
            this.RTSMgr = this.RTSManager;
            this.mBulletMgr = new BulletManager(this.mStateMgr.SceneMgr, this.World);
            this.CharacMgr = new CharacMgr(this.mStateMgr, this.World, this.User, this.mBulletMgr);
            this.mBulletMgr.AttachCharacMgr(this.CharacMgr);
            this.BuildingMgr = new BuildingManager(this.mStateMgr, this.World.getIsland(), this.RTSManager);
            this.User.BuildingMgr = this.BuildingMgr;
            this.User.PlayerRTS = this.RTSManager.PlayerRTS;
        }

        public override void Show()
        {
            base.Show();

            if (this.mStateMgr.GameInfo.Type == TypeWorld.Plains)
                this.mStateMgr.SoundPlayer.SoundLocation = "Media/sounds/dark castle.wav";
            else if (this.mStateMgr.SoundPlayer.SoundLocation == "")
                this.mStateMgr.SoundPlayer.SoundLocation = "Media/sounds/Desert Theme.wav";
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            this.RTSManager.Update(frameTime);
            this.mBulletMgr.Update(frameTime);

            User user = this.mStateMgr.MainState.User;
            VanillaPlayer mainPlayer = this.mStateMgr.MainState.CharacMgr.MainPlayer;
            if (user.IsFreeCamMode && mainPlayer != null && !GUI.Visible)
            {
                bool ctrlPressed = this.mStateMgr.Controller.IsKeyDown(MOIS.KeyCode.KC_LCONTROL);
                mainPlayer.SetIsAllowedToMove(ctrlPressed, false);
                user.IsAllowedToMoveCam = !ctrlPressed;
            }

            if (this.mStateMgr.Controller.WasKeyPressed(MOIS.KeyCode.KC_F1) && !GUI.Visible)
                user.SwitchFreeCamMode();
        }

        public override void Save() { this.World.save(this.CharacMgr.MainPlayer); }

        public override void OpenMainGUI()
        {
            InGameMenu.Save = this.Save;
            new InGameMenu(this.mStateMgr);
        }

        /*protected override void Shutdown()
        {
            base.Shutdown();
            this.mSoundPlayer.Stop();
        }*/
    }
}
