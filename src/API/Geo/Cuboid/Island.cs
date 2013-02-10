using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo;
using API.Generator;
using API.Generic;

using Entity   = API.Ent.Entity;
using Material = API.Generic.Material;

using Mogre;

namespace API.Geo.Cuboid
{
    /**
     * Represents an Island
     */
    public abstract class Island : AreaChunkAccess {

        protected Vector3                        mIslandSize;
        protected World                          mWorld;
        protected SceneNode                      mNode;
        protected bool                           mIsTerrainUpdated;
        protected Biome                          mBiome;
        public Dictionary<string, MultiBlock>    multiList = new Dictionary<string, MultiBlock>();

        protected Dictionary<Vector3, Chunk> mChunkList;

        public SceneNode Node        { get { return this.mNode; } }
        public bool IsTerrainUpdated { get { return this.mIsTerrainUpdated; } }

	    public Island(SceneNode node, Vector2 size, Game.World.MainWorld currentWorld) {
            this.mChunkList = new Dictionary<Vector3,Chunk>();
            this.initChunks(size);

            this.mNode = node;
            this.mIslandSize  = new Vector3(size.x, 0, size.y);
            this.mWorld = currentWorld;
        }

        //Init
        public abstract void initChunks(Vector2 size);
        //get
	    public Vector3 getSize()       { return this.mIslandSize;  }
        public Vector3 getPosition()   { return this.mNode.Position; }
        public Vector3 getSpawnPoint() { throw new NotImplementedException(); }

        public Chunk getChunk  (int x, int y, int z)                  { return this.mChunkList[new Vector3(x,y,z)]; }
	    public Chunk getChunkAt(Vector3 relativePosition)             { return this.mChunkList[relativePosition];   }

        public abstract Chunk getChunkFromBlock(int x, int y, int z);
        public Chunk getChunkFromBlock(Vector3 loc)              { return this.getChunkFromBlock((int) loc.x, (int) loc.y, (int) loc.z); }

        public abstract int getSurfaceHeight(int x, int z);

        public Block getBlock(Vector3 loc, bool force) { return this.getBlock((int) loc.x, (int) loc.y, (int) loc.z, force); }
        public abstract Block getBlock(int x, int y, int z, bool force);
        
        public Vector3 getBlockCoord(Vector3 loc) { return this.getBlockCoord((int) loc.x, (int) loc.y, (int) loc.z); }
        public abstract Vector3 getBlockCoord(int x, int y, int z);
        public abstract void getBlockCoord(Vector3 pos, out Vector3 blockPos, out Vector3 chunkPos);

        public virtual List<Character> getPlayers() { throw new NotImplementedException(); }

        public abstract bool hasVisiblefaceAt(int x, int y, int z, BlockFace face);
        public abstract void setVisibleFaceAt(int x, int y, int z, BlockFace face, bool val);

        //set
        public abstract void setBlockAt(int x, int y, int z, string material, bool force);
        public abstract void setBlockAt(int x, int y, int z, byte material, bool force);
        public abstract string getMaterialFromName(string name);


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
        public abstract void display();

        public abstract void removeFromScene(Vector3 item);

        public abstract void RechargeMulti(MultiBlock multi);


	    /**
	     * Save the Island
         * Warning this function does not create a thread
	     */
	    public virtual void save() { throw new NotImplementedException(); }

	    /**
	     * Performs the nessecary tasks to unload this region from the world, and
	     * all associated chunks.
	     * @param save whether to save the region and associated data.
	     */
	    public virtual void unload(bool save) { throw new NotImplementedException(); }

        public void unloadChunk(int x, int y, int z, bool save) { throw new NotImplementedException(); }
        public void unloadChunk(Vector3 position, bool save)    { this.unloadChunk((int)position.x, (int)position.y, (int)position.z, save); }

    }
}