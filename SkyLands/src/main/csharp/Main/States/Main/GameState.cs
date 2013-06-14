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

            if (this.mStateMgr.Controller.WasKeyPressed(Keys.F1))
                this.User.SwitchFreeCamMode();
        }

        public override void Save() { this.mWorld.save(this.CharacMgr.MainPlayer); }
    }
}
