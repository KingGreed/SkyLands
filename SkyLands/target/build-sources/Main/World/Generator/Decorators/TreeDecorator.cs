using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;
using API.Geo.Cuboid;

using Game.World.Blocks;
using Game.World;

namespace Game.World.Generator.Decorators
{
    class TreeDecorator : Decorator
    {

        public TreeDecorator() { }

        public override void populate(Island curr, Random random)
        {
            int x = random.Next(2, (int)curr.getSize().x) * MainWorld.CHUNK_SIDE;
            int z = random.Next(2, (int)curr.getSize().z) * MainWorld.CHUNK_SIDE;
            int y = curr.getSurfaceHeight(x, z);

            while (y == -1 && curr.getBlock(x, y, z, true) is GrassBlock)
            {
                x = random.Next(2, (int)curr.getSize().x) * MainWorld.CHUNK_SIDE;
                z = random.Next(2, (int)curr.getSize().z) * MainWorld.CHUNK_SIDE;
                y = curr.getSurfaceHeight(x, z);
            }

            int size = ((int)(curr.getSize().x + curr.getSize().z) / 2) / 5;

            for (int i = 0; i < size * MainWorld.CHUNK_SIDE; i += 5) {
                for (int j = 0, max = random.Next(4, 10); j < max; j++) {
                    double angle = 2.0 * Math.PI * random.NextDouble();

                    int treeX = (int)(x + i * Math.Cos(angle)) + (random.Next(-10, 11) / 5);
                    int treeZ = (int)(y + i * Math.Sin(angle)) + (random.Next(-10, 11) / 5);
                    int treeY = curr.getSurfaceHeight(treeX, treeZ, "Grass");

                    for (int k = 0, height = random.Next(4, 9); k < height; k++) {
                        curr.setBlockAt(treeX, treeY + k, treeZ, "Wood", true);
                        if (k >= height - 3) {
                            for (int l = -5; l < 6; l++) {
                                for (int m = -5; m < 6; m++) {
                                    if (m == 0 && l == 0) { continue; }
                                    if (l >= -3 && l <= 3 && m >= -3 && m <= 3) {
                                        curr.setBlockAt(treeX + l, treeY + k, treeZ + m, "Leaves", true);
                                    }
                                }
                            }
                        }
                    }
                }
            }



        }

    }
}
