using System;
using System.Collections.Generic;
using Mogre;

namespace Game.Land
{
    class Chunk
    {
        public SceneNode mNode { get; private set; }
        public Block[, ,] mBlockArray { get; internal set; }

        public Chunk(ref SceneManager sceneMgr, SceneNode chunkNode, string chunkName, int chunkSide, int cubeSide)
        {
            mNode = chunkNode;
            mBlockArray = new Block[chunkSide, chunkSide, chunkSide];

            Vector3 blockPos;
            for (int x = 0; x < chunkSide; x++)
            {
                for (int y = 0; y < chunkSide; y++)
                {
                    for (int z = 0; z < chunkSide; z++)
                    {
                        blockPos = new Vector3 (x, y, z);
                        //Console.WriteLine("Create Block " + blockPos.ToString());
                        mBlockArray[x, y, z] =
                            new Block(ref sceneMgr, mNode.CreateChildSceneNode(cubeSide * blockPos), "-Block-" + blockPos.ToString() + chunkName, cubeSide);
                    }
                }
            }
        }

    }
}