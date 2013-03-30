using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo.Cuboid;

using Mogre;

namespace API.Geo
{
    public interface AreaIslandAccess : AreaChunkAccess {

	    /**
	     * Gets an unmodifiable collection of all loaded regions
	     * 
	     * @return all loaded regions
	     */
	    List<Island> getIslands();

	    /**
	     * Gets the {@link Island} at region coordinates (x, y, z)
	     *
	     * @param x the region x coordinate
	     * @param y the region y coordinate
	     * @param z the region z coordinate
	     * @return the region
	     */
	    Island getIsland(int x, int y, int z);

	    /**
	     * Gets the {@link Island} at chunk coordinates (x, y, z)
	     *
	     * @param x the chunk x coordinate
	     * @param y the chunk y coordinate
	     * @param z the chunk z coordinate
	     * @return the region
	     */
	    Island getIslandFromChunk(int x, int y, int z);


	    /**
	     * Gets the {@link Island} at block coordinates (x, y, z)
	     *
	     * @param x the block x coordinate
	     * @param y the block y coordinate
	     * @param z the block z coordinate
	     * @return the region
	     */
	    Island getIslandFromBlock(int x, int y, int z);


	    /**
	     * Gets the {@link Island} at block coordinates (x, y, z)
	     *
	     * @param position of the block
	     * @return the region
	     */
	    Island getIslandFromBlock(Vector3 position);
    }
}