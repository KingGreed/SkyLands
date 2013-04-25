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
    class MainTower : Tower {
        public int towerNum;
        public const int xMax = 20, zMax = 20;
        List<int> mFloorHeight = new List<int>();
        Orientation mSourceBuilding;

        public MainTower(int num, Orientation sourceBuilding = Orientation.None) { this.towerNum = num; this.mSourceBuilding = sourceBuilding; }

        public void build(Island current, Random rd, Vector3 loc) {
            int towerHeight = rd.Next(60, 88);

            if(towerNum == 1) { loc = loc + 40 * Vector3.UNIT_Y; }

            List<int> floors = new List<int>();
            
            //body
            for(int x = 0; x < xMax; x++) {
                for(int z = 0; z < zMax; z++) {
                    for(int y = 0; y <= towerHeight; y++) {
                        if((x == 0 && z == 0) || (x == 0 && z == zMax - 1) || (x == xMax - 1 && z == 0) || (z == zMax - 1 && x == xMax - 1)) { //corners
                            current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, "TowerWoodBlock", true);
                        } else if((x == 0 || z == 0 || x == xMax - 1 || z == zMax - 1) && y != towerHeight) { //borders
                            current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, "DarkWood", true);
                        }
                        if(x == 0 && z == 0 && y % 9 == 0) { floors.Add(y); }
                        if(y == 0 || y == towerHeight -1) {
                            if(x == 0 || z == 0 || x == xMax - 1 || z == zMax - 1) { //borders
                                current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, "TowerWoodBlock", true);
                            } else {
                                current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, "DarkWood", true);
                            }
                        }
                    }
                }
            }

            Orientation orient = Orientation.None;

            foreach(int f in floors) {
                orient = this.MakeFloorAt(f, current, orient, loc); 
            }

            new RoofBuilder().build(current, loc + Vector3.UNIT_Y * towerHeight, new Vector2(xMax, zMax), rd);

            new DarkBeard().makeDarkBeard(current, towerHeight, loc, xMax, zMax);

            bool newMain = false;
            int i = 0;

            foreach(Orientation o in Enum.GetValues(typeof(Orientation))) {
                if(o == Orientation.None) { continue; }
                if(this.towerNum < 4) {
                    if(i != 3 && !newMain && rd.Next(0, 100) > 48) {
                        new Bridge(o, this.getEntrance(o, towerHeight - 7, loc), new MainTower(this.towerNum + 1, (Orientation)(-(int)o))).build(current);
                        newMain = true;
                    } else if(i == 3 && !newMain) {
                        new Bridge(o, this.getEntrance(o, towerHeight - 7, loc), new MainTower(this.towerNum + 1, (Orientation)(-(int)o))).build(current);
                        newMain = true;
                    } else {
                        new Bridge(o, this.getEntrance(o, towerHeight - 7, loc), new MediumTower((Orientation)(-(int)o))).build(current);
                    }
                } else { new Bridge(o, this.getEntrance(o, towerHeight - 7, loc), new MediumTower((Orientation)(-(int)o))).build(current); }
                if(o == this.mSourceBuilding) { new Bridge(o, this.getEntrance(o, 1, loc), new NullTower()).build(current); }
                else                          { new Bridge(o, this.getEntrance(o, 0, loc), new MediumTower((Orientation)(-(int)o))).build(current); }
                i++;
            }
        }

        private Vector3 getEntrance(Orientation orientation, int y, Vector3 towerLocation) {
            if(orientation == Orientation.East)       { return towerLocation + new Vector3(xMax - 1, y, zMax / 2); }
            else if(orientation == Orientation.North) { return towerLocation + new Vector3(xMax / 2, y, zMax - 1); }
            else if(orientation == Orientation.South) { return towerLocation + new Vector3(xMax / 2, y, 0);        }
            else                                      { return towerLocation + new Vector3(0, y, zMax / 2);        }
        }

        private Vector3 getFloorEntrance(Orientation o, int y, Vector3 towerLocation) {

            if(o == Orientation.None) { return Vector3.UNIT_SCALE; }

            if(o == Orientation.East) { return towerLocation + new Vector3(xMax - 2, y, zMax / 2); }
            else if(o == Orientation.North) { return towerLocation + new Vector3(xMax / 2, y, zMax - 2); }
            else if(o == Orientation.South) { return towerLocation + new Vector3(xMax / 2, y, 1); }
            else { return towerLocation + new Vector3(1, y, zMax / 2); }

        }

        // @return the orientation exit
        private Orientation MakeFloorAt(int y, Island current, Orientation entrance, Vector3 loc) {
            for(int x = 0; x < xMax; x++) {
                for(int z = 0; z < zMax; z++) {
                    if(x == 0 || z == 0 || x == xMax - 1 || z == zMax - 1) { continue; }
                    current.setBlockAt((int)loc.x + x, (int)loc.y + y, (int)loc.z + z, "DarkWood", true);
                }
            }
            Orientation r;
            Vector3 exit;

            if(entrance != Orientation.None) {
                Vector3 entranceFloor = this.getFloorEntrance(entrance, y, loc);
                exit = this.getFloorEntrance((Orientation)(-(int)entrance), y, loc);

                current.setBlockAt((int)entranceFloor.x, (int)entranceFloor.y, (int)entranceFloor.z, "Air", true);
                current.setBlockAt((int)exit.x, (int)exit.y, (int)exit.z, "Levitator", true);
                r = (Orientation)(-(int)entrance);
            } else {
                Random rd = new Random();
                r = (Orientation)rd.Next(-2, 3);
                exit = this.getFloorEntrance(r, y, loc);
                current.setBlockAt((int)exit.x, (int)exit.y, (int)exit.z, "Levitator", true);
            }

            for(int i = 1; i <= 9; i++) { current.setBlockAt((int)exit.x, (int)exit.y + i, (int)exit.z, "Levitator air", true); }

            return r;
        }
    }
}
