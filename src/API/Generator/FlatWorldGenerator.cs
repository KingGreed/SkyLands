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
     * Generates a flat world of an unbreakable material
     */
    public class FlatWorldGenerator {
	    public void generate(int chunkX, int chunkY, int chunkZ, World world) {
		    throw new NotImplementedException();
	    }

	    public Populator[] getPopulators() {
		    return new Populator[0];
	    }

	    public int[,] getSurfaceHeight(World world, int chunkX, int chunkZ) {
		    throw new NotImplementedException();
	    }
    }
}