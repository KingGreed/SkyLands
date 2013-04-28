using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.RTS
{
    public abstract class RTSManager // One RTSManager per faction
    {
        protected int mCrystals;
        private bool mIsPlayer;

        public int Crystals
        {
            get { return this.mCrystals; }
            set
            {
                if (value != this.mCrystals)
                {
                    this.mCrystals = value;
                    if (this.mIsPlayer) { (this as PlayerRTS).HUD.UpdateCrystal(this.mCrystals); }
                }
            }
        }

        public RTSManager(bool isPlayer)
        {
            this.mIsPlayer = isPlayer;
            this.mCrystals = 0;
        }
    }
}
