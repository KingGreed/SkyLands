using Game.CharacSystem;
using Game.States;

namespace Game.RTS
{
    public enum Faction { Blue, Red }

    public class RTSManager
    {
        public StateManager StateMgr { get; private set; }
        public AIRTS AIRTS { get; private set; }
        public PlayerRTS PlayerRTS { get; private set; }

        public RTSManager(StateManager stateMgr)
        {
            this.StateMgr = stateMgr;
            this.AIRTS = new AIRTS(this.StateMgr, this);
            this.PlayerRTS = new PlayerRTS(this.StateMgr, this);
        }

        public void Update(float frameTime)
        {
            this.AIRTS.Update(frameTime);
            this.PlayerRTS.Update(frameTime);
        }
    }
}
