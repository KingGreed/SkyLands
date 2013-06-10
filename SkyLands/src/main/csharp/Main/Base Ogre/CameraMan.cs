using System;
using Mogre;

namespace Game.BaseApp
{
    public class CameraMan
    {
        private Camera  mCamera;
        private Vector3 mDirectionFactor;
        private bool    mFastMove;
        private bool    mFreeze;

        public Camera Camera     { get { return this.mCamera; } }
        public bool FastMove     { set { mFastMove = value; }     get { return mFastMove; } }
        public bool Freeze       { set { mFreeze = value; }       get { return mFreeze; } }

        public CameraMan(Camera camera) { this.mCamera = camera; }

        public void UpdateCamera(float frameTime, Controller input = null)
        {

            if (input != null) { this.UpdateKeys(input); }
            if (this.mFreeze) { return; }

            Vector3 move = Vector3.ZERO;
            move += this.mCamera.Direction * this.mDirectionFactor.z;
            move += this.mCamera.Right * this.mDirectionFactor.x;
            move += this.mCamera.Up * this.mDirectionFactor.y;

            move.Normalise();

            move *= 800;
            if (this.mFastMove) { move *= 6; }
            if (move != Vector3.ZERO) { this.mCamera.Move(move * frameTime); }
        }

        private void UpdateKeys(Controller input)
        {
            this.mDirectionFactor = new Vector3(-input.MovementFactor.x,
                input.HasActionOccured(Controller.UserAction.Up) ? 1 : input.HasActionOccured(Controller.UserAction.Down) ? -1 : 0,
                input.MovementFactor.y);
            this.FastMove = input.IsActionOccuring(Controller.UserAction.Sprint);

            this.MouseMovement(input.Yaw, input.Pitch);
        }

        public void MouseMovement(float yaw, float pitch) {
            if (this.mFreeze) { return; }
            this.mCamera.Yaw(new Degree(-yaw * 0.15f));
            this.mCamera.Pitch(new Degree(-pitch * 0.15f));
        }
    }
}