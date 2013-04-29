using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo;
using API.Generator;
using API.Generic;

using Mogre;
using Entity   = API.Ent.Entity;
using Material = API.Generic.Material;

namespace API.Geo.Cuboid
{
    /**
     * Represents a cube containing 16x16x16 Blocks
     */
    public abstract class Chunk : AreaBlockAccess {

	    protected Vector3     mChunkSize;
        protected Vector3     mChunkLocation;
        public Block[, ,]  mBlockList;
        protected Island      mIsland;
        protected bool[, , ,] mVisible;

	    public Chunk(Vector3 chunkSize, Vector3 location, Island island) {
            this.mChunkSize = chunkSize;
            this.mChunkLocation = location;
            this.mVisible = new bool[16, 16, 16, 6];
        }

        public void setVisibleFaceAt(int x, int y, int z, BlockFace face, bool val) { this.mVisible[x, y, z, (int) face] = val;  }
        public bool hasVisibleFaceAt(int x, int y, int z, BlockFace face)           { return this.mVisible[x, y, z, (int) face]; }

	    /**
	     * Gets the region that this chunk is located in
	     *
	     * @return region
	     */
	    public virtual Island getIsland() { return this.mIsland; }

    
	    /**
	     * Gets the entities currently in the chunk
	     *
	     * @return the entities
	     */
	    public virtual List<Entity> getLiveEntities() { throw new NotImplementedException(); }


        public Block getBlock(int x, int y, int z, bool force = false) { return this.mBlockList[x, y, z]; }
        public Block getBlock(Vector3 loc, bool force = false)         { return this.getBlock((int)loc.x, (int)loc.y, (int)loc.z, force); }

        public abstract string getBlockMaterial(int x, int y, int z);

        //set
        public void setBlock(Vector3 loc, string material) { this.setBlock((int)loc.x, (int)loc.y, (int)loc.z, material); }
        public abstract void setBlock(int x, int y, int z, string material);
        
        public void setBlock(Vector3 loc, byte material) { this.setBlock((int)loc.x, (int)loc.y, (int)loc.z, material); }
        public abstract void setBlock(int x, int y, int z, byte material);

        public int getNumOfVisibleFaces(int x, int y, int z) { 
            int num = 0;
            for(int i = 0; i < 6; i++) { if(this.mVisible[x, y, z, i]) { num++; } }

            return num;
        }

        
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