using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo;
using API.Generic;

using Mogre;

namespace API.Generator
{
    /**
     * Represents a populator for a generator
     */
    public interface GeneratorPopulator {
	    void populate(int chunkX, int chunkY, int chunkZ, Biome biome, World world);
    }
}