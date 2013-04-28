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
    class Tower : Decorator
    {
        public Tower() { }

        private int[,] _base = new int[10, 10]
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

        private int[,] _standard = new int[10, 10]
        {
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,1,1,1,1,1,1,0,0},
            {0,1,1,0,0,0,0,1,1,0},
            {0,1,0,0,0,0,0,0,1,0},
            {0,1,0,0,0,0,0,0,1,0},
            {0,1,0,0,0,0,0,0,1,0},
            {0,1,0,0,0,0,0,0,1,0},
            {0,1,1,0,0,0,0,1,1,0},
            {0,0,1,1,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,0}
        };

        public override void populate(Island curr, Random random)
        {
            Vector3 pos = this.findRandomPoint(curr, random);

            string brick = "SmoothStone";

                for (int x = 0; x < _base.GetLength(0); x++)
                    for (int z = 0; z < _base.GetLength(1); z++)
                    {
                        if (_base[x,z] == 0)
                            curr.setBlockAt((int)pos.x + x, (int)pos.y, (int)pos.z + z, "Air", true);
                        else
                            curr.setBlockAt((int)pos.x + x, (int)pos.y, (int)pos.z + z, brick, true);
                    }

            for (int i = 1; i < 4; i++)
            {
                for (int y = 0 * i + 1; y < 5 * i + 1; y++)
                    for (int x = 0; x < _base.GetLength(0); x++)
                        for (int z = 0; z < _base.GetLength(1); z++)
                        {
                            if (_standard[x, z] == 0)
                                curr.setBlockAt((int)pos.x + x, (int)pos.y + y, (int)pos.z + z, "Air", true);
                            else
                                curr.setBlockAt((int)pos.x + x, (int)pos.y + y, (int)pos.z + z, brick, true);
                        }
            }

            for (int y = 1; y < 3; y++)
                for (int x = 0; x < 2; x++)
                    curr.setBlockAt((int)pos.x + _standard.GetLength(0)/2 + x - 1, (int)pos.y + y, (int)pos.z + 1 , "Air", true);

            for (int i = 0; i < 2; i++)
            {
                for (int x = 0; x < _base.GetLength(0); x++)
                    for (int z = 0; z < _base.GetLength(1); z++)
                    {
                        if (_base[x, z] == 0)
                            curr.setBlockAt((int)pos.x + x, (int)pos.y + 16, (int)pos.z + z, "Air", true);
                        else
                            curr.setBlockAt((int)pos.x + x, (int)pos.y + 16, (int)pos.z + z, brick, true);
                    }
            }
        }
    }
}
