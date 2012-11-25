using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;
using Game.Terrain;

namespace Game.Display
{
    class DisplayWorld
    {
        private Dictionary<Vector3, Chunk> mChunkArray;
        private World mWorld;
        private SceneManager mSceneMgr;

        public DisplayWorld(ref Dictionary<Vector3, Chunk> chunkArray, World refToWorld, ref SceneManager sceneMgr)
        {
            this.mChunkArray = chunkArray;
            this.mWorld = refToWorld;
            this.mSceneMgr = sceneMgr;
        }

        public void DisplayChunkAt(Vector3 coord) {

            if(!this.mChunkArray.ContainsKey(coord)){
                LogManager.Singleton.DefaultLog.LogMessage("Key : " + coord.ToString() + " was not found in mChunkArray");
                throw new KeyNotFoundException("Key was not found in mChunkArray");
            }
            Chunk displayingChunk = this.mChunkArray[coord];

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
            if (this.mWorld.getBlock(chunkCoord, blockCoord).IsAir())
            {
                return returnList;
            }

            Dictionary<GraphicBlock.blockFace, Vector3> coordToCheck = new Dictionary<GraphicBlock.blockFace,Vector3>();

            coordToCheck.Add(GraphicBlock.blockFace.rightFace, new Vector3(blockCoord.x + 1, blockCoord.y, blockCoord.z));
            coordToCheck.Add(GraphicBlock.blockFace.leftFace, new Vector3(blockCoord.x - 1, blockCoord.y, blockCoord.z));
            coordToCheck.Add(GraphicBlock.blockFace.upperFace, new Vector3(blockCoord.x, blockCoord.y + 1, blockCoord.z));
            coordToCheck.Add(GraphicBlock.blockFace.underFace, new Vector3(blockCoord.x, blockCoord.y - 1, blockCoord.z));
            coordToCheck.Add(GraphicBlock.blockFace.frontFace, new Vector3(blockCoord.x, blockCoord.y, blockCoord.z + 1));
            coordToCheck.Add(GraphicBlock.blockFace.backFace, new Vector3(blockCoord.x, blockCoord.y, blockCoord.z - 1));


            foreach(var block in coordToCheck) {
                if (this.mWorld.getBlock(chunkCoord, block.Value).IsAir())
                {
                    returnList.Add(block.Key);
                }
            }
            return returnList;
        }

        public void displayFaces(Vector3 chunkCoord, Vector3 blockCoord, List<GraphicBlock.blockFace> faceToDisplay) {

            if(faceToDisplay.Count == 0) { return; }

            Vector3 absoluteCoord = DisplayWorld.getAbsoluteCoordAt(chunkCoord, blockCoord);
            string faceName, faceEntName, cubeNodeName = getCubeNodeName(absoluteCoord);
            SceneNode blockNode;
            Entity ent;


            if(this.mSceneMgr.HasSceneNode(cubeNodeName)) {
                blockNode = this.mSceneMgr.GetSceneNode(cubeNodeName);
            } else {
                blockNode = this.mSceneMgr.RootSceneNode.CreateChildSceneNode(cubeNodeName, absoluteCoord);
            }

            foreach(var face in faceToDisplay) {
                faceName = GraphicBlock.getFaceName(face);
                faceEntName = getFaceName(absoluteCoord, faceName);

                ent = this.mSceneMgr.CreateEntity(faceEntName, faceName);
                ent.SetMaterialName("Cube");
                blockNode.AttachObject(ent);

            }

        }

        public static string getCubeNodeName(Vector3 absCoord){
            return "cubeNode-" + absCoord.x + "-" + absCoord.y + "-" + absCoord.z;
        }


        public static string getFaceName(Vector3 absCoord, string face)
        {
            return "face-" + absCoord.x + "-" + absCoord.y + "-" + absCoord.z + "-" + face;
        }

        public static Vector3 getAbsoluteCoordAt(Vector3 chunkCoord, Vector3 blockCoord){
            float x, y, z; //absolute coord
            x = chunkCoord.x * World.CHUNK_SIDE + blockCoord.x * World.CUBE_SIDE;
            y = chunkCoord.y * World.CHUNK_SIDE + blockCoord.y * World.CUBE_SIDE;
            z = chunkCoord.z * World.CHUNK_SIDE + blockCoord.z * World.CUBE_SIDE;

            return new Vector3(x, y, z);

        }
    }
}
