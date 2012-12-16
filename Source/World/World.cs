using System;
using System.Collections.Generic;
using Mogre;

using Game.CharacSystem;
using Game.Terrain;
using Game.Display;
using Game.States;
using Game.BaseApp;

namespace Game
{
    public class World : State
    {
        public const int NUMBER_CHUNK_X = 6;
        public const int NUMBER_CHUNK_Y = 2;
        public const int NUMBER_CHUNK_Z = 6;
        public const int CHUNK_SIDE = 16;
        public const int CUBE_SIDE = 50;

        private Vector3 mSpawnPoint;
        private SceneNode mNode;
        private CharacMgr mCharacMgr;

        Dictionary<Vector3, Chunk> mChunkArray;


        public World(StateManager stateMgr) : base(stateMgr)
        { 
            this.mSpawnPoint = Vector3.ZERO; 
            this.mChunkArray = new Dictionary<Vector3, Chunk>();
        }

        public override bool Startup(){
            if (this.mIsStartedUp) { return false; }

            this.mIsStartedUp = true;

            this.mNode        = this.mStateMgr.SceneManager.RootSceneNode.CreateChildSceneNode("TerrainNode"); 

            GraphicBlock.generateFace();

            this.createWorld();   LogManager.Singleton.DefaultLog.LogMessage("World Created");
            this.generateWorld(); LogManager.Singleton.DefaultLog.LogMessage("World Generated");
            this.populate();      LogManager.Singleton.DefaultLog.LogMessage("World Populated");

            DisplayWorld wrld = new DisplayWorld(ref this.mChunkArray, this, this.mStateMgr.SceneManager);

            wrld.displayAllChunks(); LogManager.Singleton.DefaultLog.LogMessage("World Displayed");

            return true;
        }

        private void createWorld()
        {
            Vector3 chunkPos;
            SceneNode chunkNode;
            for (int x = 0; x < NUMBER_CHUNK_X; x++) {
                for (int y = 0; y < NUMBER_CHUNK_Y; y++) {
                    for (int z = 0; z < NUMBER_CHUNK_Z; z++) {
                        chunkPos  = new Vector3(x, y, z);
                        chunkNode = this.mNode.CreateChildSceneNode("chunkNode;" + x + ";" + y + ";" + z); chunkNode.SetPosition(x, y, z);

                        this.mChunkArray.Add(new Vector3(x, y, z), new Chunk(this.mStateMgr.SceneManager, chunkNode));
                    }
                }
            }
        }

        private void generateWorld() {} /* Algorithm of terrain generation */

        private void populate(){
            this.mCharacMgr = new CharacMgr();
            this.mCharacMgr.AddPlayer(new Race(this.mStateMgr.SceneManager, "Sinbad.mesh"),
                                      new CharacterInfo("Sinbad", new Vector3(50, 1600, 10)),
                                      this.mStateMgr.Input, this.mStateMgr.Camera); // Optional parameters that make it the main player
        }

        public override void Hide() { }
        public override void Show() { }

        public override void Update(float frameTime)
        {
            this.mCharacMgr.Update(frameTime);

            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }    // Return to the MenuState
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

        public override void Shutdown()
        {
            this.mStateMgr = null;
            this.mIsStartedUp = false;
        }
    }
}