using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise;
using LibNoise.Modules;
using LibNoise.Modules.Source;
using LibNoise.Modules.Modifier;
using LibNoise.Modules.Combiner;

using API.Geo.Cuboid;
using API.Generator;

using Mogre;

namespace Game.World.Generator
{
    public class RandomIsland : VanillaIsland
    {
        public RandomIsland(Vector3 islandCoord, Vector2 size) : base(islandCoord, size) {

            if(size.x != size.y) { throw new ArgumentException("Dome Islands can't be non square Islands"); }

            this.generate();
        }

        public override void generate() {
            Vector3 chunkTempPosition = new Vector3(0, 0, 0);

            for (int xx = 0; xx < this.mIslandSize.x * MainWorld.CHUNK_SIDE; xx++) {
                for (int zz = 0; zz < this.mIslandSize.z * MainWorld.CHUNK_SIDE; zz++) {

                        chunkTempPosition.x = xx / MainWorld.CHUNK_SIDE;
                        chunkTempPosition.z = zz / MainWorld.CHUNK_SIDE;
                    
                    for (int yy = 0; yy < MainWorld.MaxHeight; yy++) {

                        chunkTempPosition.y = yy / MainWorld.CHUNK_SIDE;

                        if(!this.hasChunk(chunkTempPosition)) {
                            this.mChunkList.Add(chunkTempPosition, new VanillaChunk(new Vector3(16,16,16), chunkTempPosition, this));
                            if(chunkTempPosition.y > this.mIslandSize.y) {
                                this.mIslandSize.y = chunkTempPosition.y;
                            }
                        }
                                
                        this.getBlock(xx, yy, zz).setMaterial(API.Generic.Material.GRASS);
				    }
                }
            }
        }


    }
}
