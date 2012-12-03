using System;
using System.Collections.Generic;
using Mogre;

using Game.CharacSystem;
using Game.Terrain;
using Game.Display;

namespace Game
{
    public class World
    {
        public const int NUMBER_CHUNK_X = 6;
        public const int NUMBER_CHUNK_Y = 1;
        public const int NUMBER_CHUNK_Z = 6;
        public const int CHUNK_SIDE = 16;
        public const int CUBE_SIDE = 50;

        public SceneNode mNode { get; private set; }
        Dictionary<Vector3, Chunk> mChunkArray;

        public Vector3 mSpawnPoint { get; private set; }


        public World(SceneManager sceneMgr)
        {
            mNode = sceneMgr.RootSceneNode.CreateChildSceneNode("TerrainNode");
            mSpawnPoint = Vector3.ZERO;

            this.mChunkArray = new Dictionary<Vector3, Chunk>();
            LogManager.Singleton.DefaultLog.LogMessage("World Init done");

            CreateWorld(ref sceneMgr);
            LogManager.Singleton.DefaultLog.LogMessage("World Created");

            GenerateWorld();
            LogManager.Singleton.DefaultLog.LogMessage("World Generated");

            DisplayWorld wrld = new DisplayWorld(ref this.mChunkArray, this, ref sceneMgr);
            wrld.DisplayAllChunks();
            LogManager.Singleton.DefaultLog.LogMessage("World Displayed");
        }

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

                        this.mChunkArray.Add(new Vector3(x, y, z), new Chunk(ref sceneMgr, chunkNode));
                    }
                }
            }
        }

        /* Algorithm which modify the type of the blocks */
        private void GenerateWorld()
        {
        }

        public Block getBlock(Vector3 chunkPos, Vector3 blockPos)
        {
            int[] chunkPosArray = new int[3] { (int)chunkPos.x, (int)chunkPos.y, (int)chunkPos.z };
            int[] blockPosArray = new int[3] { (int)blockPos.x, (int)blockPos.y, (int)blockPos.z };

            for (int i = 0; i < 3; i++)
            {
                chunkPosArray[i] += blockPosArray[i] / CHUNK_SIDE;  // Division entière
                blockPosArray[i] %= CHUNK_SIDE;

                if (blockPosArray[i] < 0)
                {
                    chunkPosArray[i]--;
                    blockPosArray[i] += CHUNK_SIDE;
                }
            }
            chunkPos = new Vector3(chunkPosArray[0], chunkPosArray[1], chunkPosArray[2]);

            if(chunkPosArray[0] >= NUMBER_CHUNK_X || chunkPosArray[1] >= NUMBER_CHUNK_Y || chunkPosArray[2] >= NUMBER_CHUNK_Z ||
                chunkPosArray[0] < 0 || chunkPosArray[1] < 0 || chunkPosArray[2] < 0) {
                return new Block(new Vector3(-5, 6, 0), TypeBlock.AIR);   // Block out of bounds
            }
            return mChunkArray[chunkPos].mBlockArray[blockPosArray[0], blockPosArray[1], blockPosArray[2]];
        }
    }
}