using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo.Cuboid;

using Mogre;

namespace API.Geo
{

    public interface AreaChunkAccess : AreaBlockAccess {

	    /**
	     * Gets the {@link Chunk} at chunk coordinates (x, y, z)
	     *
	     * @param x coordinate of the chunk
	     * @param y coordinate of the chunk
	     * @param z coordinate of the chunk
	     * @return the chunk
	     */
	    Chunk getChunk(int x, int y, int z);


	    /**
	     * Gets the {@link Chunk} at block coordinates (x, y, z)
	     *
	     * @param x coordinate of the block
	     * @param y coordinate of the block
	     * @param z coordinate of the block
	     * @return the chunk
	     */
	    Chunk getChunkFromBlock(int x, int y, int z);


	    /**
	     * Gets the {@link Chunk} at the given position
	     *
	     * @param position of the block
	     * @return the chunk
	     */
	    Chunk getChunkFromBlock(Vector3 position);


	    /**
	     * True if the region has a loaded chunk at the (x, y, z).
	     *
	     * @param x coordinate of the chunk
	     * @param y coordinate of the chunk
	     * @param z coordinate of the chunk
	     * @return true if chunk exists
	     */
	    bool hasChunk(int x, int y, int z);

	    /**
	     * Unloads a chunk, and queues it for saving, if requested.
	     *
	     * @param x coordinate of the chunk
	     * @param y coordinate of the chunk
	     * @param z coordinate of the chunk
	     * @Param whether to save this chunk
	     */
	    void unloadChunk(int x, int y, int z, bool save);
	
    }
}