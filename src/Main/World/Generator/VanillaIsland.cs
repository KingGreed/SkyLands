using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;
using API.Generator;
using API.Generic;

using Mogre;

using Game.Display;

namespace Game.World.Generator
{
    public abstract class VanillaIsland : Island
    {
        private Materials mtr;

        public VanillaIsland(Vector3 islandCoord, Vector2 size) : base(islandCoord, size) {
            this.mtr = new Materials();
        }
       
        public override void initChunks(Vector2 size) {
            for(int x = 0; x < size.x; x++) {
                for(int z = 0; z < size.y; z++) {
                    this.mChunkList.Add(new Vector3(x, 0, z), new VanillaChunk(new Vector3(16,16,16), new Vector3(x, 0, z), this));
                }
            }
        }


        public override Block getBlock(int x, int y, int z) {

            if(x >= this.mIslandSize.x * MainWorld.CHUNK_SIDE || y >= this.mIslandSize.y * MainWorld.CHUNK_SIDE || z >= this.mIslandSize.z * MainWorld.CHUNK_SIDE || y < 0) {
                return new VanillaBlock(new Vector3(0,0,0), new Vector3(0,0,0));
            }

            Vector3 chunkLocation = new Vector3(0, 0, 0),
                    blockLocation = new Vector3(0, 0, 0);
            
            chunkLocation.x = x / MainWorld.CHUNK_SIDE;
            chunkLocation.y = y / MainWorld.CHUNK_SIDE;
            chunkLocation.z = z / MainWorld.CHUNK_SIDE;

            blockLocation.x = x % MainWorld.CHUNK_SIDE;
            blockLocation.y = y % MainWorld.CHUNK_SIDE;
            blockLocation.z = z % MainWorld.CHUNK_SIDE;

            if(blockLocation.x < 0) { chunkLocation.x--; blockLocation.x += 16;}
            if(blockLocation.y < 0) { chunkLocation.y--; blockLocation.y += 16; }
            if(blockLocation.z < 0) { chunkLocation.z--; blockLocation.z += 16;}

            if(this.hasChunk(chunkLocation)) { return this.mChunkList[chunkLocation].getBlock(blockLocation); }
            else { return new VanillaBlock(new Vector3(0,0,0), new Vector3(0,0,0)); }
        }

        public override int getSurfaceHeight(int x, int z) {
            for(int y = (int)this.mIslandSize.y * MainWorld.CHUNK_SIDE; y != 0 ; y--) { 
                if(!this.getBlock(x, y, z).isAir()) {
                    return y + 1; 
                }
            }
            return -1;
        }

        public override Chunk getChunkFromBlock(int x, int y, int z) { throw new NotImplementedException(); }
        public override bool hasBlock(int x, int y, int z)           { throw new NotImplementedException(); }
        public override API.Generic.Material getBlockMaterial(int x, int y, int z) { throw new NotImplementedException(); }


        public override void display(SceneManager sceneMgr) {

            List<MultiBlock> multiList = new List<MultiBlock>();
            Block curr;

            for(int x = 0; x < this.mIslandSize.x * MainWorld.CHUNK_SIDE; x++) {
                for(int y = 0; y < this.mIslandSize.y * MainWorld.CHUNK_SIDE; y++) {
                    for(int z = 0; z < this.mIslandSize.z * MainWorld.CHUNK_SIDE; z++) {
                        curr = this.getBlock(x, y, z);

                        if(!curr.isAir() && this.isMultiBlockBegin(x, y, z, curr.getMaterial())) {
                            multiList.Add(this.getMultiBlockAt(x, y, z, curr.getMaterial()));
                        }

                    }
                }
            }

            foreach(MultiBlock multi in multiList) {
                multi.display(sceneMgr, this);
            }
        }

        public bool isMultiBlockBegin(int x, int y, int z, API.Generic.Material mat) {
            return this.getBlock(x-1, y, z).hasSameMaterialThan(mat)
                && this.getBlock(x, y-1, z).hasSameMaterialThan(mat)
                && this.getBlock(x, y, z-1).hasSameMaterialThan(mat);
        }

        public bool isMultiBlockBegin(Vector3 coord, API.Generic.Material mat) {
            return this.isMultiBlockBegin((int) coord.x, (int) coord.y, (int) coord.z, mat);
        }

        public MultiBlock getMultiBlockAt(int x, int y, int z, API.Generic.Material mat) {

            Stack<Vector3> adjacent  = new Stack<Vector3>();
            MultiBlock     result    = new VanillaMultiBlock(mat);
            Vector3        curr      = Vector3.ZERO;
            Vector3[]      adjCoords;

            Block          adjBlock;

            adjacent.Push(new Vector3(x, y, z));

            while(adjacent.Count != 0) {
                curr = adjacent.Pop();
                result.addBlock(curr);
                
                adjCoords = new Vector3[] {
                            new Vector3(curr.x + 1, curr.y, curr.z),
                            new Vector3(curr.x, curr.y + 1, curr.z),
                            new Vector3(curr.x, curr.y, curr.z + 1)
                };

                foreach(Vector3 loc in adjCoords) {
                    adjBlock = this.getBlock(loc);
                    if(adjBlock.isNotAir() && adjBlock.hasSameMaterialThan(mat)) {
                        adjacent.Push(loc);
                    }
                }

            }
            return result;
        }

        public void setVisibleFaces(Vector3 blockCoord, Block curr)
        {

            if (curr.isAir()) { return; }

            Dictionary<BlockFace, Vector3> coordToCheck = new Dictionary<BlockFace,Vector3>();

            coordToCheck.Add(BlockFace.rightFace, new Vector3(blockCoord.x + 1, blockCoord.y,     blockCoord.z));
            coordToCheck.Add(BlockFace.leftFace,  new Vector3(blockCoord.x - 1, blockCoord.y,     blockCoord.z));
            coordToCheck.Add(BlockFace.upperFace, new Vector3(blockCoord.x,     blockCoord.y + 1, blockCoord.z));
            coordToCheck.Add(BlockFace.underFace, new Vector3(blockCoord.x,     blockCoord.y - 1, blockCoord.z));
            coordToCheck.Add(BlockFace.frontFace, new Vector3(blockCoord.x,     blockCoord.y,     blockCoord.z + 1));
            coordToCheck.Add(BlockFace.backFace,  new Vector3(blockCoord.x,     blockCoord.y,     blockCoord.z - 1));


            foreach (KeyValuePair<BlockFace, Vector3> keyVal in coordToCheck)
            {
                if (this.getBlock(keyVal.Value).isAir()) { curr.setVisibleFaceAt(keyVal.Key, true); }
            }
        }
        
        public void displayFaces(Vector3 blockCoord, List<CubeFace> faceToDisplay, SceneManager sceneMgr) {
            if(faceToDisplay.Count == 0) { return; }

            Block block = this.getBlock(blockCoord);
            if (block == null) { return; }
            
            Vector3 absCoord = this.getAbsoluteCoordAt(blockCoord);
            string faceName, faceEntName, cubeNodeName = "cubeNode-" + absCoord.x + "-" + absCoord.y + "-" + absCoord.z;
            SceneNode blockNode;
            Entity ent;
            string type = ((int)block.getMaterial()).ToString();

            if(sceneMgr.HasSceneNode(cubeNodeName)) {
                blockNode = sceneMgr.GetSceneNode(cubeNodeName);
            } else {
                blockNode = sceneMgr.RootSceneNode.CreateChildSceneNode(cubeNodeName, absCoord);
            }

            foreach(var face in faceToDisplay) {
                faceName = GraphicBlock.getFaceName(face);
                faceEntName = "face-" + absCoord.x + "-" + absCoord.y + "-" + absCoord.z + "-" + faceName;

                ent = sceneMgr.CreateEntity(faceEntName, faceName);

                ent.SetMaterialName(this.mtr.getMaterial(type, face));

                blockNode.AttachObject(ent);

            }

        }

        public Vector3 getAbsoluteCoordAt(Vector3 loc) {
            return new Vector3(loc.x * MainWorld.CUBE_SIDE + this.mIslandCoord.x * MainWorld.CHUNK_SIDE, 
                               loc.y * MainWorld.CUBE_SIDE + this.mIslandCoord.y * MainWorld.CHUNK_SIDE,
                               loc.z * MainWorld.CUBE_SIDE + this.mIslandCoord.z * MainWorld.CHUNK_SIDE);
        }
        public void checkAndUpdate(Vector3 loc) {
            if(!this.hasChunk(loc)) {
                this.mChunkList.Add(loc, new VanillaChunk(new Vector3(16,16,16), loc, this));
                if(loc.y > this.mIslandSize.y) {
                    this.mIslandSize.y = loc.y;
                }
            }
        }

    }
}
