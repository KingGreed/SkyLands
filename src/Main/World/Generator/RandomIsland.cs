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

using Game.World.Blocks;

using Mogre;

using Material = API.Generic.Material;

namespace Game.World.Generator
{
    public class RandomIsland : VanillaIsland
    {

        private Perlin     ELEVATION  = new Perlin();
	    private Perlin     ROUGHNESS  = new Perlin();
	    private Perlin     DETAIL     = new Perlin();
	    private Turbulence TURBULENCE = new Turbulence();
	    private ScalePoint SCALE      = new ScalePoint();
	    private Clamp      FINAL      = new Clamp();

        public RandomIsland(SceneNode node, Vector2 size, Biome islandBiome, MainWorld currentWorld) : base(node, size, currentWorld) {

            this.mBiome = islandBiome;

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

            this.generate(42);
        }

        public override void generate(int seed) {

            Vector3 chunkCoord = new Vector3(0, 0, 0),
                    blockCoord = new Vector3(0, 0, 0);

            double minElevation, smoothHeight;
            double maxSum, minSum;

            Random rd = new Random();

            ELEVATION.setSeed((int) seed * 23);
		    DETAIL.setSeed((int) seed * 17);
		    TURBULENCE.setSeed((int) seed * 53);

		    double[, ,] noise  = WorldGeneratorUtils.fastNoise(FINAL, 16*(int)this.mIslandSize.x, 256, 16*(int)this.mIslandSize.z, 4, 0, 0, 0);
            double[, ,] noise2 = WorldGeneratorUtils.fastNoise(FINAL, 16*(int)this.mIslandSize.x, 256, 16*(int)this.mIslandSize.z, 4, 0, 10, 0);

            LogManager.Singleton.DefaultLog.LogMessage("Perlin generated");

            Vector3 chunkTempPosition = new Vector3(0, 0, 0);

            maxSum = this.mBiome.getMax();
            minSum = this.mBiome.getMin();
				    
			minElevation = minSum;
			smoothHeight = (maxSum - minElevation) / 2d;
            int dist = -1;

            for (int xx = 0; xx < this.mIslandSize.x * MainWorld.CHUNK_SIDE; xx++) {
                for (int zz = 0; zz < this.mIslandSize.z * MainWorld.CHUNK_SIDE; zz++) {

                    chunkTempPosition.x = xx / MainWorld.CHUNK_SIDE;
                    chunkTempPosition.z = zz / MainWorld.CHUNK_SIDE;

				    for (int yy = 250; yy > 0; yy--) {
                        
                        //double noiseValue = noise[xx, yy, zz] * noise[xx, yy, zz] + noise[xx, yy, zz] - System.Math.Abs(1 / smoothHeight * (yy - smoothHeight - minElevation));
                        //double noiseValue = (noise[xx, yy, zz] / 4d) - ((System.Math.Abs(yy - 64 - 32)) - 2 * noise2[xx, 255 - yy, zz] )/ 128.0;
                        double noiseValue = (noise[xx, yy, zz]) * noise2[xx, 255 - yy, zz] + noise[xx, yy, zz] - System.Math.Abs(1 / smoothHeight * (yy - smoothHeight - minElevation + 20));

                        if (noiseValue >= 0) {
                            
                            if(this.getBlock(xx, yy + 1, zz, false) is AirBlock) {

                                if (this.mBiome.getGroundCover()[0].Length == 1)
                                {
                                    this.setBlockAt(xx, yy, zz, this.mBiome.getGroundCover()[0][0], true);
                                    continue;
                                }
                                int rdRes = rd.Next(0, 100);
                                if (rdRes <= 60)
                                {
                                    this.setBlockAt(xx, yy, zz, this.mBiome.getGroundCover()[0][0], true);
                                }
                                else if (rdRes <= 80)
                                {
                                    this.setBlockAt(xx, yy, zz, this.mBiome.getGroundCover()[0][1], true);
                                }
                                else
                                {
                                    this.setBlockAt(xx, yy, zz, this.mBiome.getGroundCover()[0][2], true);
                                }
                            } else {
                                dist = this.isNearSurface(this.mBiome.getGroundCover().Count - 1, new Vector3(xx, yy, zz));
                                if(dist != -1) {
                                    this.setBlockAt(xx, yy, zz, this.mBiome.getGroundCover()[dist][rd.Next(0, this.mBiome.getGroundCover()[dist].Length)], true);
                                }
                                else { this.setBlockAt(xx, yy, zz, "Stone", true); }
                            }
                    	}
                        //else                 { block.setMaterial(Material.AIR); } // Not needed
				    }
			    }
            }
            LogManager.Singleton.DefaultLog.LogMessage("Perlin set");
            this.mBiome.decorate(this, new Random());
            LogManager.Singleton.DefaultLog.LogMessage("Island decorated");

        }
        //@return -1 if isFalse else : num between 0 and near
        private int isNearSurface(int near, Vector3 loc) {
            for(int i = 0; i <= near; i++) {
                if(this.getBlock((int)loc.x, (int)loc.y + i + 1, (int)loc.z, false) is AirBlock) {
                    return i;
                }
            }
            return -1;
            
        }


        private bool isValid(int x, int y, int z) {
        	Vector3 center    = new Vector3(this.mIslandSize.x / 2, 256 / 2, this.mIslandSize.z / 2);
            Vector3 distance  = new Vector3(x, y, z) - center;
            Vector3 zeroPoint = new Vector3(this.mIslandSize.x, 256 / 2, this.mIslandSize.z / 2);
            

            
            return true;
        }
    }
}