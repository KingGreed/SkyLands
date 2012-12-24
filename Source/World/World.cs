using System;
using System.Collections.Generic;
using Mogre;
using CaelumSharp;

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

        private CaelumSystem mCaelumSystem;
        private Vector3 mSpawnPoint;
        private SceneNode mNode;
        private CharacMgr mCharacMgr;
        private CameraMan mCameraMan;
        private bool mIsDebugMode;
        
        public static Dictionary<Vector3, Chunk> chunkArray;

        public World(StateManager stateMgr) : base(stateMgr)
        {
            this.mSpawnPoint = Vector3.ZERO;
            this.mIsDebugMode = false;
            this.mCameraMan = null;

            chunkArray = new Dictionary<Vector3, Chunk>();
        }


        public override bool Startup(){
            if (this.mIsStartedUp) { return false; }

            this.mIsStartedUp = true;

            this.mNode        = this.mStateMgr.SceneManager.RootSceneNode.CreateChildSceneNode("TerrainNode"); 

            GraphicBlock.generateFace();

            this.createSky(); LogManager.Singleton.DefaultLog.LogMessage("Sky Created");
            this.generateWorld(); LogManager.Singleton.DefaultLog.LogMessage("World Generated");
            this.populate();      LogManager.Singleton.DefaultLog.LogMessage("World Populated");

            DisplayWorld wrld = new DisplayWorld(this.mStateMgr.SceneManager);

            wrld.displayAllChunks(); LogManager.Singleton.DefaultLog.LogMessage("World Displayed");

            return true;
        }

        private void createSky()
        {
            this.mCaelumSystem = new CaelumSystem(this.mStateMgr.Root, this.mStateMgr.SceneManager, CaelumSystem.CaelumComponent.Default);
            this.mStateMgr.Root.FrameStarted += mCaelumSystem.FrameStarted;
            this.mStateMgr.Window.PreViewportUpdate += mCaelumSystem.PreViewportUpdate;

            this.mCaelumSystem.SceneFogDensityMultiplier = 0.0008f;
            this.mCaelumSystem.ManageSceneFog = true;
        }

        private void generateWorld() {new Island(new Vector2(7, 7));} /* Algorithm of terrain generation */

        private void populate() {
            this.mCharacMgr = new CharacMgr(this.mStateMgr.Camera);
            this.mCharacMgr.AddPlayer(this.mStateMgr.SceneManager, "Sinbad.mesh",
                                      new CharacterInfo("Sinbad", new Vector3(50, 1600, 10)),
                                      this.mStateMgr.Input);
        }
    }
}