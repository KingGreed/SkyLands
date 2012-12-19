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
    public partial class World : State
    {
        public const int NUMBER_CHUNK_X = 6;
        public const int NUMBER_CHUNK_Y = 3;
        public const int NUMBER_CHUNK_Z = 6;
        public const int CHUNK_SIDE = 16;
        public const int CUBE_SIDE = 50;

        private Vector3 mSpawnPoint;
        private SceneNode mNode;

        private CharacMgr mCharacMgr;
        private Camera mCamera; // Replace the camera from BaseApplication
        private CameraMan mCameraMan;

        public static Dictionary<Vector3, Chunk> chunkArray;

        public World() { 
            this.mSpawnPoint = Vector3.ZERO; 
            chunkArray = new Dictionary<Vector3, Chunk>();
        }

        public override bool Startup(StateManager stateMgr) {
            if (this.mIsStartedUp) { return false; }

            this.mStateMgr    = stateMgr;
            this.mIsStartedUp = true;

            this.mNode        = this.mStateMgr.SceneManager.RootSceneNode.CreateChildSceneNode("TerrainNode"); 

            GraphicBlock.generateFace();

            this.generateWorld(); LogManager.Singleton.DefaultLog.LogMessage("World Generated");
            this.populate();      LogManager.Singleton.DefaultLog.LogMessage("World Populated");

            DisplayWorld wrld = new DisplayWorld(this.mStateMgr.SceneManager);

            wrld.displayAllChunks(); LogManager.Singleton.DefaultLog.LogMessage("World Displayed");

            return true;
        }

        private void generateWorld() {new Island(new Vector2(7, 7));} /* Algorithm of terrain generation */

        private void populate() {
            this.mCharacMgr = new CharacMgr();
            this.mCharacMgr.AddPlayer(new Race(this.mStateMgr.SceneManager, "Sinbad.mesh"), new CharacterInfo("Sinbad", new Vector3(0, 0, -250)));

            this.CreateCamera();  this.CreateViewports();
        }
    }
}