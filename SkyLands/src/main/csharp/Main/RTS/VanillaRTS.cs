using System.Collections.Generic;

namespace Game.RTS
{
    public enum Faction { Blue, Red }

    public abstract class VanillaRTS
    {
        protected RTSManager mRTSMgr;
        protected List<Building> mBuildings;
        protected float mCrystals, mCrystalSpeed;
        protected float mTimeSinceInfoUpdate;

        public Faction Faction { get; protected set; }

        public float Crystals
        {
            get { return this.mCrystals; }
            set { this.mCrystals = value; }
        }

        protected VanillaRTS(RTSManager RTSMgr)
        {
            this.mRTSMgr = RTSMgr;
            this.mBuildings = new List<Building>();
            this.mCrystals = 150;
            this.mCrystalSpeed = 1 / 70;
        }

        public void Update(float frameTime)
        {
            this.mTimeSinceInfoUpdate += frameTime;
            this.mCrystals += this.mCrystalSpeed * frameTime;

            if (this.mTimeSinceInfoUpdate < 1000) { return; }


            this.MyUpdate();
            this.mTimeSinceInfoUpdate = 0;
        }

        public abstract void MyUpdate();

        public void AddBuilding()
        {
            
        }
    }
}
