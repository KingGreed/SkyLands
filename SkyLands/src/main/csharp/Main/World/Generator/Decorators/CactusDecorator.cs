using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;
using API.Geo.Cuboid;

using Game.World.Blocks;

namespace Game.World.Generator.Decorators
{
    class CactusDecorator : Decorator {
        public CactusDecorator() {}


        public override void populate(Island curr, Random random)
        {
            for (int i = 0; i < (curr.getSize().x + curr.getSize().z) * 4 ; i++)
            {
                int x = random.Next() % (int)(curr.getSize().x * MainWorld.CHUNK_SIDE);
                int z = random.Next() % (int)(curr.getSize().z * MainWorld.CHUNK_SIDE);
                int y = curr.getSurfaceHeight(x, z);

                while(y == -1) {
                    x = random.Next() % (int)(curr.getSize().x * MainWorld.CHUNK_SIDE);
                    z = random.Next() % (int)(curr.getSize().z * MainWorld.CHUNK_SIDE);
                    y = curr.getSurfaceHeight(x, z);
                }

                curr.setBlockAt(x, y, z, 8, true);
                curr.setBlockAt(x, y+1, z, 8, true);
                if (random.Next(4)  > 1) {
                    curr.setBlockAt(x, y + 2, z, 8, true);
                    if (random.Next(11) > 6) {
                        curr.setBlockAt(x, y + 3, z, 8, true);
                        if (random.Next(20) > 15) { curr.setBlockAt(x, y + 4, z, 8, true); }
                    }
                }

            }
        }
    }
}
