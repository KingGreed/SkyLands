using System;
using Mogre;

using Game.BaseApp;

namespace Game.CharacSystem
{
    public class MainPlayerCamera
    {
        private static float PITCH_OFFSET = 0.2f;

        private Player      mPlayer;
        private Camera      mCam;
        private SceneNode   mCamYawNode;
        private SceneNode   mCamPitchNode;
        private bool        mIsCameraFirstView;

        public Camera Camera { get { return this.mCam; } }

        public MainPlayerCamera(Camera cam)
        {
            this.mPlayer       = null;
            this.mCam          = cam;
            this.mIsCameraFirstView = true;
        }

        public void AttachToPlayer(Player player)
        {
            this.mPlayer = player;
            this.InitCamera();
        }

        public void Update()
        {
            if (this.mPlayer.IsFirstView != this.mIsCameraFirstView)
                this.SwitchCameraView();
            
            /* Pitch the camera */
            Radian newPitch = Mogre.Math.Abs(this.mPlayer.PitchCamValue + this.mCamPitchNode.Orientation.Pitch);
            if (newPitch < new Radian(Mogre.Math.PI / 2 - MainPlayerCamera.PITCH_OFFSET) || newPitch > new Radian(Mogre.Math.PI / 2 + MainPlayerCamera.PITCH_OFFSET))
                this.mCamPitchNode.Pitch(new Degree(this.mPlayer.PitchCamValue));

            /* Yaw the camera */
            this.mCamYawNode.Yaw(new Degree(this.mPlayer.YawCamValue));
        }

        public void SwitchCameraView()
        {
            this.mIsCameraFirstView = !this.mIsCameraFirstView;
            this.InitCamera();
        }

        public void InitCamera()
        {
            if (this.mIsCameraFirstView)
            {
                this.mPlayer.Node.SetVisible(false);

                this.mCamYawNode = this.mPlayer.Node.CreateChildSceneNode();

                this.mCamPitchNode = this.mCamYawNode.CreateChildSceneNode();
                this.mCamPitchNode.AttachObject(this.mCam);

                this.mCam.SetPosition(0, this.mPlayer.Height / 2 - 10, 0);  // Camera is set at eyes level
                this.mCam.Orientation = new Quaternion(1, 0, 0, 0);
                this.mCamYawNode.Yaw(new Degree(180));

            }
            else { }
        }
    }
}
