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
     * Represents a populator for a chunk
     */
    public abstract class Populator {

	    public Populator() {}


	    /**
	     * Populates the chunk.
	     *
	     * This method may make full use of the block modifying methods of the API.
	     *
	     * This method will be called once per chunk and it is guaranteed that a
	     * 2x2x2 cube of chunks containing the chunk will be loaded.
	     *
	     * The chunk to populate is the chunk with the lowest x, y and z coordinates
	     * of the cube.
	     *
	     * This allows the populator to create features that cross chunk boundaries.
	     *
	     * @param chunk the chunk to populate
	     * @param random The RNG for this chunk
	     */
	    public abstract void populate(Chunk chunk, Random random);
    }
}