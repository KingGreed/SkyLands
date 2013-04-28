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
        public void build(Island current, Vector3 location, Vector2 size, Random rd, bool portal = false) {
            for(int x = 0; x < size.x; x++) {
                for(int z = 0; z < size.y; z++) {
                    if(x == 0 || x == size.x - 1 || z == 0 || z == size.y - 1) {
                        //build fences
                    }

                    if(x == size.x / 2 && z == size.y / 2) { this.buildStructure(current, rd.Next(0, 100) % 3, new Vector3(location.x + x, location.y, location.z + z)); }
                }
            }

            if(portal) {
                Vector3 pos = new Vector3(location.x + size.x / 2, location.y, location.z + size.y / 2);
                Portals.makePortal(1, current, pos);
            }
        }

        public void buildStructure(Island current, int rand, Vector3 loc) {
            if(rand == 0) {
                this.makeCross(current, loc);
                for(int i = 0; i < 8; i++) { current.setBlockAt((int)loc.x, (int)loc.y + i, (int)loc.z, "TowerWoodBlock", true); }
                this.makeCross(current,  loc + Vector3.UNIT_Y * 7);
                this.makeSquare(current, loc + Vector3.UNIT_Y * 8);
                this.makeCross(current,  loc + Vector3.UNIT_Y * 9);
            } else if(rand == 1) {
                this.makeCross(current, loc);
                for(int i = 0; i < 8; i++) { current.setBlockAt((int)loc.x, (int)loc.y + i, (int)loc.z, "TowerWoodBlock", true); }
                this.makeCross(current, loc + Vector3.UNIT_Y * 7);
                this.makeRings(current, loc);
            } else if(rand == 2) {
                this.makeCross(current, loc);
                for(int i = 0; i < 8; i++) { current.setBlockAt((int)loc.x, (int)loc.y + i, (int)loc.z, "DarkWood", true); }
                current.setBlockAt((int)loc.x, (int)loc.y + 3, (int)loc.z + 1, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x, (int)loc.y + 3, (int)loc.z + 2, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x, (int)loc.y + 4, (int)loc.z + 2, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x, (int)loc.y + 5, (int)loc.z + 2, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x, (int)loc.y + 5, (int)loc.z + 3, "TowerWoodBlock", true);

                current.setBlockAt((int)loc.x + 1, (int)loc.y + 4, (int)loc.z, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x + 2, (int)loc.y + 4, (int)loc.z, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x + 2, (int)loc.y + 5, (int)loc.z, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x + 2, (int)loc.y + 6, (int)loc.z, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x + 3, (int)loc.y + 6, (int)loc.z, "TowerWoodBlock", true);

                current.setBlockAt((int)loc.x, (int)loc.y + 5, (int)loc.z - 1, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x, (int)loc.y + 5, (int)loc.z - 2, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x, (int)loc.y + 6, (int)loc.z - 2, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x, (int)loc.y + 7, (int)loc.z - 2, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x, (int)loc.y + 7, (int)loc.z - 3, "TowerWoodBlock", true);

                current.setBlockAt((int)loc.x - 1, (int)loc.y + 6, (int)loc.z, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x - 2, (int)loc.y + 6, (int)loc.z, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x - 2, (int)loc.y + 7, (int)loc.z, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x - 2, (int)loc.y + 8, (int)loc.z, "TowerWoodBlock", true);
                current.setBlockAt((int)loc.x - 3, (int)loc.y + 8, (int)loc.z, "TowerWoodBlock", true);

                current.setBlockAt((int)loc.x, (int)loc.y + 8, (int)loc.z, "TowerWoodBlock", true);

                
            }
        }

        public void makeCross(Island current, Vector3 loc) {
            current.setBlockAt((int)loc.x + 1, (int)loc.y, (int)loc.z, "TowerWoodBlock", true);
            current.setBlockAt((int)loc.x - 1, (int)loc.y, (int)loc.z, "TowerWoodBlock", true);
            current.setBlockAt((int)loc.x, (int)loc.y, (int)loc.z + 1, "TowerWoodBlock", true);
            current.setBlockAt((int)loc.x, (int)loc.y, (int)loc.z - 1, "TowerWoodBlock", true);
        }

        public void makeSquare(Island current, Vector3 loc) {
            for(int x = -1; x <= 1; x++) {
                for(int z = -1; z <= 1
                    ; z++) {
                    current.setBlockAt((int)loc.x + x, (int)loc.y, (int)loc.z + z, "TowerWoodBlock", true);
                }

            }
        }


        public void makeRings(Island current, Vector3 loc) {
            for(int x = -3; x <= 3; x++) {
                for(int z = -3; z <= 3; z++) {
                    for(int y = 0; y <= 8; y++) {
                        if((x == -3 && z == -3) || (x == -3 && z == 3) || (x == 3 && z == -3) || (x == 3 && z == 3)) { continue; }
                        if(y == 3 || y == 5) {
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
