using System;
using System.Collections.Generic;

using Mogre;

using Game.World.Blocks;
using API.Geo.Cuboid;

namespace Game.Buildings
{
    public class Building
    {
        private Vector3 mPos;
        private Island mIsland;

        public Building(Island island, Vector3 pos)
        {
            this.mPos = pos;
            this.mIsland = island;
            this.Build();
        }

        private void Build()
        {
            LogManager.Singleton.DefaultLog.LogMessage("test");
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    for (int z = 0; z < 5; z++)
                    {
                        if (this.mIsland.getBlock(this.mPos + new Vector3(x, y, z), true) is Air && isInSphere(x, y, z, 5))
                        {
                            this.mIsland.addBlockToScene(this.mPos + new Vector3(x, y, z), "Grass");
                        }
                    }
                }
            }
        }
        private bool isInSphere(int x, int y, int z, int r)
        {
            return (x * x + y * y + z * z <= r * r);
        }
    }
}
