using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;
using API.Geo.Cuboid;

using Game.World.Blocks;

using Mogre;

namespace Game.World.Generator.Decorators.DarkTowerPopulator {
    class DarkBeard {
        public void makeDarkBeard(Island current, int towerHeight, Vector3 loc, int zMax, int xMax) {
            for(int x = 0; x < zMax; x++) {
                for(int z = 0; z < zMax; z++) {
                    if((x != 0) && (x != xMax - 1) && (z != 0) && (z != zMax - 1)) { continue; } // only borders

                    int length = System.Math.Abs(x - xMax / 2) + System.Math.Abs(z - zMax / 2) / 2;

                    if(length == towerHeight - 1) { length++; }
                    if(length == -1) { length = 1; }

                    for(int y = 0; y <= length; y++) {
                        current.setBlockAt((int)loc.x + x, (int)loc.y - y, (int)loc.z + z, VanillaChunk.staticBlock["TowerWoodBlock"].getId(), true);
                    }
                }
            }
        }

    }
}
