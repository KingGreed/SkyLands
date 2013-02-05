using System;
using Mogre;

using Game.BaseApp;
using Game.CharacSystem;

namespace Game
{
    public class DebugMode
    {
        private bool        mIsDebugMode;
        private CameraMan   mCameraMan;
        private MoisManager mInput;
        private CharacMgr   mCharacMgr;

        public DebugMode(MoisManager input, CharacMgr characMgr)
        {
            this.mInput = input;
            this.mCharacMgr = characMgr;
            this.mIsDebugMode = false;
            this.mCameraMan = null;
        }

        public void Update(float frameTime)
        {
            if (this.mInput.WasKeyPressed(MOIS.KeyCode.KC_F1))
            {
                this.mIsDebugMode = !this.mIsDebugMode;

                ((VanillaPlayer)this.mCharacMgr.GetCharacter()).IsDebugMode = this.mIsDebugMode;

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

            this.mCharacMgr.Update(frameTime);

            if (!this.mCharacMgr.GetCharacter().IsMoving)
            {
                this.mCameraMan.MouseMovement(this.mInput.MouseMoveX, this.mInput.MouseMoveY);
                this.mCameraMan.UpdateCamera(frameTime, this.mInput);
            }
        }

        public void Dispose() { this.mCharacMgr.Dispose(); }
    }
}
