using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo;
using API.Generator;

using Entity   = API.Ent.Entity;
using Material = API.Generic.Material;

using Mogre;

namespace API.Geo.Cuboid
{
    /**
     * Represents an Island
     */
    public abstract class Island : AreaChunkAccess {

        protected Vector3 mIslandSize;
        protected Vector3 mIslandCoord;
        protected World   mWorld;
        
        protected Dictionary<Vector3, Chunk> mChunkList;
        

	    public Island(Vector3 islandCoord, Vector2 size, World currentWorld) {
            this.mChunkList = new Dictionary<Vector3,Chunk>();
            this.initChunks(size);

            this.mIslandCoord = islandCoord;
            this.mIslandSize  = new Vector3(size.x, 0, size.y);
            this.mWorld = currentWorld;
        }

        //Init
        public abstract void initChunks(Vector2 size);
        //get
	    public Vector3 getSize()       { return this.mIslandSize;  }
        public Vector3 getPosition()   { return this.mIslandCoord; }
        public Vector3 getSpawnPoint() { throw new NotImplementedException(); }

        public Chunk getChunk  (int x, int y, int z)                  { return this.mChunkList[new Vector3(x,y,z)]; }
	    public Chunk getChunkAt(Vector3 relativePosition)             { return this.mChunkList[relativePosition];   }

        public abstract Chunk getChunkFromBlock(int x, int y, int z);
        public Chunk getChunkFromBlock(Vector3 loc)              { return this.getChunkFromBlock((int) loc.x, (int) loc.y, (int) loc.z); }

        public abstract int getSurfaceHeight(int x, int z);

        public Block getBlock(Vector3 loc)                            { return this.getBlock((int) loc.x, (int) loc.y, (int) loc.z); }
        public abstract Block getBlock(int x, int y, int z);
        
        public abstract Material getBlockMaterial(int x, int y, int z);

        public virtual List<Character> getPlayers() { throw new NotImplementedException(); }


        //set
        public virtual void     setBlockMaterial(int x, int y, int z, Material material) { throw new NotImplementedException(); }


        //Has

        //Coord are absolute postion
	    public abstract bool hasBlock(int x, int y, int z);
        public bool hasBlock(Vector3 position)    { return this.hasBlock((int)position.x, (int)position.y, (int)position.z); }
        
        public bool hasChunk(int x, int y, int z) { return this.mChunkList.ContainsKey(new Vector3(x, y, z)); }
        public bool hasChunk(Vector3 position)    { return this.mChunkList.ContainsKey(position);             }

        /**
	     * Generate the Island's terrain
	     */
        public abstract void generate(int seed);

        /**
	     * Displays the Island's terrain
	     */
        public abstract void display(SceneManager sceneMgr);

	    /**
	     * Queues all chunks for saving at the next available opportunity.
	     */
	    public virtual void save() { throw new NotImplementedException(); }

	    /**
	     * Performs the nessecary tasks to unload this region from the world, and
	     * all associated chunks.
	     * @param save whether to save the region and associated data.
	     */
	    public virtual void unload(bool save) { throw new NotImplementedException(); }

        public void saveChunk(int x, int y, int z) { throw new NotImplementedException(); }
        public void saveChunk(Vector3 position)    { this.saveChunk((int)position.x, (int)position.y, (int)position.z); }

        public void unloadChunk(int x, int y, int z, bool save) { throw new NotImplementedException(); }
        public void unloadChunk(Vector3 position, bool save)    { this.unloadChunk((int)position.x, (int)position.y, (int)position.z, save); }

    }
}