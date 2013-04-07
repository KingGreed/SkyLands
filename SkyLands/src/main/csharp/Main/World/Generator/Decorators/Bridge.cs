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
    class Bridge {
        private Orientation mOrientation;
        private Vector3  mLocation;
        private Tower    mBridgeTo;

        public Bridge(Orientation o, Vector3 loc, Tower bridgeTo) {
            this.mOrientation = o;
            this.mLocation = loc;
            this.mBridgeTo = bridgeTo;
        }

        public void build(Island current) {
            this.makeEntrance(current);
            this.makeInner(current);
            switch(this.mOrientation) {
                case Orientation.East : this.mLocation -= new Vector3(-6, -1, MainTower.zMax / 2); break;
                case Orientation.West: this.mLocation -= new Vector3(MainTower.xMax + 5, -1, MainTower.zMax / 2); break;
                case Orientation.North: this.mLocation += new Vector3(-MainTower.xMax / 2, -1, 6); break;
                case Orientation.South: this.mLocation -= new Vector3(MainTower.xMax / 2, -1, MainTower.zMax + 5); break;
            }
            this.mBridgeTo.build(current, new Random(), this.mLocation);
        }

        public void makeEntrance(Island current) { //note that loc is the bottom left point
            for(int i = -2; i < 3; i++) {
                for(int y = 0; y < 5; y++) {
                    switch(this.mOrientation) {
                        case Orientation.East:
                            if(i == -2 || i == 2 || y == 0 || y == 4) {
                                current.setBlockAt((int)this.mLocation.x, (int)this.mLocation.y + y, (int)this.mLocation.z + i, "TowerWoodBlock", true);
                            } else {
                                current.setBlockAt((int)this.mLocation.x, (int)this.mLocation.y + y, (int)this.mLocation.z + i, "Air", true);
                            }
                        break;
                        case Orientation.West:
                            if(i == -2 || i == 2 || y == 0 || y == 4) {
                                current.setBlockAt((int)this.mLocation.x, (int)this.mLocation.y + y, (int)this.mLocation.z - i, "TowerWoodBlock", true);
                            } else {
                                current.setBlockAt((int)this.mLocation.x, (int)this.mLocation.y + y, (int)this.mLocation.z - i, "Air", true);
                            }
                        break;
                        case Orientation.North:
                            if(i == -2 || i == 2 || y == 0 || y == 4) {
                                current.setBlockAt((int)this.mLocation.x + i, (int)this.mLocation.y + y, (int)this.mLocation.z, "TowerWoodBlock", true);
                            } else {
                                current.setBlockAt((int)this.mLocation.x + i, (int)this.mLocation.y + y, (int)this.mLocation.z, "Air", true);
                            }
                        break;
                        default :
                            if(i == -2 || i == 2 || y == 0 || y == 4) {
                                current.setBlockAt((int)this.mLocation.x - i, (int)this.mLocation.y + y, (int)this.mLocation.z, "TowerWoodBlock", true);
                            } else {
                                current.setBlockAt((int)this.mLocation.x - i, (int)this.mLocation.y + y, (int)this.mLocation.z, "Air", true);
                            }
                        break;

                    }
                }
            }
        }

        public void makeInner(Island current) {
            for(int x = -2; x < 3; x++) {
                for(int y = 0; y < 5; y++) {
                    for(int z = -2; z < 3; z++) {

                        switch(this.mOrientation) {
                            case Orientation.East :
                                if((y == 0 && z == -2) || (y == 0 && z == 2) || (y == 4 && z == -2) || (y == 4 && z == 2)) {
                                    current.setBlockAt((int)this.mLocation.x + x + 3, (int)this.mLocation.y + y, (int)this.mLocation.z + z, "TowerWoodBlock", true);
                                } else if((z == -2 || z == 2 || y == 0 || y == 4)){
                                    current.setBlockAt((int)this.mLocation.x + x + 3, (int)this.mLocation.y + y, (int)this.mLocation.z + z, "DarkWood", true);
                                }
                            break;
                            case Orientation.West :
                                if((y == 0 && z == -2) || (y == 0 && z == 2) || (y == 4 && z == -2) || (y == 4 && z == 2)) {
                                    current.setBlockAt((int)this.mLocation.x - x - 3, (int)this.mLocation.y + y, (int)this.mLocation.z - z, "TowerWoodBlock", true);
                                } else if((z == -2 || z == 2 || y == 0 || y == 4)) {
                                    current.setBlockAt((int)this.mLocation.x - x - 3, (int)this.mLocation.y + y, (int)this.mLocation.z - z, "DarkWood", true);
                                }
                            break;
                            case Orientation.North:
                                if((y == 0 && x == -2) || (y == 0 && x == 2) || (y == 4 && x == -2) || (y == 4 && x == 2)) {
                                    current.setBlockAt((int)this.mLocation.x + x, (int)this.mLocation.y + y, (int)this.mLocation.z + z + 3, "TowerWoodBlock", true);
                                } else if((x == -2 || x == 2 || y == 0 || y == 4)) {
                                    current.setBlockAt((int)this.mLocation.x + x, (int)this.mLocation.y + y, (int)this.mLocation.z + z + 3, "DarkWood", true);
                                }
                            break;
                            case Orientation.South:
                                if((y == 0 && x == -2) || (y == 0 && x == 2) || (y == 4 && x == -2) || (y == 4 && x == 2)) {
                                    current.setBlockAt((int)this.mLocation.x - x, (int)this.mLocation.y + y, (int)this.mLocation.z - z - 3, "TowerWoodBlock", true);
                                } else if((x == -2 || x == 2 || y == 0 || y == 4)) {
                                    current.setBlockAt((int)this.mLocation.x - x, (int)this.mLocation.y + y, (int)this.mLocation.z - z - 3, "DarkWood", true);
                                }
                            break;

                        }
                    }
                }
            }


        }


    }
}
