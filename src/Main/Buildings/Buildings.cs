using System;
using System.Collections.Generic;

using Mogre;

using API.Geo.Cuboid;

namespace Game.Buildings
{
    public class Buildings
    {
        private Vector3 mPos;
        private Island mIsland;
        
        public Buildings(Island island, Vector3 pos)
        {
            this.mPos = pos;
            this.mIsland = island;
            this.Build();
        }

        private void Build()
        {
            /*for (int y = 5; y > 0; y--)
            {
                for(int x = 0; x <
            }

            for (int y = 1; y <= 5; y++)
            {
                int side = 2 * y + 1;
            }*/
        }
    }
}
