using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;
using API.Generator;

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
            if(blockLocation.z < 0) { chunkLocation.z--; blockLocation.z += 16;}

            if(this.hasChunk(chunkLocation)) { return this.mChunkList[chunkLocation].getBlock(blockLocation); }
            else { return new VanillaBlock(new Vector3(0,0,0), new Vector3(0,0,0)); }
        }

        public override void display(SceneManager sceneMgr) {
            for(int x = 0; x < this.mIslandSize.x * MainWorld.CHUNK_SIDE; x++) {
                for(int y = 0; y < this.mIslandSize.y * MainWorld.CHUNK_SIDE; y++) {
                    for(int z = 0; z < this.mIslandSize.z * MainWorld.CHUNK_SIDE; z++) {
                        List<CubeFace> displayable = this.getDisplayableFacesAt(new Vector3(x, y, z));

                        if(displayable.Count != 0) {
                            displayFaces(new Vector3(x, y, z), displayable, sceneMgr);
                        }

                    }
                }
            }
        }

        public List<CubeFace> getDisplayableFacesAt(Vector3 blockCoord)
        {

            List<CubeFace> returnList = new List<CubeFace>();
            Block block = this.getBlock(blockCoord);
            if (block != null && block.IsAir()) { return returnList; }

            Dictionary<CubeFace, Vector3> coordToCheck = new Dictionary<CubeFace,Vector3>();

            coordToCheck.Add(CubeFace.rightFace, new Vector3(blockCoord.x + 1, blockCoord.y,     blockCoord.z));
            coordToCheck.Add(CubeFace.leftFace,  new Vector3(blockCoord.x - 1, blockCoord.y,     blockCoord.z));
            coordToCheck.Add(CubeFace.upperFace, new Vector3(blockCoord.x,     blockCoord.y + 1, blockCoord.z));
            coordToCheck.Add(CubeFace.underFace, new Vector3(blockCoord.x,     blockCoord.y - 1, blockCoord.z));
            coordToCheck.Add(CubeFace.frontFace, new Vector3(blockCoord.x,     blockCoord.y,     blockCoord.z + 1));
            coordToCheck.Add(CubeFace.backFace,  new Vector3(blockCoord.x,     blockCoord.y,     blockCoord.z - 1));


            foreach (KeyValuePair<CubeFace, Vector3> keyVal in coordToCheck)
            {
                if (this.getBlock(keyVal.Value).IsAir()) { returnList.Add(keyVal.Key); }
            }
            return returnList;
        }
        
        public void displayFaces(Vector3 blockCoord, List<CubeFace> faceToDisplay, SceneManager sceneMgr) {
            if(faceToDisplay.Count == 0) { return; }

            Block block = this.getBlock(blockCoord);
            if (block == null) { return; }
            
            Vector3 absCoord = this.getAbsoluteCoordAt(blockCoord);
            string faceName, faceEntName, cubeNodeName = "cubeNode-" + absCoord.x + "-" + absCoord.y + "-" + absCoord.z;
            SceneNode blockNode;
            Entity ent;
            string type = "1";

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


    }
}
