using System;
using Mogre;
using System.Collections.Generic;

using Game.PlayerSystem;

namespace Game.Terrain
{
    class World
    {
        private SceneManager mSceneMgr;
        private RenderWindow mRenderWindow;
        private List<Player> mPlayerList;
        Dictionary<Vector3, Chunk> mLoadedChunks = new Dictionary<Vector3, Chunk>();

        private Vector3 mSpawnPoint;


        public World(ref SceneManager sceneMgr, ref RenderWindow renderWindow)
        {
            this.mSceneMgr = sceneMgr;
            this.mRenderWindow = renderWindow;
        }

        public void generateWorld() {
            if (mSpawnPoint == null) { this.mSpawnPoint = generateSpawnPoint(); }
            Vector3 chunkPos;
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    for (int z = 0; z < 9; z++)
                    {
                        chunkPos = new Vector3(x, y, z);
                        try
                        {
                            this.mLoadedChunks.Add(chunkPos, new Chunk(ref chunkPos, ref this.mSceneMgr));
                        }
                        catch (ArgumentException)
                        {
                            LogManager.Singleton.DefaultLog.LogMessage("An element with position " + chunkPos + "already exists.");
                        }
                    }
                }
            }
        }

        public Vector3 generateSpawnPoint() {
            return new Vector3(0, 0, 0);
        }
    }
}
