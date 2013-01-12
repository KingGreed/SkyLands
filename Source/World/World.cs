using System;
using System.Collections.Generic;
using System.Threading;
using Mogre;

using Game.CharacSystem;
using Game.Terrain;
using Game.Display;
using Game.States;
using Game.BaseApp;
using Game.GUICreator;
using Game.Sky;

namespace Game
{
    public partial class World : State
    {
        public const int NUMBER_CHUNK_X = 6;
        public const int NUMBER_CHUNK_Y = 3;
        public const int NUMBER_CHUNK_Z = 6;
        public const int CHUNK_SIDE = 16;
        public const int CUBE_SIDE = 50;

        private SkyMgr mSkyMgr;
        private Vector3 mSpawnPoint;
        private SceneNode mNode;
        private CharacMgr mCharacMgr;
        private CameraMan mCameraMan;
        private bool mIsDebugMode;
        private LoadingBarGUI mLoadingBar;
        private bool mIsWorldGenerated;
        private bool mIsWorldLoaded;
        
        public static Dictionary<Vector3, Chunk> chunkArray;

        public World(StateManager stateMgr) : base(stateMgr)
        {
            this.mSpawnPoint = Vector3.ZERO;
            this.mIsDebugMode = false;
            this.mCameraMan = null;
            this.mLoadingBar = new LoadingBarGUI(this.mStateMgr.MiyagiManager, "LoadingBar GUI");
            this.mSkyMgr = new SkyMgr(this.mStateMgr);
            chunkArray = new Dictionary<Vector3, Chunk>();
        }


        public override bool Startup(){
            if (this.mIsStartedUp) { return false; }

            this.mIsStartedUp = true;
            this.mIsWorldGenerated = false;
            this.mIsWorldLoaded = false;

            new Thread(this.generateWorld).Start();

            this.mNode = this.mStateMgr.SceneManager.RootSceneNode.CreateChildSceneNode("TerrainNode");
            GraphicBlock.generateFace();
            this.mSkyMgr.CreateSky(); LogManager.Singleton.DefaultLog.LogMessage("Sky Created");
            this.populate();          LogManager.Singleton.DefaultLog.LogMessage("World Populated");

            return true;
        }

        private void generateWorld()
        {
            new Island(new Vector2(3, 3)); LogManager.Singleton.DefaultLog.LogMessage("World Generated");
            this.mIsWorldGenerated = true;
        }

        private void populate() {
            this.mCharacMgr = new CharacMgr(this.mStateMgr.Camera);
            this.mCharacMgr.AddPlayer(this.mStateMgr.SceneManager, "Sinbad.mesh",
                                      new CharacterInfo("Sinbad", new Vector3(300, 7000, 1000)),
                                      this.mStateMgr.Input);
        }
    }
}