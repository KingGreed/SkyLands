using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.RTS
{
    public class PlayerRTS : RTSManager
    {
        
        public PlayerRTS() : base(true)
        {
            this.Crystals = 150;
        }
    }
}
