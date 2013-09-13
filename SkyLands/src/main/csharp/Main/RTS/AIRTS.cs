using System;
using System.Collections.Generic;
using Game.States;
using Game.World.Blocks;
using Mogre;

using API.Geo.Cuboid;

namespace Game.RTS {
    public class AIRTS : VanillaRTS {
        private struct Circle {
            private Vector3 mCenter;
            private float mRadius, mAngleMin, mAngleMax;
            private List<Degree> mAngles;

            public Circle(Vector3 center, float angleMin, float angleMax)
                : this() {
                this.mCenter = center;
                this.mRadius = 0;
                this.mAngleMin = angleMin;
                this.mAngleMax = angleMax;
                this.mAngles = new List<Degree> { 0 };
            }

            public Circle(Circle prev, float addRadius)
                : this() {
                this.mCenter = prev.mCenter;
                this.mRadius = prev.mRadius + addRadius;
                this.mAngleMin = prev.mAngleMin;
                this.mAngleMax = prev.mAngleMax;
                this.mAngles = new List<Degree>();
                int nbAngle = (int)((this.mAngleMax - this.mAngleMin) * Mogre.Math.PI * this.mRadius / (180 * 10));

                float deltaAngle = nbAngle == 1 ? 0 : (this.mAngleMax - this.mAngleMin) / (nbAngle - 1);
                for (int i = 0; i < nbAngle; i++)
                    this.mAngles.Add(new Degree(this.mAngleMin + i * deltaAngle));
            }

            public Vector3 GetPoint() {
                if (this.mAngles.Count <= 0) { return Vector3.ZERO; }

                Random random = new Random();
                int n = random.Next(0, this.mAngles.Count - 1);
                Degree angle = this.mAngles[n];
                this.mAngles.Remove(angle);

                angle = new Degree(angle.ValueDegrees + random.Next(-5, 5));
                return this.mCenter + new Vector3(Mogre.Math.Cos(angle) * this.mRadius, 0, Mogre.Math.Sin(angle) * this.mRadius);
            }
        }

        private API.Geo.World mWorld;
        private Circle mCircle;
        private Vector3 mBasePos;
        private Random mRandom;
        private string mNextBuilding;
        private int mNbUpdateSkipped;

        public int NbBuildingsAllowedToAdd { get; set; }
        public int NbRobotsAllowedToAdd { get; set; }

        public AIRTS(StateManager stateMgr, RTSManager RTSMgr)
            : base(stateMgr, RTSMgr) {
            this.Faction = Faction.Red;
            this.mWorld = this.mStateMgr.MainState.World;
            Vector3 isldSize = this.mWorld.getIsland().getSize() * 16;
            Vector2[] corner = new Vector2[] { Vector2.ZERO, new Vector2(0, isldSize.z - 1),
                new Vector2(isldSize.x - 1, 0), new Vector2(isldSize.x - 1, isldSize.z - 1) };

            this.mRandom = new Random();
            int coinId = 0;// this.mRandom.Next(0, 4);
            float angleMin = coinId * 90;
            float angleMax = (coinId + 1) * 90;
            Console.WriteLine("coinId : " + coinId + "\ncorner : " + corner[coinId]);

            Vector2 coin = corner[coinId];
            int y = this.mWorld.getSurfaceHeight((int)coin.x, (int)coin.y);
            this.mBasePos = new Vector3(coin.x + Mogre.Math.Cos(new Degree(angleMin)) * 8, y, coin.y + Mogre.Math.Cos(new Degree(angleMax)) * 8);
            Console.WriteLine("basePos : " + mBasePos);

            this.mCircle = new Circle(mBasePos, angleMin, angleMax);

            this.mNextBuilding = "";
            this.mNbUpdateSkipped = 0;
            this.NbRobotsAllowedToAdd = 0;
            this.NbBuildingsAllowedToAdd = 4;
        }

        protected override void Update() {
            if (this.mNbUpdateSkipped++ < 30) { return; }
            this.mNbUpdateSkipped = 0;

            int nbRobotTocreate = 0;
            if (this.AmountUnits < 3) { nbRobotTocreate = 3 - this.AmountUnits; }
            if (this.NbRobotsAllowedToAdd > nbRobotTocreate) { nbRobotTocreate = this.NbRobotsAllowedToAdd; }
            this.NbRobotsAllowedToAdd = nbRobotTocreate - this.CreateRobot(nbRobotTocreate);

            if (this.NbBuildingsAllowedToAdd <= 0) { return; }
            if (this.mNextBuilding == "") {
                if (!this.Buildings.Exists(b => b.Selection == "HQ")) { this.mNextBuilding = "HQ"; }
                else if (this.mRTSMgr.PlayerRTS.CrystalSpeed > 1.3 * this.CrystalSpeed || this.CrystalSpeed == 5) { this.mNextBuilding = "CD"; }
                else if (this.mRTSMgr.PlayerRTS.Capacity > 1.3 * this.Capacity || this.Capacity == 0) { this.mNextBuilding = "G"; }
                else if (this.mRTSMgr.PlayerRTS.NbFactory > 1.3 * this.NbFactory || this.NbFactory == 0) { this.mNextBuilding = "RF"; }
                else { mNextBuilding = new string[] { "CD", "G", "RF" }[this.mRandom.Next(0, 3)]; }
            }

            Building building = null;
            Island island = this.mWorld.getIsland();
            switch (this.mNextBuilding) {
                case "HQ":
                    building = new HeadQuarter(this.mStateMgr, island, this, this.GetBuildPos(HeadQuarter.sizeX, HeadQuarter.sizeZ));
                    break;
                case "CD":
                    building = new CrystalDrill(this.mStateMgr, island, this, this.GetBuildPos(CrystalDrill.sizeX, CrystalDrill.sizeZ));
                    break;
                case "RF":
                    building = new RobotFactory(this.mStateMgr, island, this, this.GetBuildPos(RobotFactory.sizeX, RobotFactory.sizeZ));
                    break;
                case "G":
                    building = new Generator(this.mStateMgr, island, this, this.GetBuildPos(Generator.sizeX, Generator.sizeZ));
                    break;
            }
            if (building != null) {
                building.Build();
                this.mNextBuilding = "";
                this.NbBuildingsAllowedToAdd--;
            }

            /*foreach (VanillaNonPlayer npc in this.mRTSMgr.CharacMgr.GetFactionCharacters(this.Faction))
            {
                
            }*/
        }

        private Vector3 GetBuildPos(int sizeX, int sizeZ) {
            Vector3 pos;
            bool posAccepted;

            do {
                do {    // Get the point on the current circle
                    pos = this.mCircle.GetPoint();
                    posAccepted = pos != Vector3.ZERO;
                    if (!posAccepted)
                        this.mCircle = new Circle(this.mCircle, 5 + this.mRandom.Next(0, 6));
                } while (!posAccepted);

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
                    posAccepted = diffY >= 10;
                }

                /* Test if the building is in the bounds of the terrain */
                if (posAccepted) {
                    Vector3 islandSize = island.getSize() * 16;
                    Vector2 halfSize = new Vector2(Mogre.Math.Ceil(sizeX / 2f), Mogre.Math.Ceil(sizeX / 2f));
                    Vector2 min = new Vector2(pos.x - halfSize.x, pos.z - halfSize.y);
                    Vector2 max = new Vector2(pos.x + halfSize.x, pos.z + halfSize.y);
                    if (min.x < 0 || min.y < 0 || max.x > islandSize.x || max.y > islandSize.z)
                        posAccepted = false;
                }

                /* Modify the angle min and max of the circle */
                if (!posAccepted) {

                }

            } while (!posAccepted);

            return pos;
        }
    }
}