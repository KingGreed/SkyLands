using API.Generic;
using Game.World.Generator;
using Mogre;

using API.Geo.Cuboid;
using Game.States;

namespace Game.RTS
{
    public class RobotFactory : Building
    {
        private const int sizeX = 5, sizeY = 5, sizeZ = 5;

        public RobotFactory(StateManager stateMgr, Island island, Vector3 position) : base(stateMgr, island, position) { }
        protected override void Init()
        {
            this.Size = new Vector3(sizeX, sizeY, sizeZ);
            this.mBuilding = new byte[sizeX, sizeY, sizeZ];
            this.mNeededRessources.Add(3, 20); // stone
            this.mYDiff = 1;
        }

        protected override void Create()
        {
            const byte smoothStone = 20;

            for (int y = 0; y < 2; y++)
                for (int x = 0; x < sizeX; x++)
                    for (int z = 0; z < sizeZ; z++)
                        this.mBuilding[x, y, z] = smoothStone;

            for (int y = 2; y < sizeY; y++)
                for (int x = 1; x < sizeX - 1; x++)
                    for (int z = 1; z < sizeZ - 1; z++)
                        this.mBuilding[x, y, z] = smoothStone;

            for (int y = 2; y < 4; y++)
                this.mBuilding[2, y, 2] = 0;

            for (int y = 2; y < 4; y++)
                this.mBuilding[2, y, 1] = this.mColoredBlock;

            for (int y = 2; y < 4; y++)
                for (int x = 1; x < sizeX - 1; x++)
                    this.mClearZone.Add(new Vector3(x, y, 0));

            base.Create();
        }
    }
}
