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
            //CaelumSharp.CaelumScript.Singleton.LoadScript("test");
            //this.mStateMgr.SceneManager.ShadowTechnique = ShadowTechnique.SHADOWTYPE_TEXTURE_MODULATIVE;
            this.mCaelumSystem = new CaelumSystem(this.mStateMgr.Root, this.mStateMgr.SceneManager, CaelumSystem.CaelumComponent.All);
            this.mStateMgr.Root.FrameStarted += mCaelumSystem.FrameStarted;
            this.mStateMgr.Window.PreViewportUpdate += mCaelumSystem.PreViewportUpdate;

            //this.mCaelumSystem.TimeScale = 100;
            this.mCaelumSystem.SceneFogDensityMultiplier = 0.0007f;
            this.mCaelumSystem.ManageSceneFog = true;
            //this.mCaelumSystem.ManageAmbientLight = true;
            //this.mCaelumSystem.EnsureSingleLightSource = true;
            //this.mCaelumSystem.EnsureSingleShadowSource = true;

            //this.mCaelumSystem.SetSkyGradientsImage("CustomSkyGradient2.png");

            /*mCaelumSystem.PrecipitationController = new PrecipitationController(this.mStateMgr.SceneManager);

            mCaelumSystem.PrecipitationController.CreateViewportInstance(this.mStateMgr.Viewport);

            mCaelumSystem.PrecipitationController.PresetType = PrecipitationType.Rain;
            mCaelumSystem.PrecipitationController.Intensity = 0.2f;
            mCaelumSystem.PrecipitationController.Speed = 0.1f;*/
            
            /* Sky */
            this.mCaelumSystem.SkyDome = new SkyDome(this.mStateMgr.SceneManager, this.mCaelumSystem.GetCaelumCameraNode());
            //this.mCaelumSystem.SkyDome.HazeEnabled = true;
            //this.mCaelumSystem.SkyDome.SkyGradientsImage = "CustomSkyGradient2.png";
            //this.mCaelumSystem.SkyDome.SkyGradientsImage = "CustomSkyGradient.png";
            //this.mCaelumSystem.SkyDome.HazeColour = new ColourValue(0.86f, 0.89f, 0.89f);
            //this.mCaelumSystem.SkyDome.AtmosphereDepthImage = "CustomAtmosphereDepth.png";
            //this.mCaelumSystem.SkyDome.QueryFlags = 0;
            //this.mCaelumSystem.SkyDome.VisibilityFlags = 256;
            //this.mCaelumSystem.SkyDome.SetAutoRadius();
            //this.mCaelumSystem.SkyDome.SunDirection = new Vector3(0, 1, 0);
            
            /* Sun */
            this.mCaelumSystem.Sun = new SpriteSun(this.mStateMgr.SceneManager, this.mCaelumSystem.GetCaelumCameraNode(), "Custom_sun_disc.png", 4);
            this.mCaelumSystem.Sun.AmbientMultiplier = new ColourValue(0.8f, 0.8f, 0.8f);
            this.mCaelumSystem.Sun.DiffuseMultiplier = new ColourValue(3, 3, 2.7f);
            this.mCaelumSystem.Sun.SpecularMultiplier = new ColourValue(5, 5, 5);
            this.mCaelumSystem.Sun.AutoDisable = true;
            this.mCaelumSystem.Sun.AutoDisableThreshold = 0.05f;
            //this.mCaelumSystem.Sun.Node.SetPosition(0, 1, 0);
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