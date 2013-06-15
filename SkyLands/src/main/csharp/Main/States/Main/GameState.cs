using System.Windows.Forms;
using Game.Input;
using Mogre;

using Game.CharacSystem;
using Game.Shoot;
using Game.RTS;
using Game.World.Display;

namespace Game.States
{
    public class GameState : MainState
    {
        private BulletManager mBulletMgr;
        private PlayerRTS     mPlayerRTS;

        public GameState(StateManager stateMgr) : base(stateMgr, "Game") { }

        protected override void Startup()
        {
            base.Startup();

            CharacterInfo playerInfo = new CharacterInfo("Sinbad", Faction.Blue, true) { SpawnPoint = this.mWorld.getSpawnPoint() };
            this.CharacMgr.AddCharacter(playerInfo);
            this.User.SwitchFreeCamMode();

            this.CharacMgr.AddCharacter(new CharacterInfo("Robot-01", Faction.Red)
            {
                SpawnPoint = playerInfo.SpawnPoint + new Vector3(800, 500, 200)
            });

            ParticleGenerator.mkParticle(this.mStateMgr.SceneMgr, this.mWorld.getSpawnPoint(), "MultiEmitters");
        }

        protected override void AfterWorldCreation()
        {
            this.mPlayerRTS = new PlayerRTS();
            this.mBulletMgr = new BulletManager(this.mStateMgr.SceneMgr, this.mWorld);
            this.CharacMgr = new CharacMgr(this.mStateMgr, this.mWorld, this.User, this.mBulletMgr);
            this.mBulletMgr.AttachCharacMgr(this.CharacMgr);
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);
            
            this.mBulletMgr.Update(frameTime);

            User user = this.mStateMgr.MainState.User;
            VanillaPlayer mainPlayer = this.mStateMgr.MainState.CharacMgr.MainPlayer;
            if (user.IsFreeCamMode && mainPlayer != null && !user.IsGUIOpen)
            {
                bool ctrlPressed = this.mStateMgr.Controller.IsKeyDown(MOIS.KeyCode.KC_LCONTROL);
                mainPlayer.SetIsAllowedToMove(ctrlPressed, false);
                user.IsAllowedToMoveCam = !ctrlPressed;
            }

            if (this.mStateMgr.Controller.WasKeyPressed(MOIS.KeyCode.KC_F1) || this.mStateMgr.Controller.HasActionOccured(UserAction.Dance))
                user.SwitchFreeCamMode();
        }

        public override void Save() { this.mWorld.save(this.CharacMgr.MainPlayer); }
    }
}
