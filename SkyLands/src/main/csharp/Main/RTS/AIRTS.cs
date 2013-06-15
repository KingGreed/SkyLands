using System.Collections.Generic;

using Game.CharacSystem;

namespace Game.RTS
{
    public class AIRTS : VanillaRTS
    {
        public AIRTS(RTSManager RTSMgr) : base(RTSMgr)
        {
            this.Faction = Faction.Red;
        }

        public override void MyUpdate()
        {
            
            
            /*foreach (VanillaNonPlayer npc in this.mRTSMgr.CharacMgr.GetFactionCharacters(this.Faction))
            {
                
            }*/
        }
    }
}
