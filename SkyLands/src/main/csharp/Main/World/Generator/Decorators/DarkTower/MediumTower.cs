using System;
using System.Collections.Generic;

using API.Generic;
using API.Geo.Cuboid;

using Game.CharacSystem;
using Game.States;

using Mogre;


namespace Game.World.Generator.Decorators.DarkTowerPopulator {
    class MediumTower : Tower {
        public const int xMax = 10, zMax = 10;
        Orientation mSourceBuilding;

        public MediumTower(Orientation o) { this.mSourceBuilding = o; }

        public void build(Island current, Random rd, Vector3 loc) {

            int towerHeight = rd.Next(15, 30);
            //body
            for(int x = 0; x < xMax; x++) {
                for(int z = 0; z < zMax; z++) {
                    for(int y = 0; y <= towerHeight; y++) {
                        if((x == 0 && z == 0) || (x == 0 && z == zMax - 1) || (x == xMax - 1 && z == 0) || (z == zMax - 1 && x == xMax - 1)) { //corners
                            current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, "TowerWoodBlock", true);
                        } else if((x == 0 || z == 0 || x == xMax - 1 || z == zMax - 1) && y != towerHeight) { //borders
                            current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, "DarkWood", true);
                        }

                        if(y == 0 || y == 1 || y == towerHeight - 1) {
                            if(x == 0 || z == 0 || x == xMax - 1 || z == zMax - 1) { //borders
                                current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, "TowerWoodBlock", true);
                            } else {
                                current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, "DarkWood", true);
                            }
                        }
                    }
                }
            }


            new DarkBeard().makeDarkBeard(current, towerHeight, loc, xMax, zMax);
            new RoofBuilder().build(current, loc + Vector3.UNIT_Y * towerHeight, new Vector2(xMax, zMax), rd);
            new Bridge(this.mSourceBuilding, this.getEntrance(this.mSourceBuilding, 1, loc), new NullTower()).build(current);
        }

        private Vector3 getEntrance(Orientation orientation, int y, Vector3 towerLocation) {
            if(orientation == Orientation.East)       { return towerLocation + new Vector3(xMax - 1, y, zMax / 2); }
            else if(orientation == Orientation.North) { return towerLocation + new Vector3(xMax / 2, y, zMax - 1); }
            else if(orientation == Orientation.South) { return towerLocation + new Vector3(xMax / 2, y, 0);        }
            else                                      { return towerLocation + new Vector3(0, y, zMax / 2);        }
            

        }
    }
}
