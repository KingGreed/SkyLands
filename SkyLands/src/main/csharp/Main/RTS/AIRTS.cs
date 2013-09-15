using System;
using System.Collections.Generic;
using System.Threading;
using Game.States;
using Game.World.Blocks;
using Mogre;

using API.Geo.Cuboid;

namespace Game.RTS {
    public class AIRTS : VanillaRTS
    {
        private struct Circle
        {
            private Vector3 mCenter;
            private float mRadius;
            private int mDeltaAngle;
            private Degree mLastAngle;
            private List<Degree> mPossibleAngles;

            public float Radius { get { return this.mRadius; } }

            public Circle(Vector3 center) : this()
            {
                this.mCenter = center;
                this.mRadius = 0;
                this.mPossibleAngles = new List<Degree> { 0 };
            }

            public Circle(Circle prev, float addRadius) : this()
            {
                this.mCenter = prev.mCenter;
                this.mRadius = prev.mRadius + addRadius;
                this.mPossibleAngles = new List<Degree>();
                int nbAngle = (int)(2 * Mogre.Math.PI * this.mRadius / 10);// 10 is the max building size
                if (nbAngle > 360) { nbAngle = 360; }

                this.mDeltaAngle = nbAngle == 1 ? 0 : 360 / nbAngle;
                for (int i = 0; i < nbAngle; i++)
                    this.mPossibleAngles.Add(new Degree(i * this.mDeltaAngle));
            }

            public Vector3 GetPoint()
            {
                int nbPossibleAngles = this.mPossibleAngles.Count;
                if (nbPossibleAngles <= 0) { return -Vector3.UNIT_SCALE; }

                Random random = new Random();
                int n = nbPossibleAngles == 1 ? 0 : random.Next(0, nbPossibleAngles - 1);
                this.mLastAngle = this.mPossibleAngles[n];
                this.mPossibleAngles.Remove(this.mLastAngle);

                this.mLastAngle = new Degree(this.mLastAngle.ValueDegrees + random.Next(-5, 5));
                return this.mCenter + new Vector3(Mogre.Math.Cos(this.mLastAngle) * this.mRadius, 0, Mogre.Math.Sin(this.mLastAngle) * this.mRadius);
            }

            public void RejectLastAngle()
            {
                int nbAngleToDelete = (int)(57 * Mogre.Math.PI * this.mRadius / (180 * 10));    // delete angles on a range of 57 degree
                for (int i = 0; i < nbAngleToDelete; i++)
                    this.mPossibleAngles.Remove(new Degree(this.mLastAngle.ValueAngleUnits + this.mDeltaAngle * (i - nbAngleToDelete / 2)));
            }
        }

        private API.Geo.World mWorld;
        private Random mRandom;
        private Building mWaitingBuilding;
        private Circle mCircle;
        private Vector3 mBasePos;
        private string mNextBuilding;
        private int mNbUpdateSkipped;
        private float mAverageHeight;

        public int NbBuildingsAllowedToAdd { get; set; }
        public int NbRobotsAllowedToAdd { get; set; }

        public AIRTS(StateManager stateMgr, RTSManager RTSMgr) : base(stateMgr, RTSMgr)
        {
            this.Faction = Faction.Red;
            this.mWorld = this.mStateMgr.MainState.World;

            Vector3 isldSize = this.mWorld.getIsland().getSize() * 16;
            this.mRandom = new Random();
            int x = this.mRandom.Next(7, (int)isldSize.x);
            int z = this.mRandom.Next(7, (int)isldSize.z);
            this.mAverageHeight = this.mWorld.getSurfaceHeight(x, z);
            this.mBasePos = new Vector3(x, this.mAverageHeight, z);

            this.mCircle = new Circle(mBasePos);

            this.mNextBuilding = "";
            this.mNbUpdateSkipped = 0;
            this.NbRobotsAllowedToAdd = 0;
            this.NbBuildingsAllowedToAdd = 4;
        }

        protected override void Update()
        {
            if (this.mWaitingBuilding != null)
            {
                this.mWaitingBuilding.Build();
                this.mNextBuilding = "";
                this.NbBuildingsAllowedToAdd--;
                this.mWaitingBuilding = null;
            }
        }

        protected override void StepUpdate()
        {
            if (this.mNbUpdateSkipped++ < 6) { return; }
            this.mNbUpdateSkipped = 0;

            int nbRobotTocreate = 0;
            if (this.AmountUnits < 3) { nbRobotTocreate = 3 - this.AmountUnits; }
            if (this.NbRobotsAllowedToAdd > nbRobotTocreate) { nbRobotTocreate = this.NbRobotsAllowedToAdd; }
            this.NbRobotsAllowedToAdd = nbRobotTocreate - this.CreateRobot(nbRobotTocreate);

            if (this.mWaitingBuilding == null && this.NbBuildingsAllowedToAdd > 0)
                new Thread(this.ChooseBuilding).Start();

            /*foreach (VanillaNonPlayer npc in this.mRTSMgr.CharacMgr.GetFactionCharacters(this.Faction))
            {
                
            }*/
        }

        private void ChooseBuilding()
        {
            if (this.mNextBuilding == "")
            {
                if (!this.Buildings.Exists(b => b.Selection == "HQ")) { this.mNextBuilding = "HQ"; }
                else if (this.mRTSMgr.PlayerRTS.CrystalSpeed > 1.3 * this.CrystalSpeed || this.CrystalSpeed == 5) { this.mNextBuilding = "CD"; }
                else if (this.mRTSMgr.PlayerRTS.Capacity > 1.3 * this.Capacity || this.Capacity == 0) { this.mNextBuilding = "G"; }
                else if (this.mRTSMgr.PlayerRTS.NbFactory > 1.3 * this.NbFactory || this.NbFactory == 0) { this.mNextBuilding = "RF"; }
                else { mNextBuilding = new string[] { "CD", "G", "RF" }[this.mRandom.Next(0, 3)]; }
            }

            Island island = this.mWorld.getIsland();
            switch (this.mNextBuilding)
            {
                case "HQ":
                    this.mWaitingBuilding = new HeadQuarter(this.mStateMgr, island, this, this.GetBuildPos(HeadQuarter.sizeX, HeadQuarter.sizeZ));
                    break;
                case "CD":
                    this.mWaitingBuilding = new CrystalDrill(this.mStateMgr, island, this, this.GetBuildPos(CrystalDrill.sizeX, CrystalDrill.sizeZ));
                    break;
                case "RF":
                    this.mWaitingBuilding = new RobotFactory(this.mStateMgr, island, this, this.GetBuildPos(RobotFactory.sizeX, RobotFactory.sizeZ));
                    break;
                case "G":
                    this.mWaitingBuilding = new Generator(this.mStateMgr, island, this, this.GetBuildPos(Generator.sizeX, Generator.sizeZ));
                    break;
            }
        }

        private Vector3 GetBuildPos(int sizeX, int sizeZ) {
            Vector3 pos;
            bool posAccepted;

            do {
                do {    // Get the point on the current circle
                    pos = this.mCircle.GetPoint();
                    posAccepted = pos != -Vector3.UNIT_SCALE;
                    if (!posAccepted)
                        this.mCircle = new Circle(this.mCircle, this.mCircle.Radius == 0 ? 12 : 5 + this.mRandom.Next(0, 6));
                } while (!posAccepted);

                pos.x = (float)System.Math.Round(pos.x);
                pos.y = (float)System.Math.Round(this.mAverageHeight);
                pos.z = (float)System.Math.Round(pos.z);

                Island island = this.mWorld.getIsland();
                int diffY = 0;
                Vector3 pDown = pos;
                pDown.y--;
                Block posBlock = island.getBlock(pos, false);
                Block downBlock = island.getBlock(pDown, false);

                /* Position the building on the floor */
                while ((downBlock is Air || !(posBlock is Air)) && diffY > -10) {
                    pos.y--;
                    pDown.y--;
                    diffY--;
                    posBlock = downBlock;
                    downBlock = island.getBlock(pDown, false);
                }

                if (diffY <= -10) {
                    pos.y += 10;
                    pDown.y += 10;
                    diffY = 0;
                    posBlock = island.getBlock(pos, false);
                    do {
                        pos.y++;
                        pDown.y++;
                        diffY++;
                        downBlock = posBlock;
                        posBlock = island.getBlock(pos, false);
                    } while ((downBlock is Air || !(posBlock is Air)) && diffY < 10);
                    posAccepted = diffY < 10;
                }

                /* Test if the building is in the bounds of the terrain */
                if (posAccepted) {
                    Vector3 islandSize = island.getSize() * 16;
                    Vector2 halfSize = new Vector2(Mogre.Math.Ceil(sizeX / 2f), Mogre.Math.Ceil(sizeZ / 2f));
                    Vector2 min = new Vector2(pos.x - halfSize.x, pos.z - halfSize.y);
                    Vector2 max = new Vector2(pos.x + halfSize.x, pos.z + halfSize.y);
                    if (min.x < 0 || min.y < 0 || max.x > islandSize.x || max.y > islandSize.z)
                        posAccepted = false;
                }

                if (!posAccepted) { this.mCircle.RejectLastAngle(); }   // Delete the angles near the rejected one

            } while (!posAccepted);

            this.mAverageHeight += (pos.y - this.mAverageHeight) / (this.Buildings.Count + 1);  // the building hasn't been added yet

            return pos;
        }
    }
}
