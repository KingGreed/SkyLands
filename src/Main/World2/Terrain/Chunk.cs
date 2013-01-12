using System;
using System.Collections.Generic;
using Mogre;

namespace Game.Terrain
{
    public class Chunk
    {
        private Block[, ,] mBlockArray;

        public Block[, ,] BlockArray { get { return this.mBlockArray; } }

        public Chunk()
        {
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
        }

    }
}