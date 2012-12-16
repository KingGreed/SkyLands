using System;
using Mogre;

using Game.BaseApp;

namespace Game.CharacSystem
{
    public struct MainPlayerCarac
    {
        internal static float YAW_SENSIVITY = 0.1f;   // Temp
        internal static float PITCH_SENSIVITY = 0.15f;   // Temp
        internal static float PITCH_OFFSET = 0.2f;   // Temp

        public enum CamView { DEBUG, FIRST_PERSON, THIRD_PERSON }
        private MoisManager mInput;
        private Camera      mCam;
        private CamView     mCamView;
        private SceneNode   mCamYawNode;
        private SceneNode   mCamPitchNode;
        private CameraMan   mCameraMan;
        private bool        mIsPlayerMoving;
        private bool        mIsDebugMode;
        private bool        mIsMainPlayer;

        public MoisManager Input      { get { return this.mInput; } }
        public Camera Camera          { get { return this.mCam; } }
        public CamView CameraView     { get { return this.mCamView; } set { this.mCamView = value; } }
        public SceneNode CamYawNode   { get { return this.mCamYawNode; } set { this.mCamYawNode = value; } }
        public SceneNode CamPitchNode { get { return this.mCamPitchNode; } set { this.mCamPitchNode = value; } }
        public CameraMan CameraMan    { get { return this.mCameraMan; } set { this.mCameraMan = value; } }
        public bool IsPlayerMoving    { get { return this.mIsPlayerMoving; } set { this.mIsPlayerMoving = value; } }
        public bool IsDebugMode       { get { return this.mIsDebugMode; } set { this.mIsDebugMode = value; } }
        public bool IsMainPlayer      { get { return this.mIsMainPlayer; } }

        public MainPlayerCarac(MoisManager input, Camera cam)
        {
            mInput          = input;
            mCam            = cam;
            mCamView        = CamView.FIRST_PERSON;
            mCamYawNode     = null;
            mCamPitchNode   = null;
            mCameraMan      = null;
            mIsPlayerMoving = true;
            mIsDebugMode    = false;
            mIsMainPlayer   = true;
        }
    }
}
