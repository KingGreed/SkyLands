using API.Generic;
using Game.World.Generator;
using Mogre;

using API.Geo.Cuboid;
using Game.States;

namespace Game.RTS
{
    public class Generator : Building
    {
        private const int sizeX = 4, sizeY = 5, sizeZ = 4;

        public Generator(StateManager stateMgr, Island island, Faction fact, string selection)
            : base(stateMgr, island, fact, selection) { }
        protected override void Init()
        {
            this.Size = new Vector3(sizeX, sizeY, sizeZ);
            this.mBuilding = new byte[sizeX, sizeY, sizeZ];
            this.mYDiff = 1;
        }

        protected override void Create()
        {
            base.Create();

            const byte smoothScarvedStone = 21; 
            const byte glass = 22;

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    for (int z = 0; x < sizeZ; z++)
                    {
                        if (x == 0 || x == sizeX - 1 || z == 0 || z == sizeZ - 1) { continue; }

                        byte b = glass;
                        if (y == 0 || y == sizeY - 1) { b = smoothScarvedStone; }

                        this.mBuilding[x, y, z] = b;
                    }
                }
            }
        }
    }
}
