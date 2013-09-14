using Game.States;

namespace Game.RTS
{
    public class PlayerRTS : VanillaRTS
    {
        public PlayerRTS(StateManager stateMgr,  RTSManager RTSMgr) : base(stateMgr, RTSMgr)
        {
            this.Faction = Faction.Blue;
        }

        protected override void Update()
        {

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
