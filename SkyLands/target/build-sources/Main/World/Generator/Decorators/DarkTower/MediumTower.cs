using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;
using API.Generic;

using API.Geo.Cuboid;

using Game.World.Blocks;

using Mogre;


namespace Game.World.Generator.Decorators.DarkTowerPopulator {
    class MediumTower : Tower {
        public const int xMax = 20, zMax = 20;
        public void build(Island current, Random rd, Vector3 loc) {

            int towerHeight = rd.Next(20, 35);
            //body
            for(int x = 0; x < xMax; x++) {
                for(int z = 0; z < zMax; z++) {
                    for(int y = 0; y <= towerHeight; y++) {
                        if((x == 0 && z == 0) || (x == 0 && z == zMax - 1) || (x == xMax - 1 && z == 0) || (z == zMax - 1 && x == xMax - 1)) { //corners
                            current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, "TowerWoodBlock", true);
                        } else if((x == 0 || z == 0 || x == xMax - 1 || z == zMax - 1) && y != towerHeight) { //borders
                            current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, "DarkWood", true);
                        }

                        if(y == 0 || y == towerHeight - 1) {
                            if(x == 0 || z == 0 || x == xMax - 1 || z == zMax - 1) { //borders
                                current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, "TowerWoodBlock", true);
                            } else {
                                current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, "DarkWood", true);
                            }
                        }
                    }
                }
            }
        }
    }
}
