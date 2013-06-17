using API.Geo.Cuboid;

using Mogre;

namespace Game.RTS
{
    class HeadQuarter : Building
    {
        public HeadQuarter(Vector3 position) : base(position) { }

        protected override void Create()
        {
            const int sizeX = 10, sizeY = 8, sizeZ = 10;

            this.Size = new Vector3(sizeX, sizeY, sizeZ);
            this.mBuilding = new byte[sizeX, sizeY, sizeZ];
            this.mNeededRessources.Add(2, 5);   // dirt
            this.mNeededRessources.Add(13, 20); // planks
            
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

            for (int x = 0; x < sizeX; x++)
                for (int z = 0; z < sizeZ; z++)
                    this.mBuilding[x, 0, z] = (byte) ((_middle[x, z] == 0) ? 0 : darkWood);

            for (int y = 1; y < 6; y++)
                for (int x = 0; x < sizeX; x++)
                    for (int z = 0; z < sizeZ; z++)
                        this.mBuilding[x, y, z] = (byte)((_base[x, z] == 0) ? 0 : wood);

            for (int y = 1; y < 3; y++)
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
                    this.mBuilding[x, 6, z] = b;
                }
            }

            for (int x = 0; x < sizeX; x++)
                for (int z = 0; z < sizeX; z++)
                    this.mBuilding[x, 7, z] = (byte)((_base[x, z] == 0) ? 0 : darkWood);
        }
    }
}
