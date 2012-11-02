using System;
using System.Collections.Generic;
using Mogre;

using Game.CharacSystem;

namespace Game.Land
{
    public class World
    {
        private const int NUMBER_CHUNK_X = 1;
        private const int NUMBER_CHUNK_Y = 1;
        private const int NUMBER_CHUNK_Z = 1;
        private const int CHUNK_SIDE = 16;
        private const int CUBE_SIDE = 10;   // Small CUBE_SIZE since Sinbad.mesh is small as well

        public SceneNode mNode { get; private set; }
        private Chunk[, ,] mChunkArray;// { get; private set; }

        public Vector3 mSpawnPoint { get; private set; }


        public World(ref SceneManager sceneMgr)
        {
            mNode = sceneMgr.RootSceneNode.CreateChildSceneNode();
            mChunkArray = new Chunk[NUMBER_CHUNK_X, NUMBER_CHUNK_Y, NUMBER_CHUNK_Z];
            mSpawnPoint = Vector3.ZERO;

            sceneMgr.AmbientLight = new ColourValue(1, 1, 1);

            CreateWorld(ref sceneMgr);
            Console.WriteLine("Created");
            GenerateWorld();
        }

        /* Create each chunks and blocks. Blocks are AIR by default */
        private void CreateWorld(ref SceneManager sceneMgr)
        {
            Vector3 chunkPos;
            for (int x = 0; x < NUMBER_CHUNK_X; x++)
            {
                for (int y = 0; y < NUMBER_CHUNK_Y; y++)
                {
                    for (int z = 0; z < NUMBER_CHUNK_Z; z++)
                    {
                        chunkPos = new Vector3(x, y, z);
                        mChunkArray[x, y, z] = new Chunk(ref sceneMgr, mNode.CreateChildSceneNode(CHUNK_SIDE * CUBE_SIDE * chunkPos), "-Chunk-" + chunkPos.ToString(), CHUNK_SIDE, CUBE_SIDE);
                    }
                }
            }
        }

        /* Algorithm which modify the type of the blocks */
        private void GenerateWorld()
        {
            ChangeTypeBlock(Vector3.ZERO, Vector3.ZERO, TypeBlock.METAL);
            ChangeTypeBlock(Vector3.ZERO, Vector3.UNIT_Y, TypeBlock.METAL);
            ChangeTypeBlock(Vector3.ZERO, Vector3.UNIT_X, TypeBlock.METAL);
            ChangeTypeBlock(Vector3.ZERO, Vector3.UNIT_Z, TypeBlock.METAL);
            ChangeTypeBlock(Vector3.ZERO, new Vector3(1, 1, 0), TypeBlock.METAL);
            ChangeTypeBlock(Vector3.ZERO, Vector3.UNIT_SCALE, TypeBlock.METAL);
            ChangeTypeBlock(Vector3.ZERO, new Vector3(1, 1, 2), TypeBlock.METAL);
        }

        /* Changing the type of a block may change its own and its neihboors' visibility */
        private void ChangeTypeBlock(Vector3 chunkPos, Vector3 blockPos, TypeBlock type)
        {
            Block modifiedBlock = getBlock(chunkPos, blockPos);

            if (modifiedBlock != null && modifiedBlock.SetType(type))
            {
                //Block[] blockArray = new Block[6];  // Array of the 6 blocks which need an update for their face's visibility
                Vector3[] difference = new Vector3[6]   // Array used to get the 6 blocks next to the modified block
                {
                    new Vector3(1, 0, 0),   // RIGHT
                    new Vector3(-1, 0, 0),  // LEFT
                    new Vector3(0, 1, 0),   // TOP
                    new Vector3(0, -1, 0),  // DOWN
                    new Vector3(0, 0, 1),   // FRONT
                    new Vector3(0, 0, -1),  // BEHIND
                };
                BlockFace[] faceArray = new BlockFace[6]    // The faces which will be modified (in correspondence with blockArray)
                {
                    BlockFace.LEFT,
                    BlockFace.RIGHT,
                    BlockFace.DOWN,
                    BlockFace.TOP,
                    BlockFace.BEHIND,
                    BlockFace.FRONT,
                };

                for (int i = 0; i < 6; i++)
                {
                    Block actualBlock = getBlock(chunkPos, blockPos + difference[i]);

                    if (actualBlock != null && actualBlock.isFaceVisible(faceArray[i]))
                    {
                        modifiedBlock.SetFaceVisibility(Block.opposite(faceArray[i]), false);
                        actualBlock.SetFaceVisibility(faceArray[i], false);
                    }
                }
            }
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
