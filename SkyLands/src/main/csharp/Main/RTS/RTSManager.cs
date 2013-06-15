using Game.CharacSystem;

namespace Game.RTS
{
    public class RTSManager
    {
        public CharacMgr CharacMgr { get; private set; }
        public AIRTS AIRTS { get; private set; }
        public PlayerRTS PlayerRTS { get; private set; }

        public RTSManager(CharacMgr characMgr)
        {
            this.CharacMgr = characMgr;
            this.AIRTS = new AIRTS(this);
            this.PlayerRTS = new PlayerRTS(this);
        }

        public void Update(float frameTime)
        {
            this.AIRTS.Update(frameTime);
            this.PlayerRTS.Update(frameTime);
        }
    }
}
