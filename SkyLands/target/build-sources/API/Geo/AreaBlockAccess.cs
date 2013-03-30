using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo.Cuboid;

using Mogre;
using Material = API.Generic.Material;

namespace API.Geo
{
    public interface AreaBlockAccess {

        /**
	     * Gets if a block is contained in this area
	     *
	     * @param x coordinate of the block
	     * @param y coordinate of the block
	     * @param z coordinate of the block
	     * @return true if it is contained, false if not
	     */
	    bool hasBlock(int x, int y, int z);

	    /**
	     * Gets a {@link Block} representing the block at (x, y, z)
	     *
	     * @param x coordinate of the block
	     * @param y coordinate of the block
	     * @param z coordinate of the block
	     * @return the Block
	     */
	    Block getBlock(int x, int y, int z, bool force);

	    /**
	     * Gets a {@link Block} representing the block at the position given
	     *
	     * @param position of the block
	     * @return the Block
	     */
	    Block getBlock(Vector3 position, bool force);

    }
}