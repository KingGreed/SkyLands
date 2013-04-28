using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.GUICreator;

namespace Game.RTS
{
    public class PlayerRTS : RTSManager
    {
        private HUD mHUD;

        public HUD HUD { get { return this.mHUD; } }
        
        public PlayerRTS(HUD hud) : base(true)
        {
            this.mHUD = hud;
            this.Crystals = 150;
        }
    }
}
