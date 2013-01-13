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

using Material = API.Generic.Material;

namespace Game.World.Generator
{
    public class RandomIsland : VanillaIsland
    {

        private  Perlin     ELEVATION  = new Perlin();
	    private  Perlin     ROUGHNESS  = new Perlin();
	    private  Perlin     DETAIL     = new Perlin();
	    private  Turbulence TURBULENCE = new Turbulence();
	    private  ScalePoint SCALE      = new ScalePoint();
	    private  Clamp      FINAL      = new Clamp();
	    // smoothing stuff
	    //private  int SMOOTH_SIZE = 4;

        public RandomIsland(Vector3 islandCoord, Vector2 size) : base(islandCoord, size) {

            if(size.x != size.y) { throw new ArgumentException("Dome Islands can't be non square Islands"); }

            ELEVATION.setFrequency(0.2);
		    ELEVATION.setLacunarity(1);
		    ELEVATION.setNoiseQuality(NoiseQuality.STANDARD);
		    ELEVATION.setPersistence(0.7);
		    ELEVATION.setOctaveCount(1);

		    DETAIL.setFrequency(0.7);
		    DETAIL.setLacunarity(1);
		    DETAIL.setNoiseQuality(NoiseQuality.STANDARD);
		    DETAIL.setPersistence(0.7);
		    DETAIL.setOctaveCount(1);

		    Multiply multiply = new Multiply();
		    multiply.SetSourceModule(0, ROUGHNESS);
		    multiply.SetSourceModule(1, DETAIL);

		    Add add = new Add();
		    add.SetSourceModule(0, multiply);
		    add.SetSourceModule(1, ELEVATION);

		    SCALE.SetSourceModule(0, add);
		    SCALE.setxScale(0.03);
		    SCALE.setyScale(0.06);
		    SCALE.setzScale(0.03);

		    TURBULENCE.SetSourceModule(0, SCALE);
		    TURBULENCE.setFrequency(0.01);
		    TURBULENCE.setPower(6);
		    TURBULENCE.setRoughness(1);

		    FINAL.SetSourceModule(0, SCALE);
		    FINAL.setLowerBound(-1);
		    FINAL.setUpperBound(1);

            this.generate();
        }

        public override void generate() {

            int seed = 42;

            Vector3 chunkCoord = new Vector3(0, 0, 0),
                    blockCoord = new Vector3(0, 0, 0);

            double minElevation, smoothHeight;
            float maxSum, minSum;

            ELEVATION.setSeed((int) seed * 23);
		    DETAIL.setSeed((int) seed * 17);
		    TURBULENCE.setSeed((int) seed * 53);

            Random random = new Random(seed);
		    double[, ,] noise  = WorldGeneratorUtils.fastNoise(FINAL, 16*(int)this.mIslandSize.x, 256, 16*(int)this.mIslandSize.z, 4, 0, 0, 0);


            LogManager.Singleton.DefaultLog.LogMessage("Perlin set");

            Vector3 chunkTempPosition = new Vector3(0, 0, 0);


            for (int xx = 0; xx < this.mIslandSize.x * MainWorld.CHUNK_SIDE; xx++) {
                for (int zz = 0; zz < this.mIslandSize.z * MainWorld.CHUNK_SIDE; zz++) {

                    chunkTempPosition.x = xx / MainWorld.CHUNK_SIDE;
                    chunkTempPosition.z = zz / MainWorld.CHUNK_SIDE;

				    maxSum = 32.5f;
				    minSum = 160;
				    
				    minElevation = minSum;
				    smoothHeight = (maxSum - minElevation) / 2d;
				    for (int yy = 0; yy < 256; yy++) {
                        
                        chunkTempPosition.y = yy / MainWorld.CHUNK_SIDE;

                        if(!this.hasChunk(chunkTempPosition)) {
                            this.mChunkList.Add(chunkTempPosition, new VanillaChunk(new Vector3(16,16,16), chunkTempPosition, this));
                            if(chunkTempPosition.y > this.mIslandSize.y) {
                                this.mIslandSize.y = chunkTempPosition.y;
                            }
                        }
					    
                        double noiseValue = noise[xx, yy, zz] * noise[xx, yy, zz] - System.Math.Abs(1 / smoothHeight * (yy - smoothHeight - minElevation));

					    Block block = this.getBlock(new Vector3(xx, yy, zz));
                        if(block == null) { continue; }

                        if (noiseValue >= 0) { block.setMaterial(Material.GRASS); }
                        else                 { block.setMaterial(Material.AIR); }
				    }
			    }
            }

        }


    }
}
