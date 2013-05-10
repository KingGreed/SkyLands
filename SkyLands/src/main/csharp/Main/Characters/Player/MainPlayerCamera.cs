using Mogre;

namespace Game.CharacSystem
{
    public class MainPlayerCamera
    {
        private readonly VanillaPlayer mPlayer;
        private readonly Camera        mCam;
        private SceneNode     mCamYawNode, mCamPitchNode;
        private bool          mIsCameraFirstView;   // Else is in third view

        public Degree Pitch { get { return this.mCamPitchNode.Orientation.Pitch; } }

        public MainPlayerCamera(Camera cam, VanillaPlayer p)
        {
            this.mPlayer = p;
            this.mCam = cam;

            this.mIsCameraFirstView = true;
            this.InitCamera();
        }

        public void Update()
        {
            if (this.mPlayer.IsFirstView != this.mIsCameraFirstView)
                this.SwitchCameraView();

            /* Pitch the camera */
            this.mCamPitchNode.Pitch(new Degree(this.mPlayer.MovementInfo.PitchValue));

            float pitchAngle = 2 * new Degree(Math.ACos(this.mCamPitchNode.Orientation.w)).ValueAngleUnits;
            float pitchAngleSign = this.mCamPitchNode.Orientation.x;

            if (pitchAngle > 90.0f) // Limit the pitch between -90 degrees and +90 degrees
            {
                if      (pitchAngleSign > 0) { this.mCamPitchNode.SetOrientation(Math.Sqrt(0.5f), Math.Sqrt(0.5f), 0, 0); }
                else if (pitchAngleSign < 0) { this.mCamPitchNode.SetOrientation(Math.Sqrt(0.5f), -Math.Sqrt(0.5f), 0, 0); }
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
                this.mCamPitchNode.AttachObject(this);

                ((Camera)this).SetPosition(0, this.mPlayer.Size.y / 2 - 13, 0);  // Camera is set at eyes level
                ((Camera)this).Orientation = new Quaternion(1, 0, 0, 0);
            }
            else { }
        }

        public static implicit operator Camera(MainPlayerCamera mainPlayerCam)
        {
            return mainPlayerCam.mCam;
        }
    }
}
