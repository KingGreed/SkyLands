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
        private Camera mCamera; // Replace the camera from BaseApplication
        private CameraMan mCameraMan;

        Dictionary<Vector3, Chunk> mChunkArray;


        public World() { 
            this.mSpawnPoint = Vector3.ZERO; 
            this.mChunkArray = new Dictionary<Vector3, Chunk>();
        }

        public override bool Startup(StateManager stateMgr){
            if (this.mIsStartedUp) { return false; }

            this.mStateMgr    = stateMgr;
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
            this.mCharacMgr.AddPlayer(new Race(this.mStateMgr.SceneManager, "Sinbad.mesh"), new CharacterInfo("Sinbad", new Vector3(0, 0, -250)));

            this.CreateCamera();  this.CreateViewports();
        }

        private void CreateCamera()
        {
            this.mCamera = this.mStateMgr.SceneManager.CreateCamera("DebugCam");

            this.mCamera.Position = new Vector3(0, 100, 250);

            this.mCamera.LookAt(new Vector3(0, 50, 0));
            this.mCamera.NearClipDistance = 5;
            this.mCamera.FarClipDistance = 3000;

            this.mCameraMan = new CameraMan(this.mCamera);
        }

        private void CreateViewports()
        {
            var vp = this.mStateMgr.Window.AddViewport(mCamera);
            vp.BackgroundColour = ColourValue.Black;

            this.mCamera.AspectRatio = (vp.ActualWidth / vp.ActualHeight);
        }

        public override void Update(float frameTime)
        {
            MoisManager input = this.mStateMgr.Input;
            this.mCameraMan.UpdateCamera(frameTime, input);
            
            this.mCharacMgr.Update(frameTime);
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
            if (this.mStateMgr == null) { return; }

            this.mStateMgr = null;
            this.mIsStartedUp = false;
        }
    }
}