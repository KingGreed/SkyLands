using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo;
using API.Geo.Cuboid;
using API.Generator;
using API.Generic;

using Mogre;
using Entity   = API.Ent.Entity;
using Material = API.Generic.Material;

using Game.World.Generator;

namespace API.Geo.Cuboid
{
    /**
     * Represents a cube containing 16x16x16 Blocks
     */
    public abstract class Chunk : AreaBlockAccess {

	    protected Vector3     mChunkSize;
        protected Vector3     mChunkLocation;
        public Block[, ,]     mBlockList;
        protected Island      mIsland;
        protected bool[, , ,] mVisible;
        public static bool    Clean;

        public Dictionary<string, MultiBlock> multiList = new Dictionary<string, MultiBlock>();
        protected HashSet<MultiBlock> dirtyMultiList   = new HashSet<MultiBlock>();
        public HashSet<String>  dirtyMultiListName  = new HashSet<string>();

	    public Chunk(Vector3 chunkSize, Vector3 location, Island island) {
            this.mChunkSize = chunkSize;
            this.mChunkLocation = location;
            this.mVisible = new bool[16, 16, 16, 6];
            this.mIsland = island;
        }

        public void setVisibleFaceAt(int x, int y, int z, BlockFace face, bool val) { this.mVisible[x, y, z, (int) face] = val;  }
        public bool hasVisibleFaceAt(int x, int y, int z, BlockFace face)           { return this.mVisible[x, y, z, (int) face]; }

        public void dirtyMultiListInsert(String name, MultiBlock multi) {
            this.dirtyMultiListName.Add(name);
            this.dirtyMultiList.Add(multi);
        }

        public void clearDirtyMultiList() {
            this.dirtyMultiList.Clear();
            this.dirtyMultiListName.Clear();
        }

        public void clean() {
            if (Clean) {
                HashSet<string> keys = new HashSet<string>();

                foreach (MultiBlock item in dirtyMultiList) {
                    string face = item.getMaterial();
                    int meshType = item.meshType;
                    item.Remove();
                    this.addBlockType(meshType, face);
                    keys.Add(face);
                }
                this.clearDirtyMultiList();
                
                for (int x = 0; x < mChunkSize.x; x++) {
                    for (int y = 0; y < mChunkSize.y; y++) {
                        for (int z = 0; z < mChunkSize.z; z++) {
                            if (this.mBlockList[x, y, z].getMaterial() == null || !this.mIsland.setVisibleFaces(new Vector3(x, y, z) + this.mChunkLocation * Cst.CHUNK_SIDE, this.mBlockList[x, y, z])) {
                                continue;
                            }
                            for (int i = 0; i < 6; i++) {
                                if (!this.hasVisibleFaceAt(x, y, z, (BlockFace)i)) {
                                    continue;
                                }
                                if (keys.Contains(this.mBlockList[x, y, z].getFace(i))) {
                                    this.multiList[this.mBlockList[x, y, z].getFace(i)].addBlock(this.mChunkLocation * Cst.CHUNK_SIDE + new Vector3(x, y, z), (BlockFace)i);
                                }
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, MultiBlock> pair in this.multiList) {
                    if(keys.Contains(pair.Key)) {
                        pair.Value.display(this.mIsland, this.mIsland.mWorld);
                    }
                }

            }
        }

        public void addBlockType(int meshType, string face) {
            if(this.multiList.ContainsKey(face)) { return; }
            switch (meshType) {
                case 0: this.multiList.Add(face, new VanillaMultiBlock                (face, this.mIsland, this.mIsland.mWorld, meshType)); break;
                case 2: this.multiList.Add(face, new VanillaMultiVerticalHalfBlock    (face, this.mIsland, this.mIsland.mWorld, meshType)); break;
                case 1: this.multiList.Add(face, new VanillaMultiHorizontalHalfBlock  (face, this.mIsland, this.mIsland.mWorld, meshType)); break;
                case 3: this.multiList.Add(face, new VanillaMultiHorizontalEighthBlock(face, this.mIsland, this.mIsland.mWorld, meshType)); break;
            }
        }

        /**
         * Gets the Island that this chunk is located in
         *
         * @return Island
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
        public void setBlock(Vector3 loc, byte material)   { this.setBlock((int)loc.x, (int)loc.y, (int)loc.z, material); }
        public abstract void setBlock(int x, int y, int z, string material);
        public abstract void setBlock(int x, int y, int z, byte   material);

        public void setDirty(int x, int y, int z) {
            if (this.mBlockList[x, y, z].getName() == "Air") { return; }
            for (int i = 0; i < 6; i++) {
                if(this.multiList.ContainsKey(this.mBlockList[x, y, z].getFace(i)) && this.hasVisibleFaceAt(x,y,z, (BlockFace)i)) {
                    this.dirtyMultiListInsert(this.mBlockList[x, y, z].getFace(i), this.multiList[this.mBlockList[x, y, z].getFace(i)]);
                    this.multiList.Remove(this.mBlockList[x, y, z].getFace(i));
                }
            }
        }

        public int getNumOfVisibleFaces(int x, int y, int z) { 
            int num = 0;
            for(int i = 0; i < 6; i++) { if(this.mVisible[x, y, z, i]) { num++; } }
            return num;
        }

        public abstract void generateVisualChunk();
        
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