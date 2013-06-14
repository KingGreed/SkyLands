using Mogre;

using Game.Input;

namespace Game.BaseApp
{
    public class CameraMan
    {
        private readonly Camera  mCamera;
        private Vector3 mDirectionFactor;
        private bool    mFastMove;
        private bool    mFreeze;

        public Camera Camera     { get { return this.mCamera; } }
        public bool   FastMove   { get { return mFastMove; } set { mFastMove = value; } }
        public bool   Freeze     { get { return mFreeze; }   set { mFreeze = value; } }

        public CameraMan(Camera camera) { this.mCamera = camera; }

        public void UpdateCamera(float frameTime, Controller input = null)
        {
            if (input != null) { this.UpdateKeys(frameTime, input); }
            if (this.mFreeze) { return; }

            Vector3 move = Vector3.ZERO;
            move += this.mCamera.Direction * this.mDirectionFactor.z;
            move -= this.mCamera.Right * this.mDirectionFactor.x;
            move += this.mCamera.Up * this.mDirectionFactor.y;

            move.Normalise();

            move *= 800;
            if (this.mFastMove) { move *= 6; }
            if (move != Vector3.ZERO) { this.mCamera.Move(move * frameTime); }
        }

        private void UpdateKeys(float frameTime, Controller input)
        {
            this.mDirectionFactor = input.MovementFactor;
            this.FastMove = input.IsActionOccuring(UserAction.Sprint);

            this.MouseMovement(frameTime, input.Yaw, input.Pitch);
        }

        public void MouseMovement(float frameTime, float yaw, float pitch)
        {
            if (this.mFreeze) { return; }
            this.mCamera.Yaw(new Degree(yaw));
            this.mCamera.Pitch(new Degree(pitch));
        }
    }
}