using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.States;

namespace Game.RTS
{
    public class PlayerRTS : VanillaRTS
    {
        public PlayerRTS(StateManager stateMgr,  RTSManager RTSMgr) : base(stateMgr, RTSMgr)
        {
            this.Faction = Faction.Blue;
        }

        public override void MyUpdate(float frameTime)
        {

            if (this.mTimeSinceInfoUpdate < 1) { return; }
            this.mTimeSinceInfoUpdate = 0;

        }


    }
}
