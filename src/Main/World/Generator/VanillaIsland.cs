using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;
using API.Generator;
using API.Generic;

using Mogre;

using Game.Display;
using Game.World.Blocks;

using Material = API.Generic.Material;


namespace Game.World.Generator
{
    public abstract class VanillaIsland : Island
    {
        static Block defaultBlock = new AirBlock();

        public VanillaIsland(Vector3 islandCoord, Vector2 size, MainWorld currentWorld) : base(islandCoord, size, currentWorld) {}
       
        public override void initChunks(Vector2 size) {
            for(int x = 0; x < size.x; x++) {
                for(int z = 0; z < size.y; z++) {
                    this.mChunkList.Add(new Vector3(x, 0, z), new VanillaChunk(new Vector3(16,16,16), new Vector3(x, 0, z), this));
                }
            }
        }

        public override int getSurfaceHeight(int x, int z) {
            for(int y = (int)this.mIslandSize.y * MainWorld.CHUNK_SIDE; y != 0 ; y--) { 
                if(!(this.getBlock(x, y, z, false) is AirBlock)) {
                    return y + 1; 
                }
            }
            return -1;
        }

        public override bool hasBlock(int x, int y, int z)           { throw new NotImplementedException(); }
        public override API.Generic.Material getBlockMaterial(int x, int y, int z) { throw new NotImplementedException(); }


        public override void display(SceneManager sceneMgr) {

            Dictionary<string, MultiBlock> multiList = new Dictionary<string, MultiBlock>();
            Block curr;

            foreach (KeyValuePair<string, Block> pair in VanillaChunk.staticBlock) {
                if(!(pair.Value is AirBlock)) {
                    multiList.Add(pair.Key, new VanillaMultiBlock(pair.Key));
                }
            }

            for(int x = 0; x < this.mIslandSize.x * MainWorld.CHUNK_SIDE; x++) {
                for(int y = 0; y < this.mIslandSize.y * MainWorld.CHUNK_SIDE; y++) {
                    for(int z = 0; z < this.mIslandSize.z * MainWorld.CHUNK_SIDE; z++) {
                        curr = this.getBlock(x, y, z, false);
                        if(!(curr is AirBlock) && this.setVisibleFaces(new Vector3(x, y, z), curr)) {
                            string[] key = curr.getComposingFaces();
                            for(int i = 0; i < curr.getComposingFaces().Length; i++) {
                                multiList[key[i]].addBlock(new Vector3(x, y, z));
                            }
                        }
                    }
                }
            }
            foreach(KeyValuePair<string, MultiBlock> pair in multiList) {
                pair.Value.display(sceneMgr, this, this.mWorld);
            }
        }


        //For optimization purpose returns wether the block has visible faces
        public bool setVisibleFaces(Vector3 blockCoord, Block curr)
        {

            if (curr is AirBlock) { return false; }
            bool hasVisiblefaces = false;
            Dictionary<BlockFace, Vector3> coordToCheck = new Dictionary<BlockFace,Vector3>();

            coordToCheck.Add(BlockFace.rightFace, new Vector3(blockCoord.x + 1, blockCoord.y,     blockCoord.z));
            coordToCheck.Add(BlockFace.leftFace,  new Vector3(blockCoord.x - 1, blockCoord.y,     blockCoord.z));
            coordToCheck.Add(BlockFace.upperFace, new Vector3(blockCoord.x,     blockCoord.y + 1, blockCoord.z));
            coordToCheck.Add(BlockFace.underFace, new Vector3(blockCoord.x,     blockCoord.y - 1, blockCoord.z));
            coordToCheck.Add(BlockFace.frontFace, new Vector3(blockCoord.x,     blockCoord.y,     blockCoord.z + 1));
            coordToCheck.Add(BlockFace.backFace,  new Vector3(blockCoord.x,     blockCoord.y,     blockCoord.z - 1));


            foreach (KeyValuePair<BlockFace, Vector3> keyVal in coordToCheck)
            {
                if (this.getBlock(keyVal.Value, false) is AirBlock) { 
                    this.setVisibleFaceAt(blockCoord, keyVal.Key, true); hasVisiblefaces = true; 
                }
            }
            return hasVisiblefaces;
        }

        private void setVisibleFaceAt(Vector3 loc, BlockFace face, bool val) {
            this.setVisibleFaceAt((int)loc.x, (int)loc.y, (int)loc.z, face, val); 
        }

        public override void setVisibleFaceAt(int x, int y, int z, BlockFace face, bool val) {
            Chunk blockChunk = this.getChunkFromBlock(x, y, z);
            if(blockChunk != null) {
                blockChunk.setVisibleFaceAt(x % 16, y % 16, z % 16, face, val);
            }
        }

        public override bool hasVisiblefaceAt(int x, int y, int z, BlockFace face) {
            Chunk blockChunk = this.getChunkFromBlock(x, y, z);
            if(blockChunk != null) {
                return blockChunk.hasVisibleFaceAt(x % 16, y % 16, z % 16, face);
            }
            return true;
        }

        public override Block getBlock(int x, int y, int z, bool force) {
            if(x < 0 || y < 0 || z < 0) { return defaultBlock; }

            if(force && y > this.mIslandSize.y * MainWorld.CHUNK_SIDE) {
                this.mIslandSize.y = (int) System.Math.Ceiling((float)y / 16f);
            }

            Vector3 chunkLocation = getChunkCoordFromRelative(x, y, z), 
                    blockLocation = getBlockCoordFromRelative(x, y, z);


            if(this.hasChunk(chunkLocation)) { return this.mChunkList[chunkLocation].getBlock(blockLocation); }
            else if(force) {
                this.mChunkList.Add(chunkLocation, new VanillaChunk(new Vector3(16,16,16), chunkLocation, this));
                return this.mChunkList[chunkLocation].getBlock(blockLocation);
            } 
            else { return defaultBlock; }
        }

        public override Vector3 getBlockCoord(int x, int y, int z) {
            if(x < 0 || y < 0 || z < 0) { return -Vector3.UNIT_SCALE; }
            
            Vector3 chunkLocation = getChunkCoordFromRelative(x, y, z), 
                    blockLocation = getBlockCoordFromRelative(x, y, z);

            if(this.hasChunk(chunkLocation)) { return blockLocation; }
            else                             { return -Vector3.UNIT_SCALE; }
        }

        public override Chunk getChunkFromBlock(int x, int y, int z) {
            if(x < 0 || y < 0 || z < 0) { return null; }


            Vector3 chunkLocation = getChunkCoordFromRelative(x, y, z);

            if(this.hasChunk(chunkLocation)) { return this.mChunkList[chunkLocation]; }
            else                             { return null; }


        }

        private Vector3 getBlockCoordFromRelative(int x, int y, int z) { return new Vector3(x % 16, y % 16, z % 16); }
        private Vector3 getChunkCoordFromRelative(int x, int y, int z) { return new Vector3(x / 16, y / 16, z / 16); }

        public override void setBlockAt(int x, int y, int z, string material, bool force) {
            if(x < 0 || y < 0 || z < 0) { return; }

            if(force && y > this.mIslandSize.y * MainWorld.CHUNK_SIDE) {
                this.mIslandSize.y = (int) System.Math.Ceiling((float)y / 16f);
            }

            Vector3 chunkLocation = getChunkCoordFromRelative(x, y, z), 
                    blockLocation = getBlockCoordFromRelative(x, y, z);


            if(this.hasChunk(chunkLocation)) { this.mChunkList[chunkLocation].setBlock(x % 16, y % 16, z % 16, material); }
            else if(force) {
                this.mChunkList.Add(chunkLocation, new VanillaChunk(new Vector3(16,16,16), chunkLocation, this));
                this.mChunkList[chunkLocation].setBlock(x % 16, y % 16, z % 16, material);
            } 
            else { return; }
        }

        public override string getMaterialFromName(string name) {
            return VanillaChunk.staticBlock[name].getMaterial();
        }


    }
}
