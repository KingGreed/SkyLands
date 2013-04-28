using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;
using API.Geo.Cuboid;

using Game.World.Blocks;

using Mogre;

namespace Game.World.Generator.Decorators
{
    class Spawner : Decorator
    {
        public Spawner() { }

        private int[,] _base = new int[10, 10]
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
        private int[,] _renaudTmoche = new int[10, 10]
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
        private int[,] _roof = new int[10, 10]
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


        
        public override void populate(Island curr, Random random)
        {
            Vector3 pos = this.findRandomPoint(curr, random);

            string brick1 = "SmoothStone";
            string brick2 = "SmoothStone_Carved";

            for (int x = 0; x < _base.GetLength(0); x++)
                for (int z = 0; z < _base.GetLength(1); z++)
                {
                    if (_renaudTmoche[x, z] == 0)
                        curr.setBlockAt((int)pos.x + x, (int)pos.y, (int)pos.z + z, "Air", true);
                    else
                        curr.setBlockAt((int)pos.x + x, (int)pos.y, (int)pos.z + z, brick2, true);
                }

            for (int y = 1; y < 6; y++)
            {
                for (int x = 0; x < _base.GetLength(0); x++)
                    for (int z = 0; z < _base.GetLength(1); z++)
                    {
                        if (_base[x, z] == 0)
                            curr.setBlockAt((int)pos.x + x, (int)pos.y + y, (int)pos.z + z, "Air", true);
                        else
                            curr.setBlockAt((int)pos.x + x, (int)pos.y + y, (int)pos.z + z, brick1, true);
                    }
            }

            for (int y = 1; y < 3; y++)
                for (int x = 0; x < 2; x++)
                    curr.setBlockAt((int)pos.x + _base.GetLength(0)/2 + x - 1, (int)pos.y + y, (int)pos.z, "Air", true);

            for (int x = 0; x < _base.GetLength(0); x++)
                for (int z = 0; z < _base.GetLength(1); z++)
                {
                    if (_roof[x, z] == 0)
                        curr.setBlockAt((int)pos.x + x, (int)pos.y + 6, (int)pos.z + z, "Air", true);
                    else if (_roof[x,z] == 1)
                        curr.setBlockAt((int)pos.x + x, (int)pos.y + 6, (int)pos.z + z, brick1, true);
                    else
                        curr.setBlockAt((int)pos.x + x, (int)pos.y + 6, (int)pos.z + z, "Glass", true);
                }

            for (int x = 0; x < _base.GetLength(0); x++)
                for (int z = 0; z < _base.GetLength(1); z++)
                {
                    if (_base[x, z] == 0)
                        curr.setBlockAt((int)pos.x + x, (int)pos.y + 7, (int)pos.z + z, "Air", true);
                    else
                        curr.setBlockAt((int)pos.x + x, (int)pos.y + 7, (int)pos.z + z, brick2, true);
                }
        }
    }
}
