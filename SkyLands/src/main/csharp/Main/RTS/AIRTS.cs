using System;
using Game.States;
using Game.World.Blocks;
using Mogre;

namespace Game.RTS
{
    public class AIRTS : VanillaRTS
    {
        private Vector3 mSafeBuildPos;
        private Random mRandom;
        private string mNextBuilding;
        private int mNbUpdateSkipped;

        public int NbBuildingsAllowedToAdd { get; set; }
        public int NbRobotsAllowedToAdd { get; set; }
        
        public AIRTS(StateManager stateMgr, RTSManager RTSMgr) : base(stateMgr, RTSMgr)
        {
            this.Faction = Faction.Red;
            Vector3 isldSize = this.mStateMgr.MainState.World.getIsland().getSize();
            Vector2[] corner = new Vector2[] { Vector2.ZERO, new Vector2(0, isldSize.z),
                new Vector2(isldSize.x, 0), new Vector2(isldSize.x, isldSize.z) };

            Vector2 coin = (16 * corner[new Random().Next(0, 4)] + 10 * Vector2.UNIT_SCALE) / 1.3f;
            this.mSafeBuildPos = new Vector3(coin.x, 16 * isldSize.y, coin.y);
            while (this.mStateMgr.MainState.World.getIsland().getBlock(this.mSafeBuildPos, false) is Air && this.mSafeBuildPos.y > 0)
                this.mSafeBuildPos.y--;
            this.mSafeBuildPos.y++;

            this.mRandom = new Random();
            this.mNextBuilding = "";
            this.mNbUpdateSkipped = 0;
            this.NbRobotsAllowedToAdd = 0;
            this.NbBuildingsAllowedToAdd = 4;
        }

        protected override void Update()
        {
            if (this.mNbUpdateSkipped++ < 5) { return; }
            this.mNbUpdateSkipped = 0;

            int nbRobotTocreate = 0;
            if (this.AmountUnits < 3) { nbRobotTocreate = 3 - this.AmountUnits; }
            if (this.NbRobotsAllowedToAdd > nbRobotTocreate) { nbRobotTocreate = this.NbRobotsAllowedToAdd; }
            this.NbRobotsAllowedToAdd = nbRobotTocreate - this.CreateRobot(nbRobotTocreate);

            if (this.NbBuildingsAllowedToAdd <= 0) { return; }
            if(this.mNextBuilding == "")
            {
                if (!this.Buildings.Exists(b => b.Selection == "HQ"))                                             { this.mNextBuilding = "HQ"; }
                else if (this.mRTSMgr.PlayerRTS.CrystalSpeed > 1.3 * this.CrystalSpeed || this.CrystalSpeed == 5) { this.mNextBuilding = "CD"; }
                else if (this.mRTSMgr.PlayerRTS.Capacity > 1.3 * this.Capacity || this.Capacity == 0)             { this.mNextBuilding = "G"; }
                else if (this.mRTSMgr.PlayerRTS.NbFactory > 1.3 * this.NbFactory || this.NbFactory == 0)          { this.mNextBuilding = "RF"; }
                else { mNextBuilding = new string[] { "CD", "G", "RF" }[this.mRandom.Next(0, 3)]; }
            }

            Building building = null;
            switch (this.mNextBuilding)
            {
                case "HQ":
                    building = new HeadQuarter(this.mStateMgr, this.mStateMgr.MainState.World.getIsland(), this, this.mSafeBuildPos);
                    break;
                case "CD":
                    building = new CrystalDrill(this.mStateMgr, this.mStateMgr.MainState.World.getIsland(), this, this.mSafeBuildPos);
                    break;
                case "RF":
                    building = new RobotFactory(this.mStateMgr, this.mStateMgr.MainState.World.getIsland(), this, this.mSafeBuildPos);
                    break;
                case "G":
                    building = new Generator(this.mStateMgr, this.mStateMgr.MainState.World.getIsland(), this, this.mSafeBuildPos);
                    break;
            }
            if (building != null)
            {
                building.Build();
                this.Buildings.Add(building);
                this.ComputeNextBuildPos();
                this.mNextBuilding = "";
                this.NbBuildingsAllowedToAdd--;
            }

            /*foreach (VanillaNonPlayer npc in this.mRTSMgr.CharacMgr.GetFactionCharacters(this.Faction))
            {
                
            }*/
        }

        private void ComputeNextBuildPos()
        {
            Vector3 newSafePos;
            const float radius = 11;
            //float dist = radius;

            Console.WriteLine("Begin");
            do {
                float angle = new Random().Next(0, 360);
                newSafePos = this.mSafeBuildPos + new Vector3((int)(System.Math.Cos(angle) * radius), 0, (int)(System.Math.Sin(angle) * radius));
            } while (this.Buildings.TrueForAll(b => (b.Position - newSafePos).Length > radius));

            /*while (this.mStateMgr.MainState.World.getIsland().getBlock(newSafePos, false) is Air)
                newSafePos.y--;*/
            while (!(this.mStateMgr.MainState.World.getIsland().getBlock(newSafePos, false) is Air))
                newSafePos.y++;

            Console.WriteLine("Build pos : " + newSafePos);
            this.mSafeBuildPos = newSafePos;
        }
    }
}
