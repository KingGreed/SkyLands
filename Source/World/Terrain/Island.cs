using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;
using Game.LibNoise;

namespace Game.Terrain
{
    class Island
    {
        private Vector2 mIslandsDim;
        private SceneNode mNode;
        private SceneManager mSceneMgr;

        public Island(Vector2 islandDim, SceneNode terrainNode, SceneManager sceneMgr){
            this.mIslandsDim = islandDim;
            this.mNode = terrainNode;
            this.mSceneMgr = sceneMgr;

            this.createIslandAt(new Vector3(0, 0, 0));
            this.generateTerrain();
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

                        World.chunkArray.Add(new Vector3(chunkPos.x, chunkPos.y, chunkPos.z), new Chunk(this.mSceneMgr, chunkNode));
                    }
                }
            }
        }

        public void generateTerrain(){
            Perlin terrain = new Perlin();

            terrain.setFrequency(0.2);
            terrain.setLacunarity(1);
            terrain.setNoiseQuality(NoiseQuality.BEST);
            terrain.setPersistence(0.7);
            terrain.setOctaveCount(1);

            terrain.setSeed(42);


            int terrainheight;
            for(int x = 0; x < World.CHUNK_SIDE * World.NUMBER_CHUNK_X; x++){
                for(int z = 0; z < World.CHUNK_SIDE * World.NUMBER_CHUNK_Z; z++){
                    terrainheight = (int) System.Math.Floor(terrain.GetValue(x, 0, z) * World.CHUNK_SIDE);
                    if(x == 0 && z == 0){
                        LogManager.Singleton.DefaultLog.LogMessage("Terrain Height = " + terrainheight);
                    }
                    this.setBlockToAir(new Vector3(x / World.CHUNK_SIDE, 0, z / World.CHUNK_SIDE), new Vector3(x % World.CHUNK_SIDE, terrainheight, z % World.CHUNK_SIDE));
                }
            }
        }

        private void setBlockToAir(Vector3 chunkPos, Vector3 blockPos){
            int y;
            for(int i = (int)chunkPos.y; i < World.NUMBER_CHUNK_Y; i++){
                if(i == chunkPos.y){ y = (int)blockPos.y; }
                else               { y = 0; }
                for(; y < World.CHUNK_SIDE; y++){
                    World.getBlock(new Vector3(chunkPos.x, i, chunkPos.z), new Vector3(blockPos.x, y, blockPos.z)).SetType(TypeBlock.AIR);
                }
            }
        }
    }
}
