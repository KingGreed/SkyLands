using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.RTS
{
    public class PlayerRTS : VanillaRTS
    {
        public PlayerRTS(RTSManager RTSMgr) : base(RTSMgr)
        {
            this.Faction = Faction.Blue;
        }

        public override void MyUpdate()
        {
        }


    }
}
