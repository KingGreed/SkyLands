using System;
using Mogre;

using Game.BaseApp;

namespace Game.CharacSystem
{
    public class MainPlayerCamera
    {
        private VanillaPlayer mPlayer;
        private Camera        mCam;
        private SceneNode     mCamYawNode, mCamPitchNode;
        private bool          mIsCameraFirstView;
        private uint          mWndWidth, mWndHeight;

        public Camera Camera    { get { return this.mCam; } }
        public uint   WndWidth  { get { return this.mWndWidth; } }
        public uint   WndHeight { get { return this.mWndHeight; } }
        public Degree Pitch     { get { return this.mCamPitchNode.Orientation.Pitch; } }

        public MainPlayerCamera(Camera cam, VanillaPlayer p, uint wndWidth, uint wndHeight)
        {
            this.mPlayer            = null;
            this.mCam               = cam;
            this.mIsCameraFirstView = true;
            this.mWndWidth = wndWidth;
            this.mWndHeight = wndHeight;

            this.mPlayer = p;
            this.InitCamera();

        }

        public void Update()
        {
            if (this.mPlayer.IsFirstView != this.mIsCameraFirstView)
                this.SwitchCameraView();

            /* Yaw the camera */
            this.mCamYawNode.Yaw(new Degree(this.mPlayer.YawCamValue));

            /* Pitch the camera */
            this.mCamPitchNode.Pitch(new Degree(this.mPlayer.PitchCamValue));

            float pitchAngle = 2 * new Degree(Mogre.Math.ACos(this.mCamPitchNode.Orientation.w)).ValueAngleUnits;
            float pitchAngleSign = this.mCamPitchNode.Orientation.x;

            if (pitchAngle > 90.0f) // Limit the pitch between -90 degrees and +90 degrees
            {
                if      (pitchAngleSign > 0) { this.mCamPitchNode.SetOrientation(Mogre.Math.Sqrt(0.5f), Mogre.Math.Sqrt(0.5f), 0, 0); }
                else if (pitchAngleSign < 0) { this.mCamPitchNode.SetOrientation(Mogre.Math.Sqrt(0.5f), -Mogre.Math.Sqrt(0.5f), 0, 0); }
            }
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
                this.mCamYawNode.Yaw(new Degree(180));

                this.mCamPitchNode = this.mCamYawNode.CreateChildSceneNode();
                this.mCamPitchNode.AttachObject(this.mCam);

                this.mCam.SetPosition(0, this.mPlayer.Height / 2 - 13, 0);  // Camera is set at eyes level
                this.mCam.Orientation = new Quaternion(1, 0, 0, 0);
            }
            else { }
        }

        public void Dispose() { this.mCam.Dispose();}
    }
}
