using System.Collections.Generic;
using API.Generic;
using Game.CharacSystem;
using Game.States;
using Mogre;

namespace Game.RTS
{
    public abstract class VanillaRTS
    {
        protected StateManager mStateMgr;
        protected RTSManager mRTSMgr;
        protected float mCrystals;
        protected float mTimeSinceInfoUpdate;
        public float CrystalSpeed { get; set; }
        public int Capacity { get; set; }
        public int AmountUnits { get; set; }

        public Faction Faction { get; protected set; }
        public List<Building> Buildings { get; protected set; }

        public float Crystals
        {
            get { return this.mCrystals; }
            set { this.mCrystals = value; }
        }

        protected VanillaRTS(StateManager stateMgr, RTSManager RTSMgr)
        {
            this.mStateMgr = stateMgr;
            this.mRTSMgr = RTSMgr;
            this.Buildings = new List<Building>();
            this.mCrystals = 150;
            this.CrystalSpeed = 1 / 70;
        }

        public void Update(float frameTime)
        {
            this.mTimeSinceInfoUpdate += frameTime;
            this.mCrystals += this.CrystalSpeed * frameTime;

            this.MyUpdate(frameTime);
        }

        public abstract void MyUpdate(float frameTime);

        public void AddBuilding(Building b) {  this.Buildings.Add(b); }
    }
}
