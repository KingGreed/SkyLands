using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.Terrain
{
    class Island
    {
        private Vector2 mIslandsDim;
        private Dictionary<Vector3, Chunk>  mChunkList;
        private SceneNode mNode;
        private SceneManager mSceneMgr;

        public Island(Vector2 islandDim, SceneNode terrainNode, ref SceneManager sceneMgr){
            this.mIslandsDim = islandDim;
            this.mChunkList  = new Dictionary<Vector3,Chunk>();
            this.mNode = terrainNode;
            this.mSceneMgr = sceneMgr;
        }

        public void createIslandAt(Vector3 coord){
            Vector3 chunkPos;
            SceneNode chunkNode;
            for (int x = 0; x < World.NUMBER_CHUNK_X; x++) {
                for (int y = 0; y < World.NUMBER_CHUNK_Y; y++) {
                    for (int z = 0; z < World.NUMBER_CHUNK_Z; z++) {

                        chunkPos  = coord + new Vector3(x, y, z);
                        chunkNode = this.mNode.CreateChildSceneNode("chunkNode;" + chunkPos.x + ";" + chunkPos.y + ";" + chunkPos.z);
                        chunkNode.SetPosition(chunkPos.x, chunkPos.y, chunkPos.z);

                        this.mChunkList.Add(new Vector3(chunkPos.x, chunkPos.y, chunkPos.z), new Chunk(ref this.mSceneMgr, chunkNode));
                    }
                }
            }
        }

        public void generateTerrain(){
            
        }

    }
}
