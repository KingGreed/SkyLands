using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;
using API.Generator;

using Mogre;

namespace Game.World.Generator
{
    class SinusIsland : VanillaIsland
    {
        public SinusIsland(Vector3 islandCoord, Vector2 size) : base(islandCoord, size) {
            this.generate(34);
        }

        public override void generate(int seed) {
            Vector3 chunkTempPosition = new Vector3(0, 0, 0);

            for (int xx = 0; xx < this.mIslandSize.x * MainWorld.CHUNK_SIDE; xx++) {
                for (int zz = 0; zz < this.mIslandSize.z * MainWorld.CHUNK_SIDE; zz++) {
                    chunkTempPosition.x = xx / MainWorld.CHUNK_SIDE;
                    chunkTempPosition.z = zz / MainWorld.CHUNK_SIDE;
                    for (int yy = 0; yy < MainWorld.MaxHeight; yy++) {
                        chunkTempPosition.y = yy / MainWorld.CHUNK_SIDE;
                        
                        if(yy <= 100) {
                            if(this.isInSinus(xx, yy)) {

                                this.checkAndUpdate(chunkTempPosition);
                                
                                this.getBlock(xx, yy, zz).setMaterial(API.Generic.Material.GRASS);
                            }
                        } else if(yy == 112) {
                            this.checkAndUpdate(chunkTempPosition);
                            this.getBlock(xx, yy, zz).setMaterial(API.Generic.Material.WOOD);
                        }

				    }
                }
            }
        }

        public bool isInSinus(int x, int y) {
            return 3*System.Math.Sin(x + System.Math.PI) + 10 > y;

        }
    }
}
