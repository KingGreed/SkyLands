using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

using API.Generator;
using API.Geo.Cuboid;

using Game.World.Blocks;
using Game.World;

using Math = System.Math;

namespace Game.World.Generator.Decorators {
    class Portals : Decorator {
        public Portals() { }

        public override void populate(Island curr, Random random) {
            Vector3 islandCenter = (curr.getSize() / 2) * 16;
            double entranceX = 0, entranceZ = 0;
            double exitX = 0, exitZ = 0;
            for (int radius = (int)islandCenter.x; radius > 0; radius--) {
                double angle = 2.0 * Math.PI * random.NextDouble();
                entranceX = islandCenter.x + Math.Cos(angle) * radius;
                entranceZ = islandCenter.z + Math.Sin(angle) * radius;

                exitX = islandCenter.x - Math.Cos(angle) * radius;
                exitZ = islandCenter.z - Math.Sin(angle) * radius;
                if(curr.getSurfaceHeight((int)entranceX, (int)entranceZ) != -1) { break; }
			}
            Vector3 entrancePortal = new Vector3((float)entranceX, curr.getSurfaceHeight((int)entranceX, (int)entranceZ), (float)entranceZ);

            makePortal(random.Next(1, 3), curr, entrancePortal);
        }

        public static void makePortal(int val, Island current, Vector3 position) {
            switch(val) {
                case 1:
                    for(int x = 0; x < 4; x++) {
                        for(int y = -1; y < 5; y++) {
                            if(x == 0 || x == 3 || y == -1 || y == 4) { current.setBlockAt((int)position.x + x, (int)position.y + y, (int)position.z, "Crystal", true); }
                            else { current.setBlockAt((int)position.x + x, (int)position.y + y, (int)position.z, "CrystalGate", true); }
                        }
                    }
                break;
                case 2:
                for(int x = 0; x < 4; x++) {
                    for(int z = 0; z < 4; z++) {
                        if(x == 0 || z == 0 || x == 3 || z == 3) { current.setBlockAt((int)position.x + x, (int)position.y, (int)position.z + z, "Crystal", true); }
                        else {
                            current.setBlockAt((int)position.x + x, (int)position.y, (int)position.z + z, "CrystalGate H", true);
                        }
                    }
                }

                break;

            }
        }
    }
}
