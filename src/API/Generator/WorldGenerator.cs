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
    public interface WorldGenerator {
	    /**
	     * Gets the block structure for a Chunk.
	     *
	     * It is recommended that seeded random number generators from
	     * WorldGeneratorUtils are used.
	     *
	     * @param chunkX coordinate
	     * @param chunkY coordinate
	     * @param chunkZ coordinate
	     * @param world in which is generated
	     */
	    void generate(int chunkX, int chunkY, int chunkZ, World world);

	    /**
	     * Gets the surface height of the world. This is used for initialisation
	     * purposed only, so only needs reasonable accuracy.<br> <br> The result
	     * value should be a 2d array of size {@link  org.spout.api.geo.cuboid.Chunk#CHUNK_SIZE}
	     * squared.<br> <br> This hint will improve lighting calculations for
	     * players who move into new areas.
	     *
	     * @param chunkX coordinate
	     * @param chunkZ coordinate
	     * @return the surface height array for the column, or null not to provide a
	     * hint
	     */
	    int[,] getSurfaceHeight(World world, int chunkX, int chunkZ);

	    /**
	     * Gets an array of Populators for the world generator
	     *
	     * @return the Populator array
	     */
	    Populator[] getPopulators();
	
	    /**
	     * Gets an array of GeneratorPopulators for the world generator
	     *
	     * @return the GeneratorPopulator array
	     */
	    GeneratorPopulator[] getGeneratorPopulators();
    }
}