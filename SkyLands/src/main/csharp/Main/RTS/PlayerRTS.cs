using Game.CharacSystem;
using Game.GUIs;
using Game.States;

namespace Game.RTS
{
    public class PlayerRTS : VanillaRTS
    {
        private string mLastCrystal, mLastCapacity;
        public PlayerRTS(StateManager stateMgr,  RTSManager RTSMgr) : base(stateMgr, RTSMgr)
        {
            this.Faction = Faction.Blue;
        }

        protected override void StepUpdate()
        {
            this.mLastCrystal = this.Crystals.ToString();
            this.mLastCapacity = this.AmountUnits + " / " + this.Capacity;
            this.UpdateLife();
        }

        public void UpdateLife()
        {
            string[] input = new string[3];
            input[0] = this.mLastCrystal;
            input[1] = ((int)this.mStateMgr.MainState.CharacMgr.MainPlayer.Info.Life) + " / " + VanillaPlayer.DEFAULT_PLAYER_LIFE;
            input[2] = this.mLastCapacity;
            Hud.SetInput(input);
        }

        public override void AddBuilding(Building b)
        {
            base.AddBuilding(b);
            this.mRTSMgr.AIRTS.NbBuildingsAllowedToAdd++;
        }

        public override int CreateRobot(int nb = -1)
        {
            int nbCreated = base.CreateRobot(nb);
            this.mRTSMgr.AIRTS.NbRobotsAllowedToAdd += nbCreated;
            return nbCreated;
        }
    }
}
