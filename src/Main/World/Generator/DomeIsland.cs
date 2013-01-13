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
    public class DomeIsland : VanillaIsland
    {
        private Perlin ELEVATION  = new Perlin();

        public DomeIsland(Vector3 islandCoord, Vector2 size) : base(islandCoord, size) {

            if(size.x != size.y) { throw new ArgumentException("Dome Islands can't be non square Islands"); }

            ELEVATION.setFrequency(0.2);
		    ELEVATION.setLacunarity(1);
		    ELEVATION.setNoiseQuality(NoiseQuality.STANDARD);
		    ELEVATION.setPersistence(0.7);
		    ELEVATION.setOctaveCount(1);

            this.generate();

            this.trees();
        }

        public override void generate() {
            Vector3 chunkTempPosition = new Vector3(0, 0, 0);

            for (int xx = 0; xx < this.mIslandSize.x * MainWorld.CHUNK_SIDE; xx++) {
                for (int zz = 0; zz < this.mIslandSize.z * MainWorld.CHUNK_SIDE; zz++) {
                    chunkTempPosition.x = xx / MainWorld.CHUNK_SIDE;
                    chunkTempPosition.z = zz / MainWorld.CHUNK_SIDE;
                    for (int yy = 0; yy < MainWorld.MaxHeight; yy++) {
                        chunkTempPosition.y = yy / MainWorld.CHUNK_SIDE;
                        
                        if(yy <= 100) {
                            if(this.isInSphere(xx, yy, zz)) {

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

        public void trees() {
            Random rd = new Random();
            int amount = rd.Next(20);

            for(int i = 0; i < amount; i++) {
                int x = rd.Next(16*(int)this.mIslandSize.x);
			    int z = rd.Next(16*(int)this.mIslandSize.z);
                int height = rd.Next(3, 8);

                if(this.getBlock(x, 101, z).getMaterial() != API.Generic.Material.WOOD) {
                    for(int j = 0; j < height; j++) {
                        this.getBlock(x, 101 + j, z).setMaterial(API.Generic.Material.WOOD);
                    }
                }
            }
        }

        public bool isInSphere(int x, int y, int z) {
            int x0 = (int) (this.mIslandSize.x * MainWorld.CHUNK_SIDE / 2),
                y0 = 100, 
                z0 = (int) (this.mIslandSize.z * MainWorld.CHUNK_SIDE / 2);

            return (((x-x0) * (x-x0) + (y-y0) * (y-y0) + (z-z0) * (z-z0)) < x0 * x0);
        }


    }
}
