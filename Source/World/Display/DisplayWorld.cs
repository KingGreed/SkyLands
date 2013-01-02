using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;
using Game.Terrain;

namespace Game.Display
{
    public class DisplayWorld
    {
        private SceneManager mSceneMgr;
        private Materials mtr;

        public DisplayWorld(SceneManager sceneMgr)
        {
            this.mSceneMgr = sceneMgr;
            this.mtr = new Materials();
        }

        public void displayAllChunks(){
            foreach (var chunk in World.chunkArray) {
	            this.displayChunkAt(chunk.Key);
	        }
        }

        public void displayChunkAt(Vector3 coord) {
            if(!World.chunkArray.ContainsKey(coord)){
                LogManager.Singleton.DefaultLog.LogMessage("Key : " + coord.ToString() + " was not found in mChunkArray");
                throw new KeyNotFoundException("Key was not found in mChunkArray");
            }
            Chunk displayingChunk = World.chunkArray[coord];

            for(int x = 0; x < World.CHUNK_SIDE; x++) {
                for(int y = 0; y < World.CHUNK_SIDE; y++) {
                    for(int z = 0; z < World.CHUNK_SIDE; z++) {
                        displayFaces(coord, new Vector3(x, y, z), getDisplayableFacesAt(coord, new Vector3(x, y, z)));
                    }
                }
            }
        }

        public List<GraphicBlock.blockFace> getDisplayableFacesAt(Vector3 chunkCoord, Vector3 blockCoord)
        {

            List<GraphicBlock.blockFace> returnList = new List<GraphicBlock.blockFace>();
            Block block = World.getBlock(chunkCoord, blockCoord);
            if (block != null && block.IsAir()) { return returnList; }

            Dictionary<GraphicBlock.blockFace, Vector3> coordToCheck = new Dictionary<GraphicBlock.blockFace,Vector3>();

            coordToCheck.Add(GraphicBlock.blockFace.rightFace, new Vector3(blockCoord.x + 1, blockCoord.y,     blockCoord.z));
            coordToCheck.Add(GraphicBlock.blockFace.leftFace,  new Vector3(blockCoord.x - 1, blockCoord.y,     blockCoord.z));
            coordToCheck.Add(GraphicBlock.blockFace.upperFace, new Vector3(blockCoord.x,     blockCoord.y + 1, blockCoord.z));
            coordToCheck.Add(GraphicBlock.blockFace.underFace, new Vector3(blockCoord.x,     blockCoord.y - 1, blockCoord.z));
            coordToCheck.Add(GraphicBlock.blockFace.frontFace, new Vector3(blockCoord.x,     blockCoord.y,     blockCoord.z + 1));
            coordToCheck.Add(GraphicBlock.blockFace.backFace,  new Vector3(blockCoord.x,     blockCoord.y,     blockCoord.z - 1));


            foreach (KeyValuePair<GraphicBlock.blockFace, Vector3> keyVal in coordToCheck)
            {
                Block tempBlock = World.getBlock(chunkCoord, keyVal.Value);
                if (tempBlock != null && tempBlock.IsAir()) { returnList.Add(keyVal.Key); }
            }
            return returnList;
        }

        public void displayFaces(Vector3 chunkCoord, Vector3 blockCoord, List<GraphicBlock.blockFace> faceToDisplay) {
            if(faceToDisplay.Count == 0) { return; }

            Block block = World.getBlock(chunkCoord, blockCoord);
            if (block == null) { return; }
            
            Vector3 absCoord = DisplayWorld.getAbsoluteCoordAt(chunkCoord, blockCoord);
            string faceName, faceEntName, cubeNodeName = "cubeNode-" + absCoord.x + "-" + absCoord.y + "-" + absCoord.z;
            SceneNode blockNode;
            Entity ent;
            string type = block.getType();


            if(this.mSceneMgr.HasSceneNode(cubeNodeName)) {
                blockNode = this.mSceneMgr.GetSceneNode(cubeNodeName);
            } else {
                blockNode = this.mSceneMgr.RootSceneNode.CreateChildSceneNode(cubeNodeName, absCoord);
            }

            foreach(var face in faceToDisplay) {
                faceName = GraphicBlock.getFaceName(face);
                faceEntName = "face-" + absCoord.x + "-" + absCoord.y + "-" + absCoord.z + "-" + faceName;

                ent = this.mSceneMgr.CreateEntity(faceEntName, faceName);


                ent.SetMaterialName(this.mtr.getMaterial(type, face));


                blockNode.AttachObject(ent);

            }

        }

        public static Vector3 getAbsoluteCoordAt(Vector3 chunkCoord, Vector3 blockCoord){
            float x, y, z; //absolute coord
            x = chunkCoord.x * World.CHUNK_SIDE * World.CUBE_SIDE + blockCoord.x * World.CUBE_SIDE;
            y = chunkCoord.y * World.CHUNK_SIDE * World.CUBE_SIDE + blockCoord.y * World.CUBE_SIDE;
            z = chunkCoord.z * World.CHUNK_SIDE * World.CUBE_SIDE + blockCoord.z * World.CUBE_SIDE;

            return new Vector3(x, y, z);

        }
    }
}
