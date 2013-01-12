using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.Generation.Biomes;

namespace Game.Generation
{
    public abstract class BiomeSelector {
        public BiomeMap parent;

	    public Biome pickBiome(int x, int z, int seed) {
		    return pickBiome(x, 0, z, seed);
	    }

	    /**
	     * The logic to select the biome at the given x,y,z. The value must be
	     * between 0 and maxBiomes
	     *
	     * @param x
	     * @param y
	     * @param z
	     * @param seed
	     * @return the biome between 0 and maxBiomes
	     */
	    public abstract Biome pickBiome(int x, int y, int z, int seed);
    }
}
