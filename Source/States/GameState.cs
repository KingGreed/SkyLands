using System;
using Mogre;

using Game.Terrain;
using Game.CharacSystem;
using Game.Display;
using Game.BaseApp;


namespace Game.States
{
    class GameState : State
    {
        private CharacMgr mCharacMgr;
        private Camera mCamera; // Replace the camera from BaseApplication
        private CameraMan mCameraMan;
        
        public GameState() : base()
        {
            this.mCharacMgr = new CharacMgr();
        }

        public override bool Startup(StateManager stateMgr)
        {
            if (this.mIsStartedUp)
                return false;

            this.mStateMgr = stateMgr;
            this.mIsStartedUp = true;

            if (!CreateScene())
                return false;

            return true;
        }

        private bool CreateScene()
        {
            LogManager.Singleton.DefaultLog.LogMessage("Creating scene");

            GraphicBlock.generateFace();
            new World(this.mStateMgr.SceneManager);

            this.mCharacMgr = new CharacMgr();
            this.mCharacMgr.AddPlayer(new Race(this.mStateMgr.SceneManager, "Sinbad.mesh"), new CharacterInfo("Sinbad", new Vector3(0, 0, -250)));

            this.CreateCamera();
            this.CreateViewports();

            return true;
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
            // Create one viewport, entire window
            var vp = this.mStateMgr.Window.AddViewport(mCamera);
            vp.BackgroundColour = ColourValue.Black;

            // Alter the camera aspect ratio to match the viewport
            this.mCamera.AspectRatio = (vp.ActualWidth / vp.ActualHeight);
        }

        public override void Update(float frameTime)
        {
            this.UpdateCamera(frameTime);
            
            this.mCharacMgr.Update(frameTime);
        }

        private void UpdateCamera(float frameTime)
        {
            MoisManager input = this.mStateMgr.Input;
            
            if (input.IsKeyDown(MOIS.KeyCode.KC_W) || input.IsKeyDown(MOIS.KeyCode.KC_UP)) { this.mCameraMan.GoingForward = true; }
            else { this.mCameraMan.GoingForward = false; }
            if (input.IsKeyDown(MOIS.KeyCode.KC_S) || input.IsKeyDown(MOIS.KeyCode.KC_DOWN)) { this.mCameraMan.GoingBack = true; }
            else { this.mCameraMan.GoingBack = false; }
            if (input.IsKeyDown(MOIS.KeyCode.KC_A) || input.IsKeyDown(MOIS.KeyCode.KC_LEFT)) { this.mCameraMan.GoingLeft = true; }
            else { this.mCameraMan.GoingLeft = false; }
            if (input.IsKeyDown(MOIS.KeyCode.KC_D) || input.IsKeyDown(MOIS.KeyCode.KC_RIGHT)) { this.mCameraMan.GoingRight = true; }
            else { this.mCameraMan.GoingRight = false; }
            if (input.IsKeyDown(MOIS.KeyCode.KC_E) || input.IsKeyDown(MOIS.KeyCode.KC_PGUP)) { this.mCameraMan.GoingUp = true; }
            else { this.mCameraMan.GoingUp = false; }
            if (input.IsKeyDown(MOIS.KeyCode.KC_Q) || input.IsKeyDown(MOIS.KeyCode.KC_PGDOWN)) { this.mCameraMan.GoingDown = true; }
            else { this.mCameraMan.GoingDown = false; }
            if (input.IsKeyDown(MOIS.KeyCode.KC_LSHIFT) || input.IsKeyDown(MOIS.KeyCode.KC_RSHIFT)) { this.mCameraMan.FastMove = true; }
            else { this.mCameraMan.FastMove = false; }

            this.mCameraMan.MouseMovement(input.MouseMoveX, input.MouseMoveY);

            this.mCameraMan.UpdateCamera(frameTime);
        }

        public override void Shutdown()
        {
            if (this.mStateMgr == null)
                return;

            this.mStateMgr = null;
            this.mIsStartedUp = false;
        }
    }
}
