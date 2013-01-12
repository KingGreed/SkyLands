using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Generic;
using API.Generator;

namespace API.Geo
{
    public interface AreaBlockSource {
	    /**
	     * Gets the material for the block at (x, y, z)
	     *
	     * @param x coordinate of the block
	     * @param y coordinate of the block
	     * @param z coordinate of the block
	     * @return the block's material from the snapshot
	     */
	    Material getBlockMaterial(int x, int y, int z);
	
    }
}