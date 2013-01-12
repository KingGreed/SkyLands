using System;
using System.Collections.Generic;
using Mogre;

using Game.CharacSystem;
using Game.BaseApp;
using Game.Display;

namespace Game {

    public partial class World
    {
        public override void Hide() { }
        public override void Show() { }

        public override void Update(float frameTime)
        {
            if (!this.mIsWorldLoaded)
            {
                if (this.mIsWorldGenerated)
                {
                    new DisplayWorld(this.mStateMgr.SceneManager).displayAllChunks();
                    this.mSkyMgr.AddListeners();
                    this.mLoadingBar.Dispose();
                    LogManager.Singleton.DefaultLog.LogMessage("World Displayed");
                    this.mIsWorldLoaded = true;
                }
                else
                {
                    this.mLoadingBar.Value += 100 * frameTime;
                    return;
                }
            }
            
            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_F1))
            {
                this.mIsDebugMode = !this.mIsDebugMode;

                ((Player)this.mCharacMgr.GetCharacter()).IsDebugMode = this.mIsDebugMode;
                
                if (this.mIsDebugMode)
                {
                    Camera cam = this.mCharacMgr.MainPlayerCam.Camera;
                    Vector3 position = cam.RealPosition;
                    Quaternion orientation = cam.RealOrientation;
                    cam.DetachFromParent();
                    cam.Position = position;
                    cam.Orientation = orientation;
                    
                    this.mCameraMan = new CameraMan(cam);
                }
                else 
                    this.mCharacMgr.MainPlayerCam.InitCamera();
            }

            float delta = 100;
            if (this.mStateMgr.Input.MouseMoveZ > 0)      { this.mSkyMgr.TimeScale += delta; }
            else if (this.mStateMgr.Input.MouseMoveZ < 0) { this.mSkyMgr.TimeScale -= delta; }

            this.mCharacMgr.Update(frameTime);

            if (!this.mCharacMgr.GetCharacter().IsMoving)
            {
                MoisManager input = ((Player)this.mCharacMgr.GetCharacter()).Input;
                this.mCameraMan.MouseMovement(input.MouseMoveX, input.MouseMoveY);
                this.mCameraMan.UpdateCamera(frameTime, input);
            }

            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }    // Return to the MenuState
        }

        public override void Shutdown() {
            if (this.mStateMgr == null) { return; }

            this.mSkyMgr.Shutdown();
            this.mStateMgr.SceneManager.ClearScene();

            this.mIsStartedUp = false;
        }
    }
}
