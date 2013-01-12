using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using LibNoise;
using LibNoise.Modules;
using LibNoise.Modules.Source;
using LibNoise.Modules.Modifier;
using LibNoise.Modules.Combiner;

using Material = API.Generic.Material;
using Game.Generation;
using Game.Generation.Biomes;

using Math = System.Math;

namespace Game.Terrain
{
    public class Island
    {
        public  int HEIGHT;
	    public  int SEA_LEVEL = 63;
	    // noise for generation
	    private  Perlin     ELEVATION  = new Perlin();
	    private  Perlin     ROUGHNESS  = new Perlin();
	    private  Perlin     DETAIL     = new Perlin();
	    private  Turbulence TURBULENCE = new Turbulence();
	    private  ScalePoint SCALE      = new ScalePoint();
	    private  Clamp      FINAL      = new Clamp();
	    // smoothing stuff
	    private  int SMOOTH_SIZE = 4;

        private Vector2 mIslandsDim;

        public Island(Vector2 islandDim){
            this.mIslandsDim = islandDim;
            this.createIslandAt(new Vector3(0, 0, 0));

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
		    TURBULENCE.setPower(8);
		    TURBULENCE.setRoughness(1);

		    FINAL.SetSourceModule(0, SCALE);
		    FINAL.setLowerBound(-1);
		    FINAL.setUpperBound(1);

            HEIGHT = 256;
            this.generateTerrain(42);
        }

        public void createIslandAt(Vector3 coord){
            Vector3 chunkPos;
            for (int x = 0; x < this.mIslandsDim.x; x++) {
                for (int z = 0; z < this.mIslandsDim.y; z++) {
                    chunkPos  = coord + new Vector3(x, 0, z);
                    World.chunkArray.Add(new Vector3(chunkPos.x, chunkPos.y, chunkPos.z), new Chunk());
                }
            }
        }

        public void generateTerrain(int seed){


            Vector3 chunkCoord = new Vector3(0, 0, 0),
                    blockCoord = new Vector3(0, 0, 0);

            double minElevation, smoothHeight;
            float maxSum, minSum;

            ELEVATION.setSeed((int) seed * 23);
		    DETAIL.setSeed((int) seed * 17);
		    TURBULENCE.setSeed((int) seed * 53);

            Random random = new Random(seed);
		    double[, ,] noise  = this.fastNoise(FINAL, new Vector3(20*this.mIslandsDim.x, 256, 20*this.mIslandsDim.y), 4, new Vector3(0, 0, 0));
            double[, ,] noise2 = this.fastNoise(FINAL, new Vector3(20*this.mIslandsDim.x, 256, 20*this.mIslandsDim.y), 4, new Vector3(0, 0, 0));
            SkyLandsBiomeSelector selector = new SkyLandsBiomeSelector();

            LogManager.Singleton.DefaultLog.LogMessage("Perlin set");

            Vector3 chunkTempPosition = new Vector3(0, 0, 0),
                    blockTempPosition = new Vector3(0, 0, 0);


            for (int xx = 0; xx < this.mIslandsDim.x * World.CHUNK_SIDE; xx++) {
                for (int zz = 0; zz < this.mIslandsDim.y * World.CHUNK_SIDE; zz++) {

                    blockTempPosition.x = xx % World.CHUNK_SIDE;
                    blockTempPosition.z = zz % World.CHUNK_SIDE;

                    chunkTempPosition.x = xx / World.CHUNK_SIDE;
                    chunkTempPosition.z = zz / World.CHUNK_SIDE;

				    maxSum = 0;
				    minSum = 0;
				    byte count = 0;
				    for (int sx = -SMOOTH_SIZE; sx <= SMOOTH_SIZE; sx++) {
					    for (int sz = -SMOOTH_SIZE; sz <= SMOOTH_SIZE; sz++) {
						    Biome adjacent = (Biome) selector.pickBiome(xx + sx, 0, zz + sz, seed);
                            //LogManager.Singleton.DefaultLog.LogMessage(adjacent.GetType().ToString());
						    
						    minSum += adjacent.getMin();
						    maxSum += adjacent.getMax();
						    count++;
					    }
				    }
				    minElevation = minSum / count;
				    smoothHeight = (maxSum / count - minElevation) / 2d;
				    for (int yy = 0; yy < this.HEIGHT; yy++) {
                        
                        chunkTempPosition.y = yy / World.CHUNK_SIDE;
                        blockTempPosition.y = yy % World.CHUNK_SIDE;

                        if(!World.hasChunk(chunkTempPosition)) { World.chunkArray.Add(chunkTempPosition, new Chunk()); }
					    
                        double noiseValue = noise[xx, yy, zz] * noise[xx, yy, zz] - Math.Abs(1 / smoothHeight * (yy - smoothHeight - minElevation));

					    Block block = World.getBlock(chunkTempPosition, blockTempPosition);
                        if(block == null) { continue; }

                        if (noiseValue >= 0) { block.setType(Material.GRASS); }
                        else                 { block.setType(Material.AIR); }
				    }
			    }
            }

        }

        private void setColHeight(Vector3 chunkPos, Vector3 blockPos, double lowerPosition, double upperPosition) {
            int i = 0;
            Vector3 chunkTempPosition = chunkPos,
                    blockTempPosition = blockPos;

            while(i < upperPosition) {
                chunkTempPosition.y = i / World.CHUNK_SIDE;
                blockTempPosition.y = i % World.CHUNK_SIDE;
                
                if(!World.hasChunk(chunkTempPosition)) { World.chunkArray.Add(chunkTempPosition, new Chunk()); }
                Block block = World.getBlock(chunkTempPosition, blockTempPosition);
                if(block != null)
                {
                    if (i <= lowerPosition || i > upperPosition) { block.setType(Material.AIR); }
                    else                                         { block.setType(Material.GRASS); }
                }

                i++;
            }

        }

        public double[ , ,] fastNoise(Module noiseGenerator, Vector3 size, int samplingRate, Vector3 coord){
            if (noiseGenerator == null) {
		        throw new ArgumentException("noiseGenerator cannot be null");
	        }
	        if (samplingRate == 0) {
		        throw new ArgumentException("samplingRate cannot be 0");
	        }
	        if (size.x % samplingRate != 0) {
		        throw new ArgumentException("xSize % samplingRate must return 0");
	        }
	        if (size.y % samplingRate != 0) {
		        throw new ArgumentException("ySize % samplingRate must return 0");
	        }
	        if (size.z % samplingRate != 0) {
		        throw new ArgumentException("zSize % samplingRate must return 0");
	        }
	        double[, ,] noiseArray = new double[(int)size.x + 1, (int)size.y + 1, (int)size.z + 1];
	        for (int xx = 0; xx <= size.x; xx += samplingRate) {
		        for (int yy = 0; yy <= size.y; yy += samplingRate) {
			        for (int zz = 0; zz <= size.z; zz += samplingRate) {
				        noiseArray[xx, yy, zz] = noiseGenerator.GetValue(xx + coord.x, coord.y + yy, coord.z + zz);
			        }
		        }
	        }
            int nx, ny, nz;
	        for (int xx = 0; xx < size.x; xx++) {
		        for (int yy = 0; yy < size.y; yy++) {
			        for (int zz = 0; zz < size.z; zz++) {
				        if (xx % samplingRate != 0 || yy % samplingRate != 0 || zz % samplingRate != 0) {
					        nx = (xx / samplingRate) * samplingRate;
					        ny = (yy / samplingRate) * samplingRate;
					        nz = (zz / samplingRate) * samplingRate;
					        noiseArray[xx, yy, zz] = MathHelper.triLerp(xx, yy, zz,
							        noiseArray[nx, ny, nz], noiseArray[nx, ny + samplingRate, nz],
							        noiseArray[nx, ny, nz + samplingRate], noiseArray[nx, ny + samplingRate, nz + samplingRate],
							        noiseArray[nx + samplingRate, ny, nz], noiseArray[nx + samplingRate, ny + samplingRate, nz],
							        noiseArray[nx + samplingRate, ny, nz + samplingRate], noiseArray[nx + samplingRate, ny + samplingRate, nz + samplingRate],
							        nx, nx + samplingRate, ny, ny + samplingRate, nz, nz + samplingRate);
				        }
			        }
		        }
	        }
	        return noiseArray;
        }
    }
}
