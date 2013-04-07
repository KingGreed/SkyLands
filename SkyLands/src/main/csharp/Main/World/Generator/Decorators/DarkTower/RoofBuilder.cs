using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;
using API.Geo.Cuboid;

using Game.World.Blocks;

using Mogre;

namespace Game.World.Generator.Decorators.DarkTowerPopulator {
    class RoofBuilder {
        public void build(Island current, Vector3 location, Vector2 size, Random rd) {
            for(int x = 0; x < size.x; x++) {
                for(int z = 0; z < size.y; z++) {
                    if(x == 0 || x == size.x - 1 || z == 0 || z == size.y - 1) {
                        //build fences
                    }
                    if(x == size.x / 2 && z == size.y / 2) { this.buildStructure(current, rd.Next(0, 100) % 2, new Vector3(location.x + x, location.y, location.z + z)); }
                }
            }
        }

        public void buildStructure(Island current, int rand, Vector3 loc) {
            if(rand == 0) {
                this.makePoint(current, loc);
                for(int i = 0; i < 8; i++) { current.setBlockAt((int)loc.x, (int)loc.y + i, (int)loc.z, "TowerWoodBlock", true); }
                this.makePoint(current, loc + Vector3.UNIT_Y * 7);
            } else if(rand == 1) {
                this.makePoint(current, loc);
                for(int i = 0; i < 8; i++) { current.setBlockAt((int)loc.x, (int)loc.y + i, (int)loc.z, "TowerWoodBlock", true); }
                this.makePoint(current, loc + Vector3.UNIT_Y * 7);
                this.makeRings(current, loc);
            }
        }

        public void makePoint(Island current, Vector3 loc) {
            current.setBlockAt((int)loc.x, (int)loc.y, (int)loc.z, "TowerWoodBlock", true);
            current.setBlockAt((int)loc.x + 1, (int)loc.y, (int)loc.z, "TowerWoodBlock", true);
            current.setBlockAt((int)loc.x - 1, (int)loc.y, (int)loc.z, "TowerWoodBlock", true);
            current.setBlockAt((int)loc.x, (int)loc.y, (int)loc.z + 1, "TowerWoodBlock", true);
            current.setBlockAt((int)loc.x, (int)loc.y, (int)loc.z - 1, "TowerWoodBlock", true);
        }

        public void makeRings(Island current, Vector3 loc) {
            for(int x = -3; x <= 3; x++) {
                for(int z = -3; z <= 3; z++) {
                    for(int y = 0; y <= 8; y++) {
                        if(y == 2 || y == 4 || y == 6) {
                            if(x == -3 || z == -3 || x == 3 || z == 3) {
                                current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, "TowerWoodBlock", true);
                            }
                        }
                    }
                }
            }
        }
    }
}
