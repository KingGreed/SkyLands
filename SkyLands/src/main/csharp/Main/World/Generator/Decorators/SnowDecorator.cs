using System;
using API.Generator;
using API.Generic;
using API.Geo.Cuboid;

namespace Game.World.Generator.Decorators {
    class SnowDecorator  : Decorator {
        public override void populate(Island curr, Random random) {
            for(int x = 0; x < curr.getSize().x * Cst.CHUNK_SIDE; x++) {
                for(int z = 0; z < curr.getSize().z * Cst.CHUNK_SIDE; z++) {
                    int y = curr.getSurfaceHeight(x, z);
                    
                    if(y >= 200) {
                        curr.setBlockAt(x, y, z, 23, true);
                    }
                }
            }

        }
    }
}
