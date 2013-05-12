using Game.CharacSystem;
using Game.GUICreator;
using Game.Shoot;
using Game.RTS;

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

            CharacterInfo playerInfo = new CharacterInfo("Sinbad", true) { SpawnPoint = this.mWorld.getSpawnPoint() };
            this.CharacMgr.AddCharacter(playerInfo);

            CharacterInfo iaInfo = new CharacterInfo("Robot-01")
            {
                SpawnPoint = playerInfo.SpawnPoint + new Mogre.Vector3(800, 100, 200)
            };
            this.CharacMgr.AddCharacter(iaInfo);

            //ParticleGenerator.mkParticle(this.mStateMgr.SceneMgr, iaInfo.SpawnPoint, "MultiEmitters");
        }

        protected override void AfterWorldCreation()
        {
            this.MainGUI = new HUD(this.mStateMgr);

            this.mPlayerRTS = new PlayerRTS((HUD)this.MainGUI);
            this.mBulletMgr = new BulletManager(this.mStateMgr.SceneMgr, this.mWorld);
            this.CharacMgr = new CharacMgr(this.mStateMgr, this.mWorld, this.mBulletMgr, this.User);
            this.mBulletMgr.AttachCharacMgr(this.CharacMgr);
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);
            
            this.mBulletMgr.Update(frameTime);

            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_F1))
                this.User.SwitchFreeCamMode();
            else if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_E))
            {

            }
            if (this.mStateMgr.Input.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Middle))
                this.CharacMgr.MainPlayer.setIsPushedByArcaneLevitator(!this.CharacMgr.MainPlayer.MovementInfo.IsPushedByArcaneLevitator);

        }

        public override void Save() { this.mWorld.save(this.CharacMgr.MainPlayer); }
    }
}
