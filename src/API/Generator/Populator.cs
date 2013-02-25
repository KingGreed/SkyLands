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
	     * This method will be called once per chunk
	     *
	     * @param chunk the chunk to populate
	     * @param random The RNG for this chunk
	     */
        public abstract void populate(Island curr, Random random);
    }
}