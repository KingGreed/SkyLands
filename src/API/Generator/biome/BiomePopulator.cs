using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo;
using API.Geo.Cuboid;
using API.Generic;

using Mogre;

namespace API.Generator
{
    /**
     * Provides Biome level population for BiomeGenerator
     */
    public class BiomePopulator : Populator {
	    public override void populate(Chunk chunk, Random random) {
		    Biome biome = chunk.getBiome();
		    if (biome != null) {
			    biome.decorate(chunk, random);
		    }
	    }
    }
}