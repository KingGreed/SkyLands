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
     * Represents an Object for a WorldGenerator
     */
    public abstract class WorldGeneratorObject {
	    /**
	     * Verify if the object can be placed at the given coordinates
	     *
	     * @param c
	     * @param x
	     * @param y
	     * @param z
	     * @return true if the object can be placed, false if it can't
	     */
	    public abstract bool canPlaceObject(World w, int x, int y, int z);

	    /**
	     * Place this object into the world
	     *
	     * @param c
	     * @param x
	     * @param y
	     * @param z
	     */
	    public abstract void placeObject(World w, int x, int y, int z);
    }
}