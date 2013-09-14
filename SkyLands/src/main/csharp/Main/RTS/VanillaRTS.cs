using System.Collections.Generic;
using System.Linq;
using Game.States;

namespace Game.RTS
{
    public abstract class VanillaRTS
    {
        protected StateManager mStateMgr;
        protected RTSManager mRTSMgr;
        protected float mCrystals;
        protected float mTimeSinceInfoUpdate;
        public int CrystalSpeed { get; set; }
        public int Capacity { get; set; }
        public int AmountUnits { get; set; }
        public int NbFactory { get; set; }

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
            this.CrystalSpeed = 0;
        }

        public void Update(float frameTime)
        {
            this.mTimeSinceInfoUpdate += frameTime;
            if (this.mTimeSinceInfoUpdate < 1) { return; }
            this.mTimeSinceInfoUpdate = 0;

            this.mCrystals += this.CrystalSpeed;
            this.Update();
        }

        protected abstract void Update();

        public virtual void AddBuilding(Building b) { this.Buildings.Add(b); }

        public virtual int CreateRobot(int nb) // Try to create nb robots if there are enough factories available (nb = -1, crate the max). Return the actual number of robot created
        {
            if (this.Crystals < RobotFactory.ROBOT_COST || this.AmountUnits >= this.Capacity) { return 0; }

            RobotFactory[] factories = this.Buildings.OfType<RobotFactory>().ToArray();
            if (nb == -1) { nb = factories.Count(); }
            int nbRobotCreated = 0;
            for (int i = 0; i < factories.Count() && nbRobotCreated < nb; i++)
            {
                if (factories[i].CanCreateUnit)
                {
                    factories[i].CreateUnit();
                    nbRobotCreated++;
                }
            }

            return nbRobotCreated;
        }
    }
}
