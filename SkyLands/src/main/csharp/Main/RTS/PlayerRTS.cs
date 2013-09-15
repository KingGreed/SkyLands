using Game.CharacSystem;
using Game.GUIs;
using Game.States;

namespace Game.RTS
{
    public class PlayerRTS : VanillaRTS
    {
        public PlayerRTS(StateManager stateMgr,  RTSManager RTSMgr) : base(stateMgr, RTSMgr)
        {
            this.Faction = Faction.Blue;
        }

        protected override void StepUpdate()
        {
            string[] input = new string[3];
            input[0] = this.Crystals.ToString();
            input[1] = ((int)this.mStateMgr.MainState.CharacMgr.MainPlayer.Info.Life) + " / " + VanillaPlayer.DEFAULT_PLAYER_LIFE;
            input[2] = this.AmountUnits + " / " + this.Capacity;
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
