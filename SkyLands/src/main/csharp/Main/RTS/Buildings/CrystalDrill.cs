﻿using Mogre;

using API.Geo.Cuboid;
using Game.States;

namespace Game.RTS
{
    public class CrystalDrill : Building
    {
        private const int sizeX = 5, sizeY = 6, sizeZ = 5;

        public CrystalDrill(StateManager stateMgr, Island island, Vector3 position) : base(stateMgr, island, position) { }
        protected override void Init()
        {
            this.Size = new Vector3(sizeX, sizeY, sizeZ);
            this.mBuilding = new byte[sizeX, sizeY, sizeZ];
            this.mConstrBlock.AddRemainingRessource(2, 10); // dirt
            this.mConstrBlock.AddRemainingRessource(3, 10); // stone
            this.mYDiff = sizeY - 3;
        }

        protected override void Create()
        {
            const byte smoothScarvedStone = 21;
            const byte glass = 22;

            for (int x = 0; x < sizeX; x++)
            {
                this.mBuilding[x, sizeY - 2, sizeZ/2] = smoothScarvedStone;
                if(x == 0 || x == sizeX - 1)
                    this.mBuilding[x, sizeY - 3, sizeZ / 2] = smoothScarvedStone;
            }

            for (int z = 0; z < sizeZ; z++)
            {
                this.mBuilding[sizeX/2, sizeY - 2, z] = smoothScarvedStone;
                if (z == 0 || z == sizeZ - 1)
                    this.mBuilding[sizeX / 2, sizeY - 3, z] = smoothScarvedStone;
            }

            Vector2[] downPoints = new Vector2[] { new Vector2(sizeX/2, 0), new Vector2(0, sizeZ/2),
                new Vector2(sizeX/2, sizeZ - 1), new Vector2(sizeX - 1, sizeZ/2), new Vector2(sizeX/2, sizeZ/2) };

            for(int i = 0; i < downPoints.Length; i++)
            {
                byte b = (i == downPoints.Length - 1) ? glass : smoothScarvedStone;
                for (int y = 1; y < sizeY - 1; y++)
                    this.mBuilding[(int)downPoints[i].x, y, (int)downPoints[i].y] = b;
            }
            this.mBuilding[(int)downPoints[4].x, 0, (int)downPoints[4].y] = glass;
            this.mBuilding[(int)downPoints[4].x, sizeY - 1, (int)downPoints[4].y] = this.mColoredBlock;

            for (int y = sizeY - 3; y < sizeY; y++)
                for (int x = 0; x < sizeX; x++)
                    for (int z = 0; z < sizeZ; z++)
                        this.mClearZone.Add(new Vector3(x, y, z));

            base.Create();
        }
    }
}
