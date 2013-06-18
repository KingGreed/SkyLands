using System;
using System.Collections.Generic;
using System.Linq;
using API.Ent;
using API.Generator;
using API.Generic;

using Game.States;

using Entity   = API.Ent.Entity;


using Mogre;

namespace API.Geo.Cuboid
{
    /**
     * Represents an Island
     */
    public abstract class Island : AreaChunkAccess {

        protected Vector3                        mIslandSize;
        public World                             mWorld;
        protected SceneNode                      mNode;
        protected SceneNode                      mFaceNode;
        protected bool                           mIsTerrainUpdated;
        protected Biome                          mBiome;
        public Dictionary<string, MultiBlock>    multiList = new Dictionary<string, MultiBlock>();
        public List<PositionFaceAndStatus>       blocksAdded;
        public List<PositionFaceAndStatus>       blocksDeleted;


        public Dictionary<Vector3, Chunk> mChunkList;

        public SceneNode Node        { get { return this.mNode; } }
        public bool IsTerrainUpdated { get { return this.mIsTerrainUpdated; } }

        public Island(SceneNode node, Vector2 size, API.Geo.World currentWorld) {
            this.mChunkList = new Dictionary<Vector3,Chunk>();
            this.initChunks(size);

            this.mNode = node;
            this.mIslandSize  = new Vector3(size.x, 0, size.y);
            this.mWorld = currentWorld;

            this.blocksAdded    = new List<PositionFaceAndStatus>();
            this.blocksDeleted = new List<PositionFaceAndStatus> ();

            this.mFaceNode = node.CreateChildSceneNode();
        }
        public Island(SceneNode node, API.Geo.World currentWorld) {
            this.mChunkList = new Dictionary<Vector3, Chunk>();

            this.mNode = node;
            this.mWorld = currentWorld;

            this.blocksAdded = new List<PositionFaceAndStatus>();
            this.blocksDeleted = new List<PositionFaceAndStatus>();

            this.mFaceNode = node.CreateChildSceneNode();
        }

        //Init
        public abstract void initChunks(Vector2 size);
        //get
	    public Vector3 getSize()       { return this.mIslandSize;  }
        public Vector3 getPosition()   { return this.mNode.Position; }
        public Vector3 getSpawnPoint() { throw new NotImplementedException(); }
        public SceneNode getFaceNode() { return this.mFaceNode; }
        public Biome getBiome()        { return this.mBiome; }

        public Chunk getChunk  (int x, int y, int z)                  { return this.mChunkList[new Vector3(x,y,z)]; }
	    public Chunk getChunkAt(Vector3 relativePosition)             { return this.mChunkList[relativePosition];   }

        public abstract Chunk getChunkFromBlock(int x, int y, int z);
        public Chunk getChunkFromBlock(Vector3 loc)              { return this.getChunkFromBlock((int) loc.x, (int) loc.y, (int) loc.z); }

        public abstract int getSurfaceHeight(int x, int z, string restriction = "");

        public Block getBlock(Vector3 loc, bool force) { return this.getBlock((int) loc.x, (int) loc.y, (int) loc.z, force); }
        public abstract Block getBlock(int x, int y, int z, bool force);    // +1 for z field added in VanillaIsland
        
        public Vector3 getBlockCoord(Vector3 loc) { return this.getBlockCoord((int) loc.x, (int) loc.y, (int) loc.z); }
        public abstract Vector3 getBlockCoord(int x, int y, int z);

        public virtual List<Character> getPlayers() { throw new NotImplementedException(); }

        public abstract bool hasVisiblefaceAt(int x, int y, int z, BlockFace face);
        public abstract void setVisibleFaceAt(int x, int y, int z, BlockFace face, bool val);

        //set
        public abstract void setBlockAt(int x, int y, int z, string material, bool force);
        public abstract void setBlockAt(int x, int y, int z, byte material, bool force);
        public abstract void setBlockAt(Vector3 loc, string name, bool force);
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
        public abstract void refreshBlock   (Vector3 relativePos);
        public abstract void addBlockToScene(Vector3 relativePos, string material);
        public abstract void addFaceToScene(BlockFace face, Vector3 relativePos, string material);



	    /**
	     * Save the Island
         * Warning this function does not create a thread
	     */
	    public virtual void save() { throw new NotImplementedException(); }
        public abstract void save(string name);

        public abstract void load();
        public abstract void load(string name);

	    /**
	     * Performs the nessecary tasks to unload this region from the world, and
	     * all associated chunks.
	     * @param save whether to save the region and associated data.
	     */
	    public virtual void unload(bool save) {
            if(save) { this.save(); }

            this.mChunkList = null;
            this.multiList = null;

            this.mNode.RemoveAndDestroyAllChildren();
            this.mWorld.getSceneMgr().DestroySceneNode(this.mNode);

            this.mWorld.unloadIsland();
        }

        public void unloadChunk(int x, int y, int z, bool save) { throw new NotImplementedException(); }
        public void unloadChunk(Vector3 position, bool save)    { this.unloadChunk((int)position.x, (int)position.y, (int)position.z, save); }


        /*
         * Returns position of item -1 if item does not exist
         */
        public bool isinBlocksAdded(Vector3 item)   { return this.blocksAdded.Any  (t => t.position == item); }
        public bool isInBlocksDeleted(Vector3 item) { return this.blocksDeleted.Any(t => t.position == item); }
        public bool isInBlocksAdded(Vector3 item, BlockFace face) { return this.blocksAdded.Any(t => t.position == item && face == t.face); }

        public void removeFromBlocksAdded(int pos)   { this.blocksAdded.RemoveAt  (pos); }
        public void removeFromBlocksDeleted(int pos) { this.blocksDeleted.RemoveAt(pos); }
        public int  getNumOfBlocksAddedAndRemoved()  { return this.blocksAdded.Count + this.blocksDeleted.Count; }
        public bool wasBlockAddedDeleted(Vector3 item, BlockFace face) {
            for (int i = 0; i < this.blocksAdded.Count; i++) {
                if (this.blocksAdded[i].position == item && face == this.blocksAdded[i].face) { return !this.blocksAdded[i].status; }
            }
            return true;
        }

        public int getItemPosInBlocksAdded(Vector3 item, BlockFace face) {
            for (int i = 0; i < this.blocksAdded.Count; i++) {
                if (this.blocksAdded[i].position == item && face == this.blocksAdded[i].face) { return i; }
            }
            return -1;
        }

        public void populate() {
            LogManager.Singleton.DefaultLog.LogMessage("Perlin set");
            if(this.mBiome != null) {
                this.mBiome.decorate(this, new Random());
            }
            LogManager.Singleton.DefaultLog.LogMessage("Island decorated");
        }
    }
}