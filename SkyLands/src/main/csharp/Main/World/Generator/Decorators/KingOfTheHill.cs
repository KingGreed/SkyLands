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
    class KingOfTheHill : Decorator
    {
        public KingOfTheHill() { }



        public override void populate(Island curr, Random random) {
            Vector3 p = new Vector3(curr.getSize().x * 8, curr.getSize().y * 16 + 16, curr.getSize().z * 8);
            Vector3 center = p + (8) * Vector3.UNIT_X  + (8) * Vector3.UNIT_Y + (8) * Vector3.UNIT_Z;

            for(float x = p.x; x < p.x + 18; x++) {
                for(float y = p.y; y < p.y + 18; y++) {
                    for(float z = p.z; z < p.z + 16; z++) {
                        if(isInSphere(x, y, z, center.x, center.y, center.z, 9)) {
                            curr.setBlockAt((int)x, (int)y, (int)z, 2, true);
                        }
                        if(y == center.y) {
                            curr.setBlockAt((int)x, (int)y, (int)z, "Grass", true);
                        }
                        
                    }
                }
            }

            createTree(curr, random, (int)center.x, (int)center.y + 1, (int)center.z);
            createTree(curr, random, (int)center.x + 7, (int)center.y + 1, (int)center.z - 3);
            createTree(curr, random, (int)center.x, (int)center.y + 1, (int)center.z + 7);
            createTree(curr, random, (int)center.x + 2, (int)center.y + 1, (int)center.z + 7);
            createTree(curr, random, (int)center.x + 5, (int)center.y + 1, (int)center.z - 7);
            createTree(curr, random, (int)center.x + 4, (int)center.y + 1, (int)center.z + 4);

            createTree(curr, random, (int)center.x - 2, (int)center.y + 1, (int)center.z + 3);
            createTree(curr, random, (int)center.x - 2, (int)center.y + 1, (int)center.z + 4);
            createTree(curr, random, (int)center.x - 6, (int)center.y + 1, (int)center.z - 7);
            createTree(curr, random, (int)center.x - 5, (int)center.y + 1, (int)center.z - 5);
            createTree(curr, random, (int)center.x - 8, (int)center.y + 1, (int)center.z - 3);

        }

        public bool isInSphere(float x, float y, float z, float x0, float y0, float z0, float r) {
            return ((x - x0) * (x - x0) + (y - y0) * (y - y0) + (z - z0) * (z - z0) <= r * r) && y < y0;
        }

        public void createTree(Island curr, Random random, int x, int y, int z) {
            for(int i = 0, height = random.Next(6, 12); i < height; i++) {
                if(i != height - 1) { curr.setBlockAt(x, y + i, z, "Wood", true); } else { curr.setBlockAt(x, y + i, z, "Leaves", true); }
                if(i >= height - 5 && i < height - 3) {
                    for(int j = -2; j < 3; j++) {
                        for(int k = -2; k < 3; k++) {
                            if(j == 0 && k == 0) { continue; }
                            if((j == -2 && (k == -2 || k == 2)) || (j == 2 && (k == -2 || k == 2))) {
                                if(random.Next(0, 100) > 15) { curr.setBlockAt(x + j, y + i, z + k, "Leaves", true); }
                            } else {
                                curr.setBlockAt(x + j, y + i, z + k, "Leaves", true);
                            }
                        }
                    }
                } else if(i >= height - 3) {
                    for(int j = -1; j < 2; j++) {
                        for(int k = -1; k < 2; k++) {
                            if(j == 0 && k == 0) { continue; }
                            if(i == height - 3 && (j == -1 && (k == -1 || k == 1)) || (j == 1 && (k == -1 || k == 1))) {
                                if(random.Next(0, 100) > 75) { curr.setBlockAt(x + j, y + i, z + k, "Leaves", true); }
                            } else {
                                curr.setBlockAt(x + j, y + i, z + k, "Leaves", true);
                            }
                        }
                    }
                }

            }

        }
    }
}
