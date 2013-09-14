using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using API.Generic;
using API.Geo.Cuboid;

using Mogre;

using Game.World.Blocks;

namespace Game.World.Generator
{
    class VanillaChunk : Chunk
    {
        public static Dictionary<string, Block>   staticBlock;
        public static Dictionary<byte,   string>  byteToString;
        public static Dictionary<string, string>  materialToString;
        public static Dictionary<string, Block>   textureToBlock;


        static VanillaChunk() {
            staticBlock      = new Dictionary<string, Block> ();
            byteToString     = new Dictionary<byte,   string>();
            materialToString = new Dictionary<string, string>();
            textureToBlock   = new Dictionary<string, Block> ();
            Clean = true;


            byteToString.Add(0, "Air");
            Type[] blockList = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "Game.World.Blocks");
            for (int i = 0; i < blockList.Length; i++) {

                if(typeof(Block).IsAssignableFrom(blockList[i]) && !blockList[i].IsAbstract) {
                    
                    ConstructorInfo ctor = blockList[i].GetConstructor(new Type[0]);
                    Block test = (Block) ctor.Invoke(new object[0]);

                    if(!staticBlock.ContainsKey(test.getName())) {
                        staticBlock.Add(test.getName(), test);

                        if (test.getItemTexture() != null)
                            textureToBlock.Add(test.getItemTexture(), test);
                        
                        if(!byteToString.ContainsKey(test.getId())) { byteToString.Add(test.getId() , test.getName()); }
                        else if(test.getId() != 255) { throw new Exception(test.getName() + " has the same Id as " +  byteToString[test.getId()]); }
                        
                        if(test.getMaterial() != null && !materialToString.ContainsKey(test.getMaterial())) {
                                materialToString.Add(test.getMaterial(), test.getName());
                        }
                        
                    } else {
                        throw new Exception("Block : " + blockList[i].Name + " has a name (" + test.getName() + ") which already exists"); 
                    }
                }
            }
        }

        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace) {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }

        public VanillaChunk(Vector3 chunkSize, Vector3 location, Island island) : base(chunkSize, location, island) {
            this.mBlockList = new Block[16, 16, 16];
            for(int x = 0; x < mChunkSize.x; x++) {
                for(int y = 0; y < mChunkSize.y; y++) {
                    for(int z = 0; z < mChunkSize.z; z++) {
                        this.mBlockList[x, y, z] = staticBlock["Air"];
                    }
                }
            }

        }

        public override void generateVisualChunk() {
            this.multiList.Clear();

            for (int x = 0; x < mChunkSize.x; x++) {
                for (int y = 0; y < mChunkSize.y; y++) {
                    for (int z = 0; z < mChunkSize.z; z++) {
                        if (this.mBlockList[x, y, z].getMaterial() == null || !this.mIsland.setVisibleFaces(new Vector3(x, y, z) + this.mChunkLocation * Cst.CHUNK_SIDE, this.mBlockList[x, y, z])) {
                            continue;
                        }
                        for (int i = 0; i < 6; i++) {
                            if(!this.hasVisibleFaceAt(x, y, z, (BlockFace)i)) {
                                continue;
                            }

                            if (!this.multiList.ContainsKey(this.mBlockList[x, y, z].getFace(i))) {
                                this.addBlockType(this.mBlockList[x, y, z].getMeshType(), this.mBlockList[x, y, z].getFace(i));
                            }

                            if (this.hasVisibleFaceAt(x, y, z, (BlockFace)i)) {
                                this.multiList[this.mBlockList[x, y, z].getFace(i)].addBlock(this.mChunkLocation * Cst.CHUNK_SIDE + new Vector3(x, y, z), (BlockFace)i);
                            }

                        }
                    }
                }
            }

            foreach (KeyValuePair<string, MultiBlock> pair in this.multiList) {
                pair.Value.display(this.mIsland, this.mIsland.mWorld);
            }

        }

        public override void setBlock(int x, int y, int z, string material) {
            if (this.mIsland.displayed) {
                this.mIsland.dirtyChunks.Add(this);
                this.updateBlock((int)this.mChunkLocation.x * 16 + x, (int)this.mChunkLocation.y * 16 + y, (int)this.mChunkLocation.z * 16 + z, this.mBlockList[x, y, z], this);
                this.mBlockList[x, y, z] = staticBlock[material];
                this.mIsland.setVisibleFaces(new Vector3((int)this.mChunkLocation.x * 16 + x, (int)this.mChunkLocation.y * 16 + y, (int)this.mChunkLocation.z * 16 + z), this.mBlockList[x, y, z]);

                if (material != "Air") {
                    this.updateBlock((int)this.mChunkLocation.x * 16 + x, (int)this.mChunkLocation.y * 16 + y, (int)this.mChunkLocation.z * 16 + z, this.mBlockList[x, y, z], this);
                }

                List<Vector3> coordToCheck = new List<Vector3>();

                coordToCheck.Add(new Vector3(x + 1, y,     z));     coordToCheck.Add(new Vector3(x - 1, y,     z));
                coordToCheck.Add(new Vector3(x,     y + 1, z));     coordToCheck.Add(new Vector3(x,     y - 1, z));
                coordToCheck.Add(new Vector3(x,     y,     z + 1)); coordToCheck.Add(new Vector3(x,     y,     z - 1));

                List<Chunk> cs = new List<Chunk>();

                foreach (Vector3 pos in coordToCheck) {

                    int absX = (int)this.mChunkLocation.x * 16 + (int)pos.x;
                    int absY = (int)this.mChunkLocation.y * 16 + (int)pos.y;
                    int absZ = (int)this.mChunkLocation.z * 16 + (int)pos.z;

                    Block b = this.mIsland.getBlock(absX, absY, absZ, true);
                    Chunk c = this.mIsland.getChunkFromBlock(new Vector3(absX, absY, absZ));

                    if (this.mIsland.setVisibleFaces(new Vector3(absX, absY, absZ), b)) {
                        if (updateBlock(absX, absY, absZ, b, c) && c != this) { cs.Add(c); }
                    }
                }

                this.clean();
                foreach (Chunk c in cs) {
                    c.clean();
                }
            }
            else {
                this.mBlockList[x, y, z] = staticBlock[material];
            }
        }

        /*
         * @return wether or not the block has anyVisible faces
         * @return false if block b is null or b is Air
         */
        private bool updateBlock(int absX, int absY, int absZ, Block b, Chunk c) {
            if (b == null || b.getName() == "Air") {
                return false;
            }
            
            bool used = false;
            MultiBlock val;
            for (int i = 0; i < 6; i++) {
                if (!this.mIsland.hasVisibleFaceAt(absX, absY, absZ, (BlockFace)i)) {
                    continue;
                }
                used = true;
                if (c.multiList.TryGetValue(b.getFace(i), out val)) {
                    c.dirtyMultiListInsert(b.getFace(i), c.multiList[b.getFace(i)]);
                    c.multiList.Remove(b.getFace(i));
                }
                else if (!c.dirtyMultiListName.Contains(b.getFace(i))) {
                    c.addBlockType(b.getMeshType(), b.getFace(i));
                    c.dirtyMultiListInsert(b.getFace(i), c.multiList[b.getFace(i)]);
                    c.multiList.Remove(b.getFace(i));
                }
            }
            return used;
        }

        public override void setBlock(int x, int y, int z, byte material) { this.setBlock(x, y, z, byteToString[material]); }
        public override string getBlockMaterial(int x, int y, int z)      { return this.mBlockList[x, y, z].getMaterial();  }

    }
}
