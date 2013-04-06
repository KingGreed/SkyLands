using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;
using API.Geo.Cuboid;

using Game.World.Blocks;

using Mogre;

namespace Game.World.Generator.Decorators.DarkTowerPopulator {
    class MainTower {
        public int towerNum;
        public const int xMax = 20, zMax = 20;
        List<int> mFloorHeight = new List<int>();

        public MainTower(int num) { this.towerNum = num; }

        public void makeTower(Island current, Random rd, Vector3 loc) {
            int towerHeight = rd.Next(40, 71);

            loc = loc + 40 * Vector3.UNIT_Y;
            
            //body
            for(int x = 0; x < xMax; x++) {
                for(int z = 0; z < zMax; z++) {
                    for(int y = 0; y <= towerHeight; y++) {
                        if((x == 0 && z == 0) || (x == 0 && z == zMax - 1) || (x == xMax - 1 && z == 0) || (z == zMax - 1 && x == xMax - 1)) { //corners
                            current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, VanillaChunk.staticBlock["TowerWoodBlock"].getId(), true);
                        } else if((x == 0 || z == 0 || x == xMax - 1 || z == zMax - 1) && y != towerHeight) { //borders
                            current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, VanillaChunk.staticBlock["DarkWood"].getId(), true);
                        }

                        if(y == 1 || y == towerHeight - 5) {
                            //make entrances
                        }

                        if(y == 0 || y == towerHeight -1) {
                            if(x == 0 || z == 0 || x == xMax - 1 || z == zMax - 1) { //borders
                                current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, VanillaChunk.staticBlock["TowerWoodBlock"].getId(), true);
                            } else {
                                current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, VanillaChunk.staticBlock["DarkWood"].getId(), true);
                            }
                        }

                    }
                }
            }
            this.makeDarkBeard(current, towerHeight, loc);
        }

        private void makeDarkBeard(Island current, int towerHeight, Vector3 loc) {
            for(int x = 0; x < zMax; x++) {
                for(int z = 0; z < zMax; z++) {
                    if((x != 0) && (x != xMax-1) && (z != 0) && (z != zMax-1)) { continue; } // only borders

                    int length = System.Math.Abs(x - xMax / 2) + System.Math.Abs(z - zMax / 2) / 2;

                    if(length == towerHeight - 1) { length++; }
                    if(length == -1) { length = 1; }

                    for(int y = 0; y <= length; y++) {
                        current.setBlockAt((int)loc.x + x, (int)loc.y - y, (int)loc.z + z, VanillaChunk.staticBlock["TowerWoodBlock"].getId(), true);
                    }
                }
            }
        }

        private void makeFloor(Island current) {

        }
    }
}
