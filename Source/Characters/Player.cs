using System;
using Mogre;

using Game.BaseApp;


namespace Game.CharacSystem
{
    class Player : Character
    {
        private const float YAW_SENSIVITY = 0.1f;
        private const float PITCH_SENSIVITY = 0.15f;

        private enum CameraView { DEBUG, FIRST_PERSON, THIRD_PERSON }
        private CameraView mCamView;
        private bool mIsMainPlayer;

        private Camera mPlayerCam = null;
        private SceneNode mCamPitchNode = null;
        private CameraMan mCameraMan = null;

        public Player(Race race, CharacterInfo info, SceneManager sceneMgr = null, string cameraName = null) : base(race, info)
        {
            this.mIsMainPlayer = sceneMgr != null && cameraName != null;

            if (this.mIsMainPlayer)
            {
                this.mPlayerCam = sceneMgr.GetCamera(cameraName);
                this.ChangeCameraMode(CameraView.FIRST_PERSON, true);
            }
        }

        public void Update(float frameTime, MoisManager input)
        {
            if (input.IsKeyDown(MOIS.KeyCode.KC_F1))      { this.ChangeCameraMode(CameraView.DEBUG); }
            else if (input.IsKeyDown(MOIS.KeyCode.KC_F2)) { this.ChangeCameraMode(CameraView.FIRST_PERSON); }
            //else if (input.IsKeyDown(MOIS.KeyCode.KC_F3)) { this.ChangeCameraMode(CameraView.THIRD_PERSON); }

            this.ProcessMouseInput(frameTime, input);

            Vector3 moveDirection = new Vector3();

            if (this.mCamView == CameraView.DEBUG)  // Don't move the character anymore in debug camera
                this.mCameraMan.UpdateCamera(frameTime, input);
            else
            {
                if (input.IsKeyDown(MOIS.KeyCode.KC_W) || input.IsKeyDown(MOIS.KeyCode.KC_UP))    { moveDirection.z = 1; }
                if (input.IsKeyDown(MOIS.KeyCode.KC_S) || input.IsKeyDown(MOIS.KeyCode.KC_DOWN))  { moveDirection.z = -1; }
                if (input.IsKeyDown(MOIS.KeyCode.KC_A) || input.IsKeyDown(MOIS.KeyCode.KC_LEFT))  { moveDirection.x = 1; }
                if (input.IsKeyDown(MOIS.KeyCode.KC_D) || input.IsKeyDown(MOIS.KeyCode.KC_RIGHT)) { moveDirection.x = -1; }
            }

            base.Move(moveDirection);
            base.Update(frameTime);
        }

        private void ProcessMouseInput(float frameTime, MoisManager input)
        {
            if (this.mCamView == CameraView.DEBUG) { this.mCameraMan.MouseMovement(input.MouseMoveX, input.MouseMoveY); }

            else if (this.mCamView == CameraView.FIRST_PERSON)
            {
                this.mRace.Node.Yaw(-input.MouseMoveX * frameTime * YAW_SENSIVITY);

                this.mCamPitchNode.Pitch(input.MouseMoveY * frameTime * PITCH_SENSIVITY);

                float pitchAngle = 2 * new Degree(Mogre.Math.ACos(this.mCamPitchNode.Orientation.w)).ValueAngleUnits;
                float pitchAngleSign = this.mCamPitchNode.Orientation.x;

                // Limit the pitch between -90 degrees and +90 degrees
                if (pitchAngle > 90.0f)
                {
                    if (pitchAngleSign > 0) { this.mCamPitchNode.SetOrientation(Mogre.Math.Sqrt(0.5f), Mogre.Math.Sqrt(0.5f), 0, 0); }
                    else if (pitchAngleSign < 0) { this.mCamPitchNode.SetOrientation(Mogre.Math.Sqrt(0.5f), -Mogre.Math.Sqrt(0.5f), 0, 0); }
                }
            }
            else if (this.mCamView == CameraView.THIRD_PERSON) { }
        }

        private void ChangeCameraMode(CameraView mode, bool firstInit = false)
        {
            if (this.mCamView == mode && !firstInit)
                return;
            
            this.mCamView = mode;

            if (this.mCamView == CameraView.DEBUG)
            {
                this.mRace.Node.SetVisible(true);

                this.mPlayerCam.DetachFromParent();
                this.mPlayerCam.Position += this.mRace.Node.Position;
                this.mCameraMan = new CameraMan(this.mPlayerCam);
            }
            else if (this.mCamView == CameraView.FIRST_PERSON)
            {
                this.mRace.Node.SetVisible(false);

                this.mCamPitchNode = null;
                this.mCamPitchNode = this.mRace.Node.CreateChildSceneNode();
                this.mCamPitchNode.AttachObject(this.mPlayerCam);

                this.mPlayerCam.SetPosition(0, this.mRace.Height / 2 - 10, 0);  // Camera is set at eyes level
                this.mPlayerCam.Orientation = new Quaternion(1, 0, 0, 0);
                this.mPlayerCam.Yaw(new Degree(180));

            }
            else if (this.mCamView == CameraView.THIRD_PERSON) {}
        }
    }
}
