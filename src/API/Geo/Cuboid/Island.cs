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

        protected Vector2 mIslandSize;
        protected Vector3 mIslandCoord;
        
        protected Dictionary<Vector3, Chunk> mChunkList;
        

	    public Island(Vector3 islandCoord, Vector2 size) {
            this.mChunkList = new Dictionary<Vector3,Chunk>();
            this.initChunks(size);

            this.mIslandCoord = islandCoord;
            this.mIslandSize  = size;
        }

        //Init
        public void initChunks(Vector2 size) {
            for(int x = 0; x < size.x; x++) {
                for(int z = 0; z < size.y; z++) {
                    this.mChunkList.Add(new Vector3(x, 0, z), new Chunk(new Vector3(16,16,16), this));
                }
            }
        }

        //get
	    public Vector2 getSize()       { return this.mIslandSize;  }
        public Vector3 getPosition()   { return this.mIslandCoord; }
        public Vector3 getSpawnPoint() { throw new NotImplementedException(); }

        public Chunk getChunk  (int x, int y, int z)                  { return this.mChunkList[new Vector3(x,y,z)]; }
	    public Chunk getChunkAt(Vector3 relativePosition)             { return this.mChunkList[relativePosition];   }

        public Chunk getChunkFromBlock(int x, int y, int z)           { throw new NotImplementedException(); }
        public Chunk getChunkFromBlock(Vector3 position)              { throw new NotImplementedException(); }

        public Block getBlock(Vector3 loc)                            { return this.getBlock((int) loc.x, (int) loc.y, (int) loc.z); }
        public Block getBlock(int x, int y, int z) {

            Vector3 chunkLocation = new Vector3(0, 0, 0),
                    blockLocation = new Vector3(0, 0, 0);
            
            chunkLocation.x = x % 16;
            chunkLocation.y = y % 16;
            chunkLocation.z = z % 16;

            blockLocation.x = x / 16;
            blockLocation.y = y / 16;
            blockLocation.z = z / 16;

            return this.mChunkList[chunkLocation].getBlock(blockLocation);
        }
        
        public virtual Material getBlockMaterial(int x, int y, int z) { throw new NotImplementedException(); }

        public abstract List<Character> getPlayers();


        //set
        public virtual void     setBlockMaterial(int x, int y, int z, Material material) { throw new NotImplementedException(); }


        //Has

        //Coord are absolute postion
	    public bool hasBlock(int x, int y, int z) { throw new NotImplementedException(); }
        public bool hasBlock(Vector3 position)    { return this.hasBlock((int)position.x, (int)position.y, (int)position.z); }
        
        public bool hasChunk(int x, int y, int z) { return this.mChunkList.ContainsKey(new Vector3(x, y, z)); }
        public bool hasChunk(Vector3 position)    { return this.mChunkList.ContainsKey(position);             }


        public virtual void generate();
	    /**
	     * Queues all chunks for saving at the next available opportunity.
	     */
	    public abstract void save();

	    /**
	     * Performs the nessecary tasks to unload this region from the world, and
	     * all associated chunks.
	     * @param save whether to save the region and associated data.
	     */
	    public abstract void unload(bool save);

        public void saveChunk(int x, int y, int z) { throw new NotImplementedException(); }
        public void saveChunk(Vector3 position)    { this.saveChunk((int)position.x, (int)position.y, (int)position.z); }

        public void unloadChunk(int x, int y, int z, bool save) { throw new NotImplementedException(); }
        public void unloadChunk(Vector3 position, bool save)    { this.unloadChunk((int)position.x, (int)position.y, (int)position.z, save); }

    }
}