using System.Collections.Generic;
using API.Generic;
using Game.CharacSystem;
using Game.World.Generator;
using Mogre;

using API.Geo.Cuboid;
using Game.States;

namespace Game.RTS
{
    public class RobotFactory : Building
    {
        public const int ROBOT_COST = 30;
        
        public const int sizeX = 5, sizeY = 6, sizeZ = 5;

        private Vector3 mSpawnPoint;  // Absolute coord
        private Timer mTimeSinceLastPop;
        private bool mHasCreateOneUnit = false;

        public bool CanCreateUnit { get { return !this.mHasCreateOneUnit || this.mTimeSinceLastPop.Milliseconds >= 8000; } }

        public RobotFactory(StateManager stateMgr, Island island, VanillaRTS rts) : base(stateMgr, island, rts, "RF") { }
        public RobotFactory(StateManager stateMgr, Island island, VanillaRTS rts, Vector3 position)
            : base(stateMgr, island, rts, "RF", position)
        {
        }
        protected override void Init()
        {
            this.Size = new Vector3(sizeX, sizeY, sizeZ);
            this.mBuilding = new byte[sizeX, sizeY, sizeZ];
            this.mYDiff = 1;
        }

        protected override void Create()
        {
            base.Create();
            const byte smoothStone = 20;

            for (int y = 0; y < 2; y++)
                for (int x = 0; x < sizeX; x++)
                    for (int z = 0; z < sizeZ; z++)
                        this.mBuilding[x, y, z] = smoothStone;

            for (int y = 2; y < sizeY; y++)
                for (int x = 1; x < sizeX - 1; x++)
                    for (int z = 1; z < sizeZ - 1; z++)
                        this.mBuilding[x, y, z] = smoothStone;

            for (int y = 2; y < 5; y++)
                this.mBuilding[2, y, 2] = 99;

            for (int y = 2; y < 5; y++)
                this.mBuilding[2, y, 1] = this.mColoredBlock;
        }

        protected override void OnBuild()
        {
            base.OnBuild();
            Vector3 diff = new Vector3(sizeX / 2f * this.mSymetricFactor, 1, sizeZ / 2f * this.mSymetricFactor);
            if (this.mSymetricFactor == 1)
                diff.z--;
            else
                diff.x++;
            this.mSpawnPoint = (this.Position + diff) * Cst.CUBE_SIDE;

            this.mTimeSinceLastPop = new Timer();
            this.RTS.NbFactory++;
        }

        public void CreateUnit()    // Called by VanillaRTS only
        {
            if (!this.CanCreateUnit) { return; }

            this.RTS.Crystals -= ROBOT_COST;
            this.RTS.AmountUnits++;
            CharacterInfo info = new CharacterInfo("Robot", this.RTS) {SpawnPoint = this.mSpawnPoint};
            int id = info.Id;
            this.mStateMgr.MainState.CharacMgr.AddCharacter(info);
            if(this.mSymetricFactor == 1)
                this.mStateMgr.MainState.CharacMgr.GetCharacterById(id).YawTo(180);
            //VanillaCharacter charac = this.mStateMgr.MainState.CharacMgr.GetCharacterById(id);
            //charac.SetForcedDest((this.Position + new Vector3(0, 1, 2 * this.mSymetricFactor)) * Cst.CUBE_SIDE);
            this.mTimeSinceLastPop.Reset();
            this.mHasCreateOneUnit = true;
        }
    }
}
