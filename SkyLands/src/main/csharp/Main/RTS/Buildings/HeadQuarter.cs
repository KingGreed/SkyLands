using API.Generic;
using Mogre;

using API.Geo.Cuboid;
using Game.States;

namespace Game.RTS
{
    public class HeadQuarter : Building
    {
        public Vector3 SpawnPoint { get; set; }

        public const int sizeX = 10, sizeY = 10, sizeZ = 10;

        public HeadQuarter(StateManager stateMgr, Island island, VanillaRTS rts, Vector3 position)
            : base(stateMgr, island, rts, "HQ", position) { }
        public HeadQuarter(StateManager stateMgr, Island island, VanillaRTS rts)
            : base(stateMgr, island, rts, "HQ") { }
        protected override void Init()
        {
            this.SpawnPoint = new Vector3(5, 1, 5);
            if(this.RTS.Faction == Faction.Blue)
                this.mStateMgr.MainState.CharacMgr.MainPlayer.setSpawnPoint((this.SpawnPoint + this.Position) * Cst.CUBE_SIDE);

            this.Size = new Vector3(sizeX, sizeY, sizeZ);
            this.mBuilding = new byte[sizeX, sizeY, sizeZ];
            //this.mConsBlockPos = this.SpawnPoint;
            this.mYDiff = 2;
        }

        protected override void Create()
        {
            base.Create();

            byte[,] _base = new byte[sizeX, sizeZ]
            {
                {0,0,1,1,1,1,1,1,0,0},
                {0,1,0,0,0,0,0,0,1,0},
                {1,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,1},
                {0,1,0,0,0,0,0,0,1,0},
                {0,0,1,1,1,1,1,1,0,0}
            };
            byte[,] _middle = new byte[sizeX, sizeZ]
            {
                {0,0,1,1,1,1,1,1,0,0},
                {0,1,1,1,1,1,1,1,1,0},
                {1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1},
                {0,1,1,1,1,1,1,1,1,0},
                {0,0,1,1,1,1,1,1,0,0}
            };
            byte[,] _roof = new byte[sizeX, sizeZ]
            {
                {0,0,1,1,1,1,1,1,0,0},
                {0,1,1,1,1,1,1,1,1,0},
                {1,1,2,2,1,1,2,2,1,1},
                {1,1,2,2,1,1,2,2,1,1},
                {1,1,1,1,2,2,1,1,1,1},
                {1,1,1,1,2,2,1,1,1,1},
                {1,1,2,2,1,1,2,2,1,1},
                {1,1,2,2,1,1,2,2,1,1},
                {0,1,1,1,1,1,1,1,1,0},
                {0,0,1,1,1,1,1,1,0,0}
            };
            const byte wood = 4;
            const byte darkWood = 11;
            const byte glass = 22;

            for(int y = 0; y < 3; y++)
                for (int x = 0; x < sizeX; x++)
                    for (int z = 0; z < sizeZ; z++)
                        this.mBuilding[x, y, z] = (byte) ((_middle[x, z] == 0) ? 0 : darkWood);

            for (int y = 3; y < 8; y++)
                for (int x = 0; x < sizeX; x++)
                    for (int z = 0; z < sizeZ; z++)
                        this.mBuilding[x, y, z] = (byte)((_base[x, z] == 0) ? 0 : wood);

            for (int y = 3; y < 5; y++)
                for (int x = 0; x < 2; x++)
                    this.mBuilding[sizeX / 2 + x - 1, y, 0] = 0;

            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    byte b = glass;
                    if (_roof[x, z] == 0)
                        b = 0;
                    else if (_roof[x, z] == 1)
                        b = wood;
                    this.mBuilding[x, sizeY - 2, z] = b;
                }
            }

            for (int x = 0; x < sizeX; x++)
                for (int z = 0; z < sizeX; z++)
                    this.mBuilding[x, sizeY - 1, z] = (byte)((_base[x, z] == 0) ? 0 : darkWood);
        }
    }
}
