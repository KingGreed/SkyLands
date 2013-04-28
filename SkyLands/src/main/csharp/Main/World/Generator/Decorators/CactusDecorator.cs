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
    class CactusDecorator : Decorator {
        public CactusDecorator() {}


        public override void populate(Island curr, Random random) {
            for (int i = 0; i < (curr.getSize().x + curr.getSize().z) * 4 ; i++)
            {
                Vector3 pos = this.findRandomPoint(curr, random);

                byte cactus = VanillaChunk.staticBlock["Cactus"].getId();

                if((curr.getBlock((int)pos.x, (int)pos.y, (int)pos.z, true) is AirBlock)) {

                    curr.setBlockAt((int)pos.x, (int)pos.y, (int)pos.z, cactus, true);
                    curr.setBlockAt((int)pos.x, (int)pos.y + 1, (int)pos.z, cactus, true);
                    if(random.Next(4) > 1) {
                        curr.setBlockAt((int)pos.x, (int)pos.y + 2, (int)pos.z, cactus, true);
                        if(random.Next(11) > 6) {
                            curr.setBlockAt((int)pos.x, (int)pos.y + 3, (int)pos.z, cactus, true);
                            if(random.Next(20) > 15) { curr.setBlockAt((int)pos.x, (int)pos.y + 4, (int)pos.z, cactus, true); }
                        }
                    }
                }
            }
        }
    }
}
