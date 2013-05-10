using Mogre;

using Game.BaseApp;
using Game.States;
using Game.CharacSystem;

namespace Game
{
    public class CameraMgr
    {
        private CameraMan              mCameraMan;
        private readonly StateManager  mStateMgr;

        public bool IsAllowedToMoveCam { get; set; }
        public bool IsFreeCamMode { get; private set; }

        public CameraMgr(StateManager stateMgr)
        {
            this.mStateMgr = stateMgr;
            this.mCameraMan = null;
            this.IsAllowedToMoveCam = true;
        }

        public void Update(float frameTime)
        {
            VanillaPlayer mainPlayer = this.mStateMgr.MainState.CharacMgr.MainPlayer;
            if (this.IsFreeCamMode && mainPlayer != null)
                this.IsAllowedToMoveCam = !mainPlayer.MovementInfo.IsAllowedToMove;

            if (this.mCameraMan != null && this.IsAllowedToMoveCam)
            {
                this.mCameraMan.MouseMovement(this.mStateMgr.Input.MouseMoveX, this.mStateMgr.Input.MouseMoveY);
                this.mCameraMan.UpdateCamera(frameTime, this.mStateMgr.Input);
            }
        }

        public void SwitchFreeCamMode()
        {
            this.IsFreeCamMode = !this.IsFreeCamMode;

            MainState mainState = this.mStateMgr.MainState;
            VanillaPlayer mainPlayer = mainState.CharacMgr.MainPlayer;

            if (mainPlayer != null)
            {
                mainState.MainGUI.SwitchVisibility();
                mainPlayer.SwitchFreeCamMode();
            }

            if (this.IsFreeCamMode)
            {
                Camera cam = this.mStateMgr.Camera;
                Vector3 position = cam.RealPosition;
                Quaternion orientation = cam.RealOrientation;
                cam.DetachFromParent();
                cam.Position = position;
                cam.Orientation = orientation;

                this.mCameraMan = new CameraMan(cam);
            }
            else
            {
                if (mainPlayer != null)
                {
                    mainPlayer.MainPlayerCam.InitCamera();
                    mainPlayer.MovementInfo.IsAllowedToMove = true;
                }
                this.mCameraMan = null;
            }
        }
    }
}
