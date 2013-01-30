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
    public abstract class Chunk : AreaBlockAccess {

	    protected Vector3    mChunkSize;
        protected Vector3    mChunkLocation;
        protected Block[, ,] mBlockList;
        protected Island     mIsland;

        private Biome mBiomeType;

	    public Chunk(Vector3 chunkSize, Vector3 location, Island island) {
            this.mChunkSize = chunkSize;
            this.mChunkLocation = location;
        }

	    /**
	     * Gets the region that this chunk is located in
	     *
	     * @return region
	     */
	    public virtual Island getIsland() { return this.mIsland; }

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


        public Block getBlock(int x, int y, int z) 
        {
            if (x >= 0 && y >= 0 && z >= 0 && x < this.mBlockList.GetLength(0) && y < this.mBlockList.GetLength(1) && z < this.mBlockList.GetLength(2))
                return this.mBlockList[x, y, z];
            else
                return null;
        }
        public Block getBlock(Vector3 loc)         { return this.getBlock((int)loc.x, (int)loc.y, (int)loc.z); }

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