using System;
using System.Collections.Generic;
using Mogre;

namespace Game.Terrain
{
    public class Chunk
    {
        SceneNode mNode { get; set; }
        public Block[, ,] mBlockArray;

        public Chunk(SceneManager sceneMgr, SceneNode chunkNode)
        {
            this.mNode = chunkNode;
            this.mBlockArray = new Block[World.CHUNK_SIDE, World.CHUNK_SIDE, World.CHUNK_SIDE];

            Vector3 blockPos;
            for (int x = 0; x < World.CHUNK_SIDE; x++)
            {
                for (int y = 0; y < World.CHUNK_SIDE; y++)
                {
                    for (int z = 0; z < World.CHUNK_SIDE; z++)
                    {
                        blockPos = new Vector3 (x, y, z);
                        this.mBlockArray[x, y, z] = new Block(blockPos);
                    }
                }
            }
            /*this.mBlockArray[0, 0, 0].SetType(TypeBlock.AIR);
            this.mBlockArray[0, 0, World.CHUNK_SIDE-1].SetType(TypeBlock.AIR);
            this.mBlockArray[World.CHUNK_SIDE-1, 0, 0].SetType(TypeBlock.AIR);
            this.mBlockArray[World.CHUNK_SIDE-1, 0, World.CHUNK_SIDE-1].SetType(TypeBlock.AIR);

            this.mBlockArray[0, World.CHUNK_SIDE-1, 0].SetType(TypeBlock.AIR);
            this.mBlockArray[0, World.CHUNK_SIDE-1, World.CHUNK_SIDE-1].SetType(TypeBlock.AIR);
            this.mBlockArray[World.CHUNK_SIDE-1, World.CHUNK_SIDE-1, 0].SetType(TypeBlock.AIR);
            this.mBlockArray[World.CHUNK_SIDE-1, World.CHUNK_SIDE-1, World.CHUNK_SIDE-1].SetType(TypeBlock.AIR);

            double sin;
            for(int x = 0; x < World.CHUNK_SIDE; x++){
                    for(int y = 0; y < World.CHUNK_SIDE; y++){
                        for(int z = 0; z < World.CHUNK_SIDE; z++){
                            sin = (System.Math.Sin(x) + 1)*4;
                            if(y >sin + 0.4 ) { this.mBlockArray[x, y, z].SetType(TypeBlock.AIR); };
                        }
                    }
                }*/
        }

    }
}