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

            this.createSky();     LogManager.Singleton.DefaultLog.LogMessage("Sky Created");
            this.generateWorld(); LogManager.Singleton.DefaultLog.LogMessage("World Generated");
            this.populate();      LogManager.Singleton.DefaultLog.LogMessage("World Populated");

            DisplayWorld wrld = new DisplayWorld(this.mStateMgr.SceneManager);

            wrld.displayAllChunks(); LogManager.Singleton.DefaultLog.LogMessage("World Displayed");

            return true;
        }

        private void createSky()
        {
            this.mCaelumSystem = new CaelumSystem(this.mStateMgr.Root, this.mStateMgr.SceneManager, CaelumSystem.CaelumComponent.None);
            this.mCaelumSystem.AttachViewport(this.mStateMgr.Viewport);
            this.mStateMgr.Window.PreViewportUpdate += mCaelumSystem.PreViewportUpdate;
            this.mStateMgr.Root.FrameStarted += mCaelumSystem.FrameStarted;

            this.mCaelumSystem.GetUniversalClock().SetGregorianDateTime(2012, 12, 21, 12, 0, 0);
            this.mCaelumSystem.TimeScale = 600;

            /* Sky */
            this.mCaelumSystem.SkyDome = new SkyDome(this.mStateMgr.SceneManager, this.mCaelumSystem.GetCaelumCameraNode());

            /* Sun */
            this.mCaelumSystem.ManageAmbientLight = true;
            this.mCaelumSystem.MinimumAmbientLight = new ColourValue(0.2f, 0.2f, 0.3f);            
            this.mCaelumSystem.Sun = new SpriteSun(this.mStateMgr.SceneManager, this.mCaelumSystem.GetCaelumCameraNode(), "Custom_sun_disc.png", 4);
            this.mCaelumSystem.Sun.AmbientMultiplier = new ColourValue(0.8f, 0.8f, 0.8f);
            this.mCaelumSystem.Sun.DiffuseMultiplier = new ColourValue(3, 3, 2.7f);
            this.mCaelumSystem.Sun.SpecularMultiplier = new ColourValue(5, 5, 5);
            this.mCaelumSystem.Sun.AutoDisable = true;
            this.mCaelumSystem.Sun.AutoDisableThreshold = 0.05f;

            /* Moon */
            this.mCaelumSystem.Moon = new Moon(this.mStateMgr.SceneManager, this.mCaelumSystem.GetCaelumCameraNode());
            this.mCaelumSystem.Moon.DiffuseMultiplier = new ColourValue(2, 2, 1.7f);
            this.mCaelumSystem.Moon.SpecularMultiplier = new ColourValue(4, 4, 4);

            /* Stars */
            this.mCaelumSystem.PointStarfield = new PointStarfield(this.mStateMgr.SceneManager, this.mCaelumSystem.GetCaelumCameraNode());
            this.mCaelumSystem.PointStarfield.MagnitudeScale = 1.05f;

            /* Fog */
            this.mCaelumSystem.SceneFogDensityMultiplier = 0.0007f;
            this.mCaelumSystem.SceneFogColourMultiplier = new ColourValue(0.3f, 0.3f, 0.3f);
            this.mCaelumSystem.ManageSceneFog = true;
        }

        private void generateWorld() {new Island(new Vector2(2, 2));} /* Algorithm of terrain generation */

        private void populate() {
            this.mCharacMgr = new CharacMgr(this.mStateMgr.Camera);
            this.mCharacMgr.AddPlayer(this.mStateMgr.SceneManager, "Sinbad.mesh",
                                      new CharacterInfo("Sinbad", new Vector3(50, 1600, 10)),
                                      this.mStateMgr.Input);
        }
    }
}