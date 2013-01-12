using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo;
using API.Generator;

using Mogre;
using Entity   = API.Ent.Entity;
using Material = API.Generic.Material;

namespace API.Geo.Cuboid
{
    /**
     * Represents a cube containing 16x16x16 Blocks
     */
    public class Chunk : AreaBlockAccess {

	    private Vector3   mChunkSize;
        public Block[, ,] mBlockList;
        private Island    mIsland;

        private Biome mBiomeType;

	    public Chunk(Vector3 chunkSize, Island island) {
            this.mChunkSize = chunkSize; 
        }

	    /**
	     * Gets the region that this chunk is located in
	     *
	     * @return region
	     */
	    public virtual Island getIsland() { throw new NotImplementedException(); }

        /**
	     * Gets the biome that this chunk is located in
	     *
	     * @return region
	     */
	    public virtual Biome getBiome() { throw new NotImplementedException(); }
        
	    /**
	     * Gets the entities currently in the chunk
	     *
	     * @return the entities
	     */
	    public virtual List<Entity> getLiveEntities() { throw new NotImplementedException(); }


        public Block getBlock(int x, int y, int z)                    { return this.mBlockList[x, y, z];                            }
        public Block getBlock(Vector3 loc)                            { return this.mBlockList[(int)loc.x, (int)loc.y, (int)loc.z]; }

        public virtual Material getBlockMaterial(int x, int y, int z) { throw new NotImplementedException(); }

        //set
        public virtual void setBlockMaterial(int x, int y, int z, Material material) { throw new NotImplementedException(); }
        public virtual void setBiome(Biome type)                                     { this.mBiomeType = type; }
        
        
        /**
	     * Tests if the chunk is currently loaded
	     *
	     * Chunks may be unloaded at the end of each tick
	     */
	    public virtual bool isLoaded() { throw new NotImplementedException(); }

	    /**
	     * Gets if this chunk already has been populated.
	     *
	     * @return if the chunk is populated.
	     */
	    public virtual bool isPopulated() { throw new NotImplementedException(); }

	    public bool hasBlock(int x, int y, int z) { throw new NotImplementedException(); }
        
        /**
	     * Populates the chunk with all the Populators attached to the
	     * WorldGenerator of its world.
	     *
	     * @return
	     */
	    public virtual bool populate() { throw new NotImplementedException(); }

        /**
	     * Performs the necessary tasks to unload this chunk from the world.
	     *
	     * @param save whether the chunk data should be saved.
	     */
	    public virtual void unload(bool save) { throw new NotImplementedException(); }

	    /**
	     * Performs the necessary tasks to save this chunk.
	     */
	    public virtual void save() { throw new NotImplementedException(); }
    }
}