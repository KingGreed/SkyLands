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


    }
}
