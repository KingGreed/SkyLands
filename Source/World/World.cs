using System;
using System.Collections.Generic;
using Mogre;

using Game.CharacSystem;

namespace Game.Land
{
    public class World
    {
        public static const int NUMBER_CHUNK_X = 1;
        public static const int NUMBER_CHUNK_Y = 1;
        public static const int NUMBER_CHUNK_Z = 1;
        public static const int CHUNK_SIDE = 16;

        public SceneNode mNode { get; private set; }
        private Chunk[, ,] mChunkArray;// { get; private set; }

        public Vector3 mSpawnPoint { get; private set; }


        public World(ref SceneManager sceneMgr)
        {
            mNode = sceneMgr.RootSceneNode.CreateChildSceneNode("TerrainNode");
            mChunkArray = new Chunk[NUMBER_CHUNK_X, NUMBER_CHUNK_Y, NUMBER_CHUNK_Z];
            mSpawnPoint = Vector3.ZERO;

            CreateWorld(ref sceneMgr);
            GenerateWorld();
        }

        /* Create each chunks and blocks. Blocks are AIR by default */
        private void CreateWorld(ref SceneManager sceneMgr)
        {
            Vector3 chunkPos;
            SceneNode chunkNode;
            for (int x = 0; x < NUMBER_CHUNK_X; x++)
            {
                for (int y = 0; y < NUMBER_CHUNK_Y; y++)
                {
                    for (int z = 0; z < NUMBER_CHUNK_Z; z++)
                    {
                        chunkPos  = new Vector3(x, y, z);
                        chunkNode = this.mNode.CreateChildSceneNode("chunkNode;" + x + ";" + y + ";" + z);
                        chunkNode.SetPosition(x, y, z);

                        mChunkArray[x, y, z] = new Chunk(ref sceneMgr, chunkNode);
                    }
                }
            }
        }

        /* Algorithm which modify the type of the blocks */
        private void GenerateWorld()
        {
        }

        private Block getBlock(Vector3 chunkPos, Vector3 blockPos)
        {
            int[] chunkPosArray = new int[3] { (int)chunkPos.x, (int)chunkPos.y, (int)chunkPos.z };
            int[] blockPosArray = new int[3] { (int)blockPos.x, (int)blockPos.y, (int)blockPos.z };

            for (int i = 0; i < blockPosArray.Length; i++)
            {
                chunkPosArray[i] += blockPosArray[i] / CHUNK_SIDE;  // Division entière
                blockPosArray[i] %= CHUNK_SIDE;

                if (blockPosArray[i] < 0)
                {
                    chunkPosArray[i]--;
                    blockPosArray[i] += CHUNK_SIDE;
                }
            }

            if (chunkPosArray[0] > NUMBER_CHUNK_X || chunkPosArray[1] > NUMBER_CHUNK_Y || chunkPosArray[2] > NUMBER_CHUNK_Z ||
                chunkPosArray[0] < 0 || chunkPosArray[1] < 0 || chunkPosArray[2] < 0)
                return null;   // Block out of bounds

            return mChunkArray[chunkPosArray[0], chunkPosArray[1], chunkPosArray[2]].mBlockArray[blockPosArray[0], blockPosArray[1], blockPosArray[2]];
        }
    }
}