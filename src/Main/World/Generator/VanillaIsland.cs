using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;
using API.Generator;
using API.Generic;

using Mogre;

using Game.Display;

using Material = API.Generic.Material;


namespace Game.World.Generator
{
    public abstract class VanillaIsland : Island
    {

        public VanillaIsland(Vector3 islandCoord, Vector2 size, MainWorld currentWorld) : base(islandCoord, size, currentWorld) {}
       
        public override void initChunks(Vector2 size) {
            for(int x = 0; x < size.x; x++) {
                for(int z = 0; z < size.y; z++) {
                    this.mChunkList.Add(new Vector3(x, 0, z), new VanillaChunk(new Vector3(16,16,16), new Vector3(x, 0, z), this));
                }
            }
        }

        public override Block getBlock(int x, int y, int z, bool force) {
            if(force && y > this.mIslandSize.y * MainWorld.CHUNK_SIDE) {
                this.mIslandSize.y = (int) System.Math.Ceiling((float)y / 16f);
            }
            if(x >= this.mIslandSize.x * MainWorld.CHUNK_SIDE || y >= this.mIslandSize.y * MainWorld.CHUNK_SIDE || z >= this.mIslandSize.z * MainWorld.CHUNK_SIDE || y < 0 || x < 0 || z < 0) {
                return new VanillaBlock(new Vector3(0,0,0));
            }

            Vector3 chunkLocation = new Vector3(0, 0, 0),
                    blockLocation = new Vector3(0, 0, 0);
            
            chunkLocation.x = x / MainWorld.CHUNK_SIDE;
            chunkLocation.y = y / MainWorld.CHUNK_SIDE;
            chunkLocation.z = z / MainWorld.CHUNK_SIDE;

            blockLocation.x = x % MainWorld.CHUNK_SIDE;
            blockLocation.y = y % MainWorld.CHUNK_SIDE;
            blockLocation.z = z % MainWorld.CHUNK_SIDE;

            /*if(blockLocation.x < 0) { chunkLocation.x--; blockLocation.x += 16;}
            if(blockLocation.y < 0) { chunkLocation.y--; blockLocation.y += 16; }
            if(blockLocation.z < 0) { chunkLocation.z--; blockLocation.z += 16;}*/

            if(this.hasChunk(chunkLocation)) { return this.mChunkList[chunkLocation].getBlock(blockLocation); }
            else if(force) {
                this.mChunkList.Add(chunkLocation, new VanillaChunk(new Vector3(16,16,16), chunkLocation, this));
                return this.mChunkList[chunkLocation].getBlock(blockLocation);
            } 
            else { return new VanillaBlock(new Vector3(0,0,0)); }        }

        public override Vector3 getBlockCoord(int x, int y, int z) {
            if(x >= this.mIslandSize.x * MainWorld.CHUNK_SIDE || y >= this.mIslandSize.y * MainWorld.CHUNK_SIDE || z >= this.mIslandSize.z * MainWorld.CHUNK_SIDE || y < 0) {
                return -Vector3.UNIT_SCALE;
            }
            Vector3 chunkLocation = new Vector3(0, 0, 0),
                    blockLocation = new Vector3(0, 0, 0);
            
            chunkLocation.x = x / MainWorld.CHUNK_SIDE;
            chunkLocation.y = y / MainWorld.CHUNK_SIDE;
            chunkLocation.z = z / MainWorld.CHUNK_SIDE;

            blockLocation.x = x % MainWorld.CHUNK_SIDE;
            blockLocation.y = y % MainWorld.CHUNK_SIDE;
            blockLocation.z = z % MainWorld.CHUNK_SIDE;

            if(this.hasChunk(chunkLocation)) { return blockLocation; }
            else { return -Vector3.UNIT_SCALE; }
        }

        public override int getSurfaceHeight(int x, int z) {
            for(int y = (int)this.mIslandSize.y * MainWorld.CHUNK_SIDE; y != 0 ; y--) { 
                if(!this.getBlock(x, y, z, false).isAir()) {
                    return y + 1; 
                }
            }
            return -1;
        }

        public override Chunk getChunkFromBlock(int x, int y, int z) { throw new NotImplementedException(); }
        public override bool hasBlock(int x, int y, int z)           { throw new NotImplementedException(); }
        public override API.Generic.Material getBlockMaterial(int x, int y, int z) { throw new NotImplementedException(); }


        public override void display(SceneManager sceneMgr) {

            Dictionary<Material, MultiBlock> multiList = new Dictionary<Material, MultiBlock>();
            Block curr;

            foreach (Material mat in Enum.GetValues(typeof(Material))) {
                if(mat != Material.AIR) {
                    multiList.Add(mat, new VanillaMultiBlock(mat));
                }
            }

            for(int x = 0; x < this.mIslandSize.x * MainWorld.CHUNK_SIDE; x++) {
                for(int y = 0; y < this.mIslandSize.y * MainWorld.CHUNK_SIDE; y++) {
                    for(int z = 0; z < this.mIslandSize.z * MainWorld.CHUNK_SIDE; z++) {
                        curr = this.getBlock(x, y, z, false);
                        if(!curr.isAir() && this.setVisibleFaces(new Vector3(x, y, z), curr)) {
                            multiList[curr.getMaterial()].addBlock(new Vector3(x, y, z));
                        }
                    }
                }
            }
            LogManager.Singleton.DefaultLog.LogMessage("MultiBlock done !");
            foreach(KeyValuePair<Material, MultiBlock> pair in multiList) {
                pair.Value.display(sceneMgr, this, this.mWorld);
            }
        }


        //For optimization purpose returns wether the block has visible faces
        public bool setVisibleFaces(Vector3 blockCoord, Block curr)
        {

            if (curr.isAir()) { return false; }
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
                if (this.getBlock(keyVal.Value, false).isAir()) { curr.setVisibleFaceAt(keyVal.Key, true); hasVisiblefaces = true; }
            }
            return hasVisiblefaces;
        }

        public Vector3 getAbsoluteCoordAt(Vector3 loc) {
            return new Vector3(loc.x * MainWorld.CUBE_SIDE + this.mIslandCoord.x * MainWorld.CHUNK_SIDE, 
                               loc.y * MainWorld.CUBE_SIDE + this.mIslandCoord.y * MainWorld.CHUNK_SIDE,
                               loc.z * MainWorld.CUBE_SIDE + this.mIslandCoord.z * MainWorld.CHUNK_SIDE);
        }

    }
}
