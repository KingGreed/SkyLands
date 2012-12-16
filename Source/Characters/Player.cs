using System;
using Mogre;

namespace Game.CharacSystem
{
    class Player : Character
    {
        private MainPlayerCarac mMainPlayerCarac;

        public MainPlayerCarac MainPlayerCarac { get { return this.mMainPlayerCarac; } }

        public Player(Race race, CharacterInfo info, MoisManager  input = null, Camera cam = null) : base(race, info)
        {
            this.mMainPlayerCarac = new MainPlayerCarac(input, cam);

            if (this.mMainPlayerCarac.IsMainPlayer)
                this.ChangeCameraMode(MainPlayerCarac.CamView.FIRST_PERSON, true);
        }

        public override void Update(float frameTime)
        {
            if (this.mMainPlayerCarac.IsMainPlayer) { this.ProcessMainPlayerUpdate(frameTime); }
        }

        #region MainPlayer
        private void ProcessMainPlayerUpdate(float frameTime)
        {
            MoisManager input = this.mMainPlayerCarac.Input;

            if (input.WasKeyPressed(MOIS.KeyCode.KC_F1))    // Switch debug mode
            {
                this.mMainPlayerCarac.IsDebugMode = !this.mMainPlayerCarac.IsDebugMode;

                if (this.mMainPlayerCarac.IsDebugMode)
                    this.ChangeCameraMode(MainPlayerCarac.CamView.DEBUG);
                else
                    this.ChangeCameraMode(MainPlayerCarac.CamView.FIRST_PERSON);
            }
            if (!this.mMainPlayerCarac.IsDebugMode && input.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Middle))  // Switch camera view
            {
                if (this.mMainPlayerCarac.CameraView == CharacSystem.MainPlayerCarac.CamView.FIRST_PERSON)
                    this.ChangeCameraMode(CharacSystem.MainPlayerCarac.CamView.THIRD_PERSON);
                else
                    this.ChangeCameraMode(CharacSystem.MainPlayerCarac.CamView.FIRST_PERSON);
            }

            this.mMainPlayerCarac.IsPlayerMoving = !this.mMainPlayerCarac.IsDebugMode || input.IsKeyDown(MOIS.KeyCode.KC_LCONTROL) || input.IsKeyDown(MOIS.KeyCode.KC_RCONTROL);

            if (!this.mMainPlayerCarac.IsPlayerMoving)
                this.ProcessDebugUpdate(frameTime);
            else    // Move the player if we are in debug view with ctrl pressed or if we aren't in debug view
            {
                float yawValue   = -input.MouseMoveX * frameTime * MainPlayerCarac.YAW_SENSIVITY;
                float pitchValue = -input.MouseMoveY * frameTime * MainPlayerCarac.PITCH_SENSIVITY;

                if (this.mMainPlayerCarac.CameraView == MainPlayerCarac.CamView.THIRD_PERSON)
                    this.ProcessThirdPersonUpdate(yawValue, pitchValue);
                else
                    this.ProcessFirstPersonUpdate(yawValue, pitchValue);
            }

            base.Update(frameTime);
        }

        private void ProcessDebugUpdate(float frameTime)
        {
            MoisManager input = this.mMainPlayerCarac.Input;            
            
            this.mMainPlayerCarac.CameraMan.MouseMovement(input.MouseMoveX, input.MouseMoveY);
            this.mMainPlayerCarac.CameraMan.UpdateCamera(frameTime, input);
        }

        private void ProcessFirstPersonUpdate(float yawValue, float pitchValue)
        {
            MoisManager input = this.mMainPlayerCarac.Input;

            /* Move the player */
            Vector3 moveDirection = new Vector3();
            if (input.IsKeyDown(MOIS.KeyCode.KC_W) || input.IsKeyDown(MOIS.KeyCode.KC_UP))    { moveDirection.z = 1; }
            if (input.IsKeyDown(MOIS.KeyCode.KC_S) || input.IsKeyDown(MOIS.KeyCode.KC_DOWN))  { moveDirection.z = -1; }
            if (input.IsKeyDown(MOIS.KeyCode.KC_A) || input.IsKeyDown(MOIS.KeyCode.KC_LEFT))  { moveDirection.x = 1; }
            if (input.IsKeyDown(MOIS.KeyCode.KC_D) || input.IsKeyDown(MOIS.KeyCode.KC_RIGHT)) { moveDirection.x = -1; }

            /* Yaw the player */
            this.mMovementInfo.YawValue = yawValue;

            /* Pitch the camera */
            Radian newPitch = Mogre.Math.Abs(pitchValue + this.mMainPlayerCarac.CamPitchNode.Orientation.Pitch);
            if (newPitch < new Radian(Mogre.Math.PI / 2 - MainPlayerCarac.PITCH_OFFSET) || newPitch > new Radian(Mogre.Math.PI / 2 + MainPlayerCarac.PITCH_OFFSET))
                this.mMainPlayerCarac.CamPitchNode.Pitch(pitchValue);
            this.mMovementInfo.MoveDirection = moveDirection;
        }

        private void ProcessThirdPersonUpdate(float yawValue, float pitchValue) { }

        private void ChangeCameraMode(MainPlayerCarac.CamView mode, bool firstInit = false)
        {
            if (!this.mMainPlayerCarac.IsMainPlayer || (this.mMainPlayerCarac.CameraView == mode && !firstInit))
                return;
            
            this.mMainPlayerCarac.CameraView = mode;

            if (this.mMainPlayerCarac.CameraView == MainPlayerCarac.CamView.DEBUG)
            {
                this.mRace.Node.SetVisible(true);

                this.mMainPlayerCarac.Camera.DetachFromParent();
                this.mMainPlayerCarac.Camera.Position += this.mRace.Node.Position;
                this.mMainPlayerCarac.CameraMan = new Game.BaseApp.CameraMan(this.mMainPlayerCarac.Camera);
            }
            else if (this.mMainPlayerCarac.CameraView == MainPlayerCarac.CamView.FIRST_PERSON)
            {
                this.mRace.Node.SetVisible(false);

                this.mMainPlayerCarac.CamYawNode = this.mRace.Node.CreateChildSceneNode();

                this.mMainPlayerCarac.CamPitchNode = this.mMainPlayerCarac.CamYawNode.CreateChildSceneNode();
                this.mMainPlayerCarac.CamPitchNode.AttachObject(this.mMainPlayerCarac.Camera);

                this.mMainPlayerCarac.Camera.SetPosition(0, this.mRace.Height / 2 - 10, 0);  // Camera is set at eyes level
                this.mMainPlayerCarac.Camera.Orientation = new Quaternion(1, 0, 0, 0);
                this.mMainPlayerCarac.CamYawNode.Yaw(new Degree(180));

            }
            else if (this.mMainPlayerCarac.CameraView == MainPlayerCarac.CamView.THIRD_PERSON) {}
        }
        #endregion
    }
}
