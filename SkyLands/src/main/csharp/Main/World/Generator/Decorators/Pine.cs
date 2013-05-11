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
    class Pine : Decorator {
        public override void populate(Island curr, Random random) {
            {
                for (int i = 0; i < (curr.getSize().x + curr.getSize().z) * 4; i++)
                {
                    Vector3 pos = this.findRandomPoint(curr, random, "Grass");

                    if ((curr.getBlock((int)pos.x, (int)pos.y, (int)pos.z, true) is AirBlock))
                    {
                        for (int p = 1; p < 6; p++)
                        {
                            int tmp = 0;
                            for (int y = 6 * p; y < 9 * p; y++)
                            {
                                for (int x = 0 + tmp; x < 7 - tmp; x++)
                                {
                                    for (int z = 0 + tmp; z < 7 - tmp; z++)
                                    {
                                        curr.setBlockAt((int)pos.x + x - 3, (int)pos.y + y, (int)pos.z + z - 3, "Leaves", true);
                                    }
                                }
                                tmp++;
                            }
                        }

                        for (int y = 0; y < 30; y++)
                        {
                            curr.setBlockAt((int)pos.x, (int)pos.y + y, (int)pos.z, "DarkWood", true);
                        }
                    }
                }
            }
        }
    }
}
