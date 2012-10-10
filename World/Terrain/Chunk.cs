﻿using System;
using System.Collections.Generic;
using Mogre;


namespace Game.Terrain
{

    public class Chunk
    {
        public const int HEIGHT = 16;
        public const int WIDTH = 16;

        private SceneManager mSceneMgr;
        private SceneNode mChunkNode;
        private Vector3 mChunkPos;

        private World mWorld;

        Dictionary<Vector3, Block> mLoadedChunks = new Dictionary<Vector3, Block>();

        public Chunk(ref Vector3 chunkPos, ref SceneManager sceneMgr)
        {
            this.mSceneMgr = sceneMgr;
            this.mChunkPos = chunkPos;

            this.mChunkNode = sceneMgr.RootSceneNode.CreateChildSceneNode("ChunkNode");
        }

        public void displayChunk()
        {
            Entity ent;
            SceneNode entNode;
            for (int x = 0; x < HEIGHT; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    for (int z = 0; z < HEIGHT; z++)
                    {
                        ent = mSceneMgr.CreateEntity("cube" + x + "-" + y + "-" + z, "cube.mesh");
                        ent.SetMaterialName("Cube");
                        entNode = this.mChunkNode.CreateChildSceneNode("cubeNode" + x + "-" + y + "-" + z, new Vector3(100 * x, 100 * y, 100 * z));

                        entNode.AttachObject(ent);


                    }


                }

            }

        }


    }
}